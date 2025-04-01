// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.Isobands.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a grid of points with z-values and returns filled polygons (isobands)
    /// representing areas between contour lines or breaks.
    /// </summary>
    /// <param name="pointGrid">Array of points forming a grid, with z-values</param>
    /// <param name="breaks">Array of values representing the boundaries of each band</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>A FeatureCollection of Polygon features representing isobands</returns>
    internal static FeatureCollection Isobands(
        Point[] pointGrid,
        double[] breaks,
        IsobandsOptions options = default
    )
    {
        options = IsobandsOptions.OrDefault(options);

        if (pointGrid.Length < 4)
        {
            throw new ArgumentException(
                "Point grid must contain at least 4 points",
                nameof(pointGrid)
            );
        }

        if (breaks.Length < 2)
        {
            throw new ArgumentException(
                "At least two break values must be provided",
                nameof(breaks)
            );
        }

        // Ensure breaks are sorted
        Array.Sort(breaks);

        // Get the z-values from points
        var zValues = new Dictionary<(double X, double Y), double>();
        foreach (var point in pointGrid)
        {
            var coord = point.Coordinate;

            // Try to get z-value from property or z-coordinate
            if (!string.IsNullOrEmpty(options.ZProperty))
            {
                // Try to extract z-value from the point's attributes
                bool foundValue = false;
                double zValue = 0;

                if (point.UserData is NetTopologySuite.Features.IAttributesTable attributes)
                {
                    var value = attributes[options.ZProperty];
                    if (value != null && value is double doubleValue)
                    {
                        zValue = doubleValue;
                        foundValue = true;
                    }
                }

                if (foundValue)
                {
                    zValues[(coord.X, coord.Y)] = zValue;
                    continue;
                }
            }

            // Fall back to z-coordinate if property not found or not specified
            if (!double.IsNaN(coord.Z))
            {
                zValues[(coord.X, coord.Y)] = coord.Z;
            }
            // Skip points without z-values
        }

        // Create a grid cells structure
        var grid = TerritoryUtils.CreateGrid(pointGrid, zValues);

        // For each pair of consecutive breaks, generate an isoband
        var features = new List<Feature>();

        for (int i = 0; i < breaks.Length - 1; i++)
        {
            var lowerBreak = breaks[i];
            var upperBreak = breaks[i + 1];

            // Find all cells that have values within this range
            var polygons = new List<Polygon>();

            foreach (var cell in grid)
            {
                var cellPolygons = FindIsobandPolygonsInCell(cell, lowerBreak, upperBreak, zValues);
                polygons.AddRange(cellPolygons);
            }

            // Merge adjacent polygons
            var mergedPolygons = MergeAdjacentPolygons(polygons);

            // Create a feature for each merged polygon
            foreach (var polygon in mergedPolygons)
            {
                var attributes = new AttributesTable();
                attributes.Add("lowerValue", lowerBreak);
                attributes.Add("upperValue", upperBreak);
                
                var feature = new Feature(polygon, attributes);

                features.Add(feature);
            }
        }

        return new FeatureCollection(features);
    }

    /// <summary>
    /// Finds isoband polygons within a grid cell
    /// </summary>
    private static List<Polygon> FindIsobandPolygonsInCell(
        (Coordinate, Coordinate, Coordinate, Coordinate) cell,
        double lowerBreak,
        double upperBreak,
        Dictionary<(double X, double Y), double> zValues
    )
    {
        // Extract corners
        var (bottomLeft, bottomRight, topRight, topLeft) = cell;

        // Get z-values for each corner
        var z_bl = zValues[(bottomLeft.X, bottomLeft.Y)];
        var z_br = zValues[(bottomRight.X, bottomRight.Y)];
        var z_tr = zValues[(topRight.X, topRight.Y)];
        var z_tl = zValues[(topLeft.X, topLeft.Y)];

        // Count how many corners are within the range
        var cornersInRange = 0;
        if (z_bl >= lowerBreak && z_bl < upperBreak)
            cornersInRange++;
        if (z_br >= lowerBreak && z_br < upperBreak)
            cornersInRange++;
        if (z_tr >= lowerBreak && z_tr < upperBreak)
            cornersInRange++;
        if (z_tl >= lowerBreak && z_tl < upperBreak)
            cornersInRange++;

        // If all corners are within range, the entire cell is in the band
        if (cornersInRange == 4)
        {
            var coords = new Coordinate[]
            {
                bottomLeft,
                bottomRight,
                topRight,
                topLeft,
                bottomLeft, // Close the ring
            };

            return new List<Polygon> { new Polygon(new LinearRing(coords)) };
        }

        // If no corners are within range, check if the cell straddles the band
        if (cornersInRange == 0)
        {
            // Check if any corner is below lower break and any corner is above upper break
            bool anyBelow =
                z_bl < lowerBreak || z_br < lowerBreak || z_tr < lowerBreak || z_tl < lowerBreak;
            bool anyAbove =
                z_bl >= upperBreak
                || z_br >= upperBreak
                || z_tr >= upperBreak
                || z_tl >= upperBreak;

            if (!anyBelow || !anyAbove)
            {
                // Cell is entirely outside the band
                return new List<Polygon>();
            }
        }

        // Calculate intersections with lower and upper isolines
        var lowerIntersections = FindIsolineIntersectionsInCell(cell, lowerBreak, zValues);
        var upperIntersections = FindIsolineIntersectionsInCell(cell, upperBreak, zValues);

        // Complex case: need to determine the shape of the isoband within the cell
        var bandPolygon = ConstructIsobandPolygon(
            cell,
            lowerIntersections,
            upperIntersections,
            lowerBreak,
            upperBreak,
            zValues
        );

        if (bandPolygon != null)
        {
            return new List<Polygon> { bandPolygon };
        }

        return new List<Polygon>();
    }

    /// <summary>
    /// Finds intersection points of an isoline with a grid cell
    /// </summary>
    private static List<Coordinate> FindIsolineIntersectionsInCell(
        (Coordinate, Coordinate, Coordinate, Coordinate) cell,
        double breakValue,
        Dictionary<(double X, double Y), double> zValues
    )
    {
        var intersections = new List<Coordinate>();

        // Extract corners
        var (bottomLeft, bottomRight, topRight, topLeft) = cell;

        // Get z-values for each corner
        var z_bl = zValues[(bottomLeft.X, bottomLeft.Y)];
        var z_br = zValues[(bottomRight.X, bottomRight.Y)];
        var z_tr = zValues[(topRight.X, topRight.Y)];
        var z_tl = zValues[(topLeft.X, topLeft.Y)];

        // Check each edge for intersections
        // Bottom edge
        if ((z_bl < breakValue && z_br >= breakValue) || (z_bl >= breakValue && z_br < breakValue))
        {
            var t = (breakValue - z_bl) / (z_br - z_bl);
            var x = bottomLeft.X + t * (bottomRight.X - bottomLeft.X);
            intersections.Add(new Coordinate(x, bottomLeft.Y));
        }

        // Right edge
        if ((z_br < breakValue && z_tr >= breakValue) || (z_br >= breakValue && z_tr < breakValue))
        {
            var t = (breakValue - z_br) / (z_tr - z_br);
            var y = bottomRight.Y + t * (topRight.Y - bottomRight.Y);
            intersections.Add(new Coordinate(bottomRight.X, y));
        }

        // Top edge
        if ((z_tr < breakValue && z_tl >= breakValue) || (z_tr >= breakValue && z_tl < breakValue))
        {
            var t = (breakValue - z_tr) / (z_tl - z_tr);
            var x = topRight.X + t * (topLeft.X - topRight.X);
            intersections.Add(new Coordinate(x, topRight.Y));
        }

        // Left edge
        if ((z_tl < breakValue && z_bl >= breakValue) || (z_tl >= breakValue && z_bl < breakValue))
        {
            var t = (breakValue - z_tl) / (z_bl - z_tl);
            var y = topLeft.Y + t * (bottomLeft.Y - topLeft.Y);
            intersections.Add(new Coordinate(topLeft.X, y));
        }

        return intersections;
    }

    /// <summary>
    /// Constructs an isoband polygon within a grid cell
    /// </summary>
    private static Polygon? ConstructIsobandPolygon(
        (Coordinate, Coordinate, Coordinate, Coordinate) cell,
        List<Coordinate> lowerIntersections,
        List<Coordinate> upperIntersections,
        double lowerBreak,
        double upperBreak,
        Dictionary<(double X, double Y), double> zValues
    )
    {
        // Extract corners
        var (bottomLeft, bottomRight, topRight, topLeft) = cell;
        var corners = new[] { bottomLeft, bottomRight, topRight, topLeft };

        // Create a list of coordinates for the polygon boundary
        var polygonCoords = new List<Coordinate>();

        // First add corners that are within the range
        var cornerValues = new[]
        {
            zValues[(bottomLeft.X, bottomLeft.Y)],
            zValues[(bottomRight.X, bottomRight.Y)],
            zValues[(topRight.X, topRight.Y)],
            zValues[(topLeft.X, topLeft.Y)],
        };

        // Define the starting point: either a corner within range or an intersection
        int startCornerIndex = -1;
        for (int i = 0; i < 4; i++)
        {
            if (cornerValues[i] >= lowerBreak && cornerValues[i] < upperBreak)
            {
                startCornerIndex = i;
                break;
            }
        }

        // If no corner is within range, use an intersection point
        if (startCornerIndex == -1)
        {
            if (lowerIntersections.Count > 0)
            {
                polygonCoords.Add(lowerIntersections[0]);
            }
            else if (upperIntersections.Count > 0)
            {
                polygonCoords.Add(upperIntersections[0]);
            }
            else
            {
                // No valid starting point found
                return null;
            }
        }
        else
        {
            // Start with the corner within range
            polygonCoords.Add(corners[startCornerIndex]);
        }

        // Walk around the cell boundary adding intersection points and corners within range
        for (int edge = 0; edge < 4; edge++)
        {
            int i1 = edge;
            int i2 = (edge + 1) % 4;

            // Check for intersections on this edge
            foreach (var intersection in lowerIntersections)
            {
                if (IsPointOnEdge(intersection, corners[i1], corners[i2]))
                {
                    if (
                        !TerritoryUtils.ArePointsClose(
                            intersection,
                            polygonCoords[polygonCoords.Count - 1]
                        )
                    )
                    {
                        polygonCoords.Add(intersection);
                    }
                }
            }

            foreach (var intersection in upperIntersections)
            {
                if (IsPointOnEdge(intersection, corners[i1], corners[i2]))
                {
                    if (
                        !TerritoryUtils.ArePointsClose(
                            intersection,
                            polygonCoords[polygonCoords.Count - 1]
                        )
                    )
                    {
                        polygonCoords.Add(intersection);
                    }
                }
            }

            // Add the end corner if it's within range
            if (cornerValues[i2] >= lowerBreak && cornerValues[i2] < upperBreak)
            {
                if (
                    !TerritoryUtils.ArePointsClose(
                        corners[i2],
                        polygonCoords[polygonCoords.Count - 1]
                    )
                )
                {
                    polygonCoords.Add(corners[i2]);
                }
            }
        }

        // Close the polygon
        if (
            !TerritoryUtils.ArePointsClose(polygonCoords[0], polygonCoords[polygonCoords.Count - 1])
        )
        {
            polygonCoords.Add(polygonCoords[0]);
        }

        // Need at least 4 points to create a valid polygon (first/last are the same)
        if (polygonCoords.Count < 4)
        {
            return null;
        }

        try
        {
            return new Polygon(new LinearRing(polygonCoords.ToArray()));
        }
        catch
        {
            // Invalid polygon geometry
            return null;
        }
    }

    /// <summary>
    /// Checks if a point lies on a line segment
    /// </summary>
    private static bool IsPointOnEdge(
        Coordinate point,
        Coordinate lineStart,
        Coordinate lineEnd,
        double tolerance = 1e-8
    )
    {
        // Check if point is closer to the segment than the tolerance
        var dx = lineEnd.X - lineStart.X;
        var dy = lineEnd.Y - lineStart.Y;
        var len2 = dx * dx + dy * dy;

        if (len2 < tolerance * tolerance)
        {
            // Line segment is very short, just check distance to start point
            var pointDistX = point.X - lineStart.X;
            var pointDistY = point.Y - lineStart.Y;
            return (pointDistX * pointDistX + pointDistY * pointDistY) < tolerance * tolerance;
        }

        // Calculate the projection of the point onto the line
        var t = ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / len2;

        // If t is outside [0,1], the point projects outside the segment
        if (t < 0 || t > 1)
        {
            return false;
        }

        // Calculate the projected point on the line
        var projX = lineStart.X + t * dx;
        var projY = lineStart.Y + t * dy;

        // Check distance from point to projection
        var pointToProjX = point.X - projX;
        var pointToProjY = point.Y - projY;

        return (pointToProjX * pointToProjX + pointToProjY * pointToProjY) < tolerance * tolerance;
    }

    /// <summary>
    /// Merges adjacent polygons into larger polygons
    /// </summary>
    private static List<Polygon> MergeAdjacentPolygons(List<Polygon> polygons)
    {
        if (polygons.Count <= 1)
        {
            return polygons;
        }

        // Use the union operation to merge polygons
        try
        {
            // Start with the first polygon
            Geometry result = polygons[0];

            // Union with each subsequent polygon
            for (int i = 1; i < polygons.Count; i++)
            {
                result = result.Union(polygons[i]);
            }

            // Convert the result back to a list of polygons
            if (result is Polygon polygon)
            {
                return new List<Polygon> { polygon };
            }
            else if (result is MultiPolygon multiPolygon)
            {
                var resultPolygons = new List<Polygon>();
                for (int i = 0; i < multiPolygon.NumGeometries; i++)
                {
                    resultPolygons.Add((Polygon)multiPolygon.GetGeometryN(i));
                }
                return resultPolygons;
            }

            // Fallback to original polygons if something went wrong
            return polygons;
        }
        catch
        {
            // If the union operation fails, return the original polygons
            return polygons;
        }
    }
}

/// <summary>
/// Options for isoband generation
/// </summary>
public readonly record struct IsobandsOptions
{
    public static readonly IsobandsOptions Empty = new();
    public static readonly IsobandsOptions Default = new() { ZProperty = null };

    /// <summary>
    /// Name of the property containing z-values in the points (default: use z-coordinate)
    /// </summary>
    public string? ZProperty { get; init; }

    public static IsobandsOptions OrDefault(IsobandsOptions options) =>
        options == Empty ? Default : options;
}
