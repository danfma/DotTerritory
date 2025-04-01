namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Generates Voronoi diagram based on the given points and bounding box.
    /// </summary>
    /// <param name="points">Array of points for which to generate Voronoi cells</param>
    /// <param name="options">Optional parameters including bounding box</param>
    /// <returns>Voronoi diagram as a FeatureCollection of Polygon features</returns>
    internal static FeatureCollection Voronoi(Point[] points, VoronoiOptions options = default)
    {
        options = VoronoiOptions.OrDefault(options);

        // Ensure we have a valid bounding box
        var bbox = options.BoundingBox;
        if (bbox == null)
        {
            // Calculate the bounding box of the points if not provided
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var point in points)
            {
                var coord = point.Coordinate;

                minX = Math.Min(minX, coord.X);
                minY = Math.Min(minY, coord.Y);
                maxX = Math.Max(maxX, coord.X);
                maxY = Math.Max(maxY, coord.Y);
            }

            // Add a buffer to the bounding box
            var bufferX = (maxX - minX) * 0.1;
            var bufferY = (maxY - minY) * 0.1;

            bbox = new BBox(
                West: minX - bufferX,
                South: minY - bufferY,
                East: maxX + bufferX,
                North: maxY + bufferY
            );
        }

        // Compute the Voronoi diagram
        var voronoiPolygons = ComputeVoronoiPolygons(points, bbox.Value);

        // Create a feature collection from the Voronoi polygons
        var features = new List<Feature>();
        for (int i = 0; i < voronoiPolygons.Length; i++)
        {
            var polygon = voronoiPolygons[i];
            var attributes = new AttributesTable();
            
            // Add properties to indicate the corresponding input point
            if (i < points.Length)
            {
                attributes.Add("point", points[i]);
            }
            
            var feature = new Feature(polygon, attributes);

            features.Add(feature);
        }

        return new FeatureCollection(features);
    }

    /// <summary>
    /// Generates Voronoi diagram based on the given geometry's coordinates and bounding box.
    /// </summary>
    /// <param name="geom">Geometry containing points for which to generate Voronoi cells</param>
    /// <param name="options">Optional parameters including bounding box</param>
    /// <returns>Voronoi diagram as a FeatureCollection of Polygon features</returns>
    internal static FeatureCollection Voronoi(Geometry geom, VoronoiOptions options = default)
    {
        // Extract points from the geometry
        var coordinates = geom.Coordinates;
        var points = new Point[coordinates.Length];

        for (int i = 0; i < coordinates.Length; i++)
        {
            points[i] = new Point(coordinates[i]);
        }

        return Voronoi(points, options);
    }

    /// <summary>
    /// Computes Voronoi polygons for the given array of points.
    /// This is a simplified implementation based on Fortune's algorithm.
    /// </summary>
    private static Polygon[] ComputeVoronoiPolygons(Point[] points, BBox bbox)
    {
        // For simplicity, this implementation creates approximated Voronoi cells
        // A full implementation would use Fortune's algorithm or a computational geometry library

        // Create a cell for each point
        var polygons = new Polygon[points.Length];

        // Process each point
        for (int i = 0; i < points.Length; i++)
        {
            // Create the Voronoi cell for this point by finding the region
            // that's closer to this point than to any other point
            var cell = GenerateVoronoiCellForPoint(points[i], points, bbox);
            polygons[i] = cell;
        }

        return polygons;
    }

    /// <summary>
    /// Generates a Voronoi cell for a single point by approximating its region.
    /// </summary>
    private static Polygon GenerateVoronoiCellForPoint(Point center, Point[] allPoints, BBox bbox)
    {
        // Simple implementation: create a grid around the point and define the cell
        // as the region containing points closer to this center than any other point

        // For now, we'll use a simpler approach to calculate the cell
        // Create perpendicular bisectors between this point and its neighbors
        var neighbors = FindNeighboringPoints(center, allPoints);

        // Create a large initial cell based on the bounding box
        var initialCellCoords = new Coordinate[]
        {
            new Coordinate(bbox.West, bbox.South),
            new Coordinate(bbox.East, bbox.South),
            new Coordinate(bbox.East, bbox.North),
            new Coordinate(bbox.West, bbox.North),
            new Coordinate(bbox.West, bbox.South), // Close the ring
        };

        var cell = new Polygon(new LinearRing(initialCellCoords));

        // For each neighboring point, clip the cell with the perpendicular bisector
        foreach (var neighbor in neighbors)
        {
            // Calculate midpoint between this point and its neighbor
            var midpoint = new Coordinate((center.X + neighbor.X) / 2, (center.Y + neighbor.Y) / 2);

            // Calculate the perpendicular direction
            var dx = neighbor.X - center.X;
            var dy = neighbor.Y - center.Y;

            // Perpendicular direction (rotate 90 degrees)
            var perpX = -dy;
            var perpY = dx;

            // Normalize the perpendicular vector
            var length = Math.Sqrt(perpX * perpX + perpY * perpY);
            if (length > 0)
            {
                perpX /= length;
                perpY /= length;
            }

            // Calculate width and height of bounding box
            var width = bbox.East - bbox.West;
            var height = bbox.North - bbox.South;

            // Create a line perpendicular to the segment between points
            // and passing through the midpoint
            var bisectorPoint1 = new Coordinate(
                midpoint.X + perpX * width * 2,
                midpoint.Y + perpY * height * 2
            );

            var bisectorPoint2 = new Coordinate(
                midpoint.X - perpX * width * 2,
                midpoint.Y - perpY * height * 2
            );

            // Create a half-plane polygon
            var halfPlanePoly = CreateHalfPlanePolygon(
                midpoint,
                bisectorPoint1,
                bisectorPoint2,
                center,
                bbox
            );

            // Intersect the current cell with the half-plane
            cell = (Polygon)cell.Intersection(halfPlanePoly);
        }

        return cell;
    }

    /// <summary>
    /// Finds neighboring points for Voronoi cell construction
    /// </summary>
    private static List<Coordinate> FindNeighboringPoints(Point center, Point[] allPoints)
    {
        var result = new List<Coordinate>();

        foreach (var point in allPoints)
        {
            if (point != center)
            {
                result.Add(point.Coordinate);
            }
        }

        return result;
    }

    /// <summary>
    /// Creates a half-plane polygon determined by a bisector line
    /// </summary>
    private static Polygon CreateHalfPlanePolygon(
        Coordinate midpoint,
        Coordinate bisectorPoint1,
        Coordinate bisectorPoint2,
        Point center,
        BBox bbox
    )
    {
        // Ensure the half-plane contains the center point
        var v1x = bisectorPoint1.X - midpoint.X;
        var v1y = bisectorPoint1.Y - midpoint.Y;
        var v2x = center.X - midpoint.X;
        var v2y = center.Y - midpoint.Y;

        // Check if the vectors are on the same side (dot product > 0)
        var dotProduct = v1x * v2x + v1y * v2y;

        // If not, swap the bisector points to ensure the half-plane contains the center
        if (dotProduct < 0)
        {
            var temp = bisectorPoint1;
            bisectorPoint1 = bisectorPoint2;
            bisectorPoint2 = temp;
        }

        // Create a polygon representing the half-plane by extending the bisector
        // to the bounds of the bbox
        var bounds = new Coordinate[]
        {
            new Coordinate(bbox.West, bbox.South),
            new Coordinate(bbox.East, bbox.South),
            new Coordinate(bbox.East, bbox.North),
            new Coordinate(bbox.West, bbox.North),
        };

        // Find the intersection points of the bisector with the bounding box
        var intersections = new List<Coordinate>();
        for (int i = 0; i < 4; i++)
        {
            var p1 = bounds[i];
            var p2 = bounds[(i + 1) % 4];

            var intersection = LineSegmentIntersection(midpoint, bisectorPoint1, p1, p2);
            if (intersection != null)
            {
                intersections.Add(intersection);
            }

            intersection = LineSegmentIntersection(midpoint, bisectorPoint2, p1, p2);
            if (intersection != null)
            {
                intersections.Add(intersection);
            }
        }

        // If we found exactly two intersection points, use them to create the half-plane
        if (intersections.Count >= 2)
        {
            var halfPlaneCoords = new List<Coordinate>();
            halfPlaneCoords.Add(intersections[0]);
            halfPlaneCoords.Add(intersections[1]);

            // Add the bounding box corners that are on the correct side of the bisector
            foreach (var corner in bounds)
            {
                // Check if the corner is on the same side as the center point
                var vCornerX = corner.X - midpoint.X;
                var vCornerY = corner.Y - midpoint.Y;

                var cornerDotProduct = v1x * vCornerX + v1y * vCornerY;

                if (cornerDotProduct >= 0)
                {
                    halfPlaneCoords.Add(corner);
                }
            }

            // Add the first point again to close the ring
            halfPlaneCoords.Add(halfPlaneCoords[0]);

            // Create the polygon
            return new Polygon(new LinearRing(halfPlaneCoords.ToArray()));
        }

        // Fallback: return the whole bounding box
        var fallbackCoords = new Coordinate[]
        {
            bounds[0],
            bounds[1],
            bounds[2],
            bounds[3],
            bounds[0], // Close the ring
        };

        return new Polygon(new LinearRing(fallbackCoords));
    }

    /// <summary>
    /// Calculates the intersection point of two line segments
    /// </summary>
    private static Coordinate? LineSegmentIntersection(
        Coordinate a1,
        Coordinate a2,
        Coordinate b1,
        Coordinate b2
    )
    {
        // Line segment intersection calculation
        double ua_t = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
        double ub_t = (a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X);
        double u_b = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);

        // If u_b equals 0, the lines are coincident or parallel
        if (Math.Abs(u_b) < double.Epsilon)
            return null;

        ua_t = ua_t / u_b;
        ub_t = ub_t / u_b;

        // Check if the intersection occurs within both line segments
        if (ua_t >= 0 && ua_t <= 1 && ub_t >= 0 && ub_t <= 1)
        {
            var x = a1.X + ua_t * (a2.X - a1.X);
            var y = a1.Y + ua_t * (a2.Y - a1.Y);

            return new Coordinate(x, y);
        }

        return null;
    }
}

/// <summary>
/// Options for Voronoi diagram generation
/// </summary>
public readonly record struct VoronoiOptions
{
    public static readonly VoronoiOptions Empty = new();
    public static readonly VoronoiOptions Default = new() { BoundingBox = null };

    /// <summary>
    /// Clipping bounding box for the Voronoi diagram
    /// </summary>
    public BBox? BoundingBox { get; init; }

    public static VoronoiOptions OrDefault(VoronoiOptions options) =>
        options == Empty ? Default : options;
}
