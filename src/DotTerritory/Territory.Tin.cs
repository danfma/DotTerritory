namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Creates a Triangulated Irregular Network (TIN) from a set of points.
    /// Uses Delaunay triangulation to generate triangles.
    /// </summary>
    /// <param name="points">Array of points to use for triangulation</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>A collection of triangular polygons</returns>
    public static Polygon[] Tin(Point[] points, TinOptions options = default)
    {
        options = TinOptions.OrDefault(options);

        if (points.Length < 3)
        {
            throw new ArgumentException("TIN requires at least 3 points", nameof(points));
        }

        // Extract z-values if available for properties
        var zValues = new Dictionary<Coordinate, double>();
        for (int i = 0; i < points.Length; i++)
        {
            var coord = points[i].Coordinate;
            if (!double.IsNaN(coord.Z))
            {
                zValues[coord] = coord.Z;
            }
        }

        // Perform Delaunay triangulation to create the TIN
        var triangles = DelaunayTriangulation(points);

        // Convert triangulation result to polygons
        var polygons = new Polygon[triangles.Count];
        for (int i = 0; i < triangles.Count; i++)
        {
            var triangle = triangles[i];
            var vertices = new Coordinate[]
            {
                triangle.Item1,
                triangle.Item2,
                triangle.Item3,
                triangle.Item1, // Close the ring
            };

            polygons[i] = new Polygon(new LinearRing(vertices));

            // If the z-property is specified, calculate and set properties for the triangle
            if (!string.IsNullOrEmpty(options.ZProperty))
            {
                var properties = new Dictionary<string, object>();
                double sum = 0;
                int count = 0;

                // Use z-coordinates from the vertices if available
                for (int j = 0; j < 3; j++)
                {
                    var vertex =
                        j == 0 ? triangle.Item1 : (j == 1 ? triangle.Item2 : triangle.Item3);
                    if (zValues.TryGetValue(vertex, out double z))
                    {
                        sum += z;
                        count++;
                    }
                }

                if (count > 0)
                {
                    // Average z-value for the triangle
                    properties[options.ZProperty] = sum / count;

                    // TODO: Attach properties to the polygon
                    // This would require modifying the Polygon class or creating a Feature
                }
            }
        }

        return polygons;
    }

    /// <summary>
    /// Creates a Triangulated Irregular Network (TIN) from a geometry containing points.
    /// </summary>
    /// <param name="pointGeometry">Geometry containing points for triangulation</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>A collection of triangular polygons</returns>
    public static Polygon[] Tin(Geometry pointGeometry, TinOptions options = default)
    {
        var coordinates = pointGeometry.Coordinates;
        var points = new Point[coordinates.Length];

        for (int i = 0; i < coordinates.Length; i++)
        {
            points[i] = new Point(coordinates[i]);
        }

        return Tin(points, options);
    }

    /// <summary>
    /// Performs Delaunay triangulation on a set of points.
    /// This is a simple implementation - for production use, consider using
    /// a dedicated computational geometry library.
    /// </summary>
    private static List<(Coordinate, Coordinate, Coordinate)> DelaunayTriangulation(Point[] points)
    {
        // Simple implementation of Delaunay triangulation
        var triangles = new List<(Coordinate, Coordinate, Coordinate)>();

        // If we have exactly 3 points, return a single triangle
        if (points.Length == 3)
        {
            triangles.Add((points[0].Coordinate, points[1].Coordinate, points[2].Coordinate));
            return triangles;
        }

        // For more than 3 points, implement a proper Delaunay algorithm
        // This is a simplified approach - real implementations would use
        // either divide-and-conquer or incremental algorithms

        // Create a convex hull to start with
        var convexHull = Convex(points);
        var hullPoints = convexHull.Shell.Coordinates;

        // Basic approach: create triangles by connecting each point
        // to its two nearest neighbors, ensuring no edge intersections
        var coordinates = points.Select(p => p.Coordinate).ToList();

        // If we have only hull points, do a simple fan triangulation
        if (coordinates.Count <= hullPoints.Length)
        {
            // For small number of points, just do a fan triangulation from first point
            for (int i = 1; i < coordinates.Count - 1; i++)
            {
                triangles.Add((coordinates[0], coordinates[i], coordinates[i + 1]));
            }
            return triangles;
        }

        // More complex case: use a simple Delaunay criterion by checking
        // if any other point is inside the circumcircle of a potential triangle
        for (int i = 0; i < coordinates.Count; i++)
        {
            for (int j = i + 1; j < coordinates.Count; j++)
            {
                for (int k = j + 1; k < coordinates.Count; k++)
                {
                    var a = coordinates[i];
                    var b = coordinates[j];
                    var c = coordinates[k];

                    // Check if these three points form a valid triangle
                    if (IsValidTriangle(a, b, c, coordinates))
                    {
                        triangles.Add((a, b, c));
                    }
                }
            }
        }

        // If no triangles were created (which shouldn't happen with a proper implementation),
        // fall back to a simple triangulation
        if (triangles.Count == 0)
        {
            // Simple fan triangulation from the centroid
            var centroid = CalculateTinCentroid(coordinates);

            for (int i = 0; i < hullPoints.Length - 1; i++)
            {
                triangles.Add((centroid, hullPoints[i], hullPoints[i + 1]));
            }
        }

        return triangles;
    }

    /// <summary>
    /// Checks if a triangle is valid for Delaunay triangulation.
    /// </summary>
    private static bool IsValidTriangle(
        Coordinate a,
        Coordinate b,
        Coordinate c,
        List<Coordinate> allPoints
    )
    {
        // Calculate the circumcircle of the triangle
        var circumcircle = CalculateCircumcircle(a, b, c);
        if (circumcircle == null)
        {
            return false; // Points are collinear
        }

        var center = circumcircle.Value.center;
        var radius = circumcircle.Value.radius;

        // Check if any other point is inside the circumcircle
        foreach (var point in allPoints)
        {
            if (point.Equals2D(a) || point.Equals2D(b) || point.Equals2D(c))
            {
                continue; // Skip the triangle vertices
            }

            // Calculate distance from point to center
            var dx = point.X - center.X;
            var dy = point.Y - center.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy);

            // If a point is inside the circumcircle, this is not a Delaunay triangle
            if (distance < radius)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Calculates the circumcircle of a triangle.
    /// </summary>
    private static (Coordinate center, double radius)? CalculateCircumcircle(
        Coordinate a,
        Coordinate b,
        Coordinate c
    )
    {
        // First check if points are collinear
        var area = a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y);
        if (Math.Abs(area) < double.Epsilon)
        {
            return null; // Points are collinear
        }

        // Calculate the perpendicular bisectors of sides AB and BC
        var abMidpoint = new Coordinate((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        var bcMidpoint = new Coordinate((b.X + c.X) / 2, (b.Y + c.Y) / 2);

        // Calculate perpendicular direction vectors
        var abDx = b.X - a.X;
        var abDy = b.Y - a.Y;
        var abPerpX = -abDy;
        var abPerpY = abDx;

        var bcDx = c.X - b.X;
        var bcDy = c.Y - b.Y;
        var bcPerpX = -bcDy;
        var bcPerpY = bcDx;

        // Calculate the intersection of the perpendicular bisectors
        // to find the circumcenter
        var t =
            ((bcMidpoint.X - abMidpoint.X) * bcPerpY - (bcMidpoint.Y - abMidpoint.Y) * bcPerpX)
            / (abPerpX * bcPerpY - abPerpY * bcPerpX);

        var center = new Coordinate(abMidpoint.X + t * abPerpX, abMidpoint.Y + t * abPerpY);

        // Calculate the radius (distance from center to any vertex)
        var dx = center.X - a.X;
        var dy = center.Y - a.Y;
        var radius = Math.Sqrt(dx * dx + dy * dy);

        return (center, radius);
    }

    /// <summary>
    /// Calculates the centroid of a set of coordinates for TIN generation
    /// </summary>
    private static Coordinate CalculateTinCentroid(List<Coordinate> coords)
    {
        var sumX = 0.0;
        var sumY = 0.0;

        foreach (var coord in coords)
        {
            sumX += coord.X;
            sumY += coord.Y;
        }

        return new Coordinate(sumX / coords.Count, sumY / coords.Count);
    }
}

/// <summary>
/// Options for TIN generation
/// </summary>
public readonly record struct TinOptions
{
    public static readonly TinOptions Empty = new();
    public static readonly TinOptions Default = new() { ZProperty = null };

    /// <summary>
    /// Name of the property to use for z-values (optional)
    /// </summary>
    public string? ZProperty { get; init; }

    public static TinOptions OrDefault(TinOptions options) => options == Empty ? Default : options;
}
