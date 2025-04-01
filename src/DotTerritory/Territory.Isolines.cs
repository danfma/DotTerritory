namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a grid of points with z-values and returns isolines (contour lines)
    /// at the specified break values.
    /// </summary>
    /// <param name="pointGrid">Array of points forming a grid, with z-values</param>
    /// <param name="breaks">Array of values representing the boundaries of each band</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>A FeatureCollection of LineString features representing isolines</returns>
    internal static FeatureCollection Isolines(
        Point[] pointGrid,
        double[] breaks,
        IsolinesOptions options = default
    )
    {
        options = IsolinesOptions.OrDefault(options);
        if (pointGrid.Length < 4)
        {
            throw new ArgumentException(
                "Point grid must contain at least 4 points",
                nameof(pointGrid)
            );
        }
        if (breaks.Length < 1)
        {
            throw new ArgumentException(
                "At least one break value must be provided",
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

        // For each break value, generate an isoline
        var features = new List<Feature>();

        foreach (var breakValue in breaks)
        {
            // Find all cell intersections with this value
            var segments = new List<(Coordinate, Coordinate)>();

            foreach (var cell in grid)
            {
                var cellSegments = FindIsolineSegmentsInCell(cell, breakValue, zValues);
                segments.AddRange(cellSegments);
            }

            // Connect segments into lines
            var lines = ConnectSegmentsIntoLines(segments);

            // Create a feature for each line
            foreach (var line in lines)
            {
                var attributes = new AttributesTable();
                attributes.Add("isoline", breakValue);
                
                var feature = new Feature(line, attributes);

                features.Add(feature);
            }
        }

        return new FeatureCollection(features);
    }

    /// <summary>
    /// Finds isoline segments within a grid cell
    /// </summary>
    private static List<(Coordinate, Coordinate)> FindIsolineSegmentsInCell(
        (Coordinate, Coordinate, Coordinate, Coordinate) cell,
        double breakValue,
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

        // Find the intersections of the isoline with the cell edges
        var intersections = new List<Coordinate>();

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

        // An isoline crossing a cell should have exactly 2 intersections
        // (except for corner cases like tangent points)
        if (intersections.Count >= 2)
        {
            var segments = new List<(Coordinate, Coordinate)>();

            // Create segments by pairing intersections
            // In most cases, there will be just one pair
            for (int i = 0; i < intersections.Count - 1; i += 2)
            {
                if (i + 1 < intersections.Count)
                {
                    segments.Add((intersections[i], intersections[i + 1]));
                }
            }

            return segments;
        }

        return new List<(Coordinate, Coordinate)>();
    }

    /// <summary>
    /// Connects segments into continuous lines
    /// </summary>
    private static List<LineString> ConnectSegmentsIntoLines(
        List<(Coordinate, Coordinate)> segments
    )
    {
        if (segments.Count == 0)
        {
            return new List<LineString>();
        }

        // Create a copy of segments that we can modify
        var remainingSegments = new List<(Coordinate, Coordinate)>(segments);
        var lines = new List<LineString>();

        while (remainingSegments.Count > 0)
        {
            // Start a new line with the first segment
            var currentSegment = remainingSegments[0];
            remainingSegments.RemoveAt(0);

            var lineCoords = new List<Coordinate> { currentSegment.Item1, currentSegment.Item2 };

            // Try to find connected segments
            bool foundConnection = true;

            while (foundConnection && remainingSegments.Count > 0)
            {
                foundConnection = false;
                var endPoint = lineCoords[lineCoords.Count - 1];

                // Look for a segment that connects to the current end point
                for (int i = 0; i < remainingSegments.Count; i++)
                {
                    var (start, end) = remainingSegments[i];

                    if (TerritoryUtils.ArePointsClose(start, endPoint))
                    {
                        // Found a segment that starts where the current line ends
                        lineCoords.Add(end);
                        remainingSegments.RemoveAt(i);
                        foundConnection = true;
                        break;
                    }
                    else if (TerritoryUtils.ArePointsClose(end, endPoint))
                    {
                        // Found a segment that ends where the current line ends
                        // Need to reverse the segment
                        lineCoords.Add(start);
                        remainingSegments.RemoveAt(i);
                        foundConnection = true;
                        break;
                    }
                }

                // If no connection was found at the end, try at the beginning
                if (!foundConnection && lineCoords.Count >= 2)
                {
                    var startPoint = lineCoords[0];

                    for (int i = 0; i < remainingSegments.Count; i++)
                    {
                        var (start, end) = remainingSegments[i];

                        if (TerritoryUtils.ArePointsClose(end, startPoint))
                        {
                            // Found a segment that ends where the current line starts
                            lineCoords.Insert(0, start);
                            remainingSegments.RemoveAt(i);
                            foundConnection = true;
                            break;
                        }
                        else if (TerritoryUtils.ArePointsClose(start, startPoint))
                        {
                            // Found a segment that starts where the current line starts
                            // Need to reverse the segment
                            lineCoords.Insert(0, end);
                            remainingSegments.RemoveAt(i);
                            foundConnection = true;
                            break;
                        }
                    }
                }
            }

            // Create a LineString from the coordinates
            if (lineCoords.Count >= 2)
            {
                lines.Add(new LineString(lineCoords.ToArray()));
            }
        }

        return lines;
    }
}

/// <summary>
/// Options for isoline generation
/// </summary>
public readonly record struct IsolinesOptions
{
    public static readonly IsolinesOptions Empty = new();
    public static readonly IsolinesOptions Default = new() { ZProperty = null };

    /// <summary>
    /// Name of the property containing z-values in the points (default: use z-coordinate)
    /// </summary>
    public string? ZProperty { get; init; }

    public static IsolinesOptions OrDefault(IsolinesOptions options) =>
        options == Empty ? Default : options;
}
