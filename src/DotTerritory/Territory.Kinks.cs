// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.Kinks.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Finds self-intersections (kinks) in a LineString or Polygon
    /// </summary>
    /// <param name="geometry">Input geometry</param>
    /// <returns>An array of Point objects representing self-intersections</returns>
    public static Point[] Kinks(Geometry geometry)
    {
        if (geometry is LineString lineString)
        {
            return FindKinksInLineString(lineString);
        }
        else if (geometry is Polygon polygon)
        {
            var kinks = new List<Point>();

            // Check the exterior ring
            var exteriorKinks = FindKinksInLinearRing(polygon.ExteriorRing);
            kinks.AddRange(exteriorKinks);

            // Check interior rings (holes)
            for (int i = 0; i < polygon.NumInteriorRings; i++)
            {
                var interiorKinks = FindKinksInLinearRing(polygon.GetInteriorRingN(i));
                kinks.AddRange(interiorKinks);
            }

            return kinks.ToArray();
        }
        else
        {
            throw new ArgumentException(
                "Geometry must be a LineString or Polygon",
                nameof(geometry)
            );
        }
    }

    private static Point[] FindKinksInLineString(LineString line)
    {
        var coords = line.Coordinates;
        var kinks = new List<Point>();

        // Need at least 4 coordinates to have self-intersections
        if (coords.Length < 4)
            return Array.Empty<Point>();

        // Check each segment against every other non-adjacent segment
        for (int i = 0; i < coords.Length - 1; i++)
        {
            var segment1Start = coords[i];
            var segment1End = coords[i + 1];

            // Skip segments that are effectively points (zero length)
            if (segment1Start.Equals2D(segment1End))
                continue;

            for (int j = i + 2; j < coords.Length - 1; j++)
            {
                // Skip adjacent segments (they share a vertex, so no real intersection)
                if (j == i + 1)
                    continue;

                var segment2Start = coords[j];
                var segment2End = coords[j + 1];

                // Skip segments that are effectively points
                if (segment2Start.Equals2D(segment2End))
                    continue;

                // Skip if the last point of segment2 is the first point of segment1
                // (circular paths would have segment n connecting to segment 0)
                if (j == coords.Length - 2 && i == 0 && segment2End.Equals2D(segment1Start))
                    continue;

                // Check if the segments intersect
                var intersection = SegmentIntersection(
                    segment1Start,
                    segment1End,
                    segment2Start,
                    segment2End
                );

                if (intersection != null)
                {
                    kinks.Add(new Point(intersection));
                }
            }
        }

        return kinks.ToArray();
    }

    private static Point[] FindKinksInLinearRing(LineString ring)
    {
        // For a linear ring, the first and last coordinates are the same
        // So we create a temporary linestring excluding the last point to avoid double counting
        var coords = new Coordinate[ring.NumPoints - 1];
        for (int i = 0; i < coords.Length; i++)
        {
            coords[i] = ring.GetCoordinateN(i);
        }

        var tempLine = new LineString(coords);
        return FindKinksInLineString(tempLine);
    }

    private static Coordinate? SegmentIntersection(
        Coordinate a1,
        Coordinate a2,
        Coordinate b1,
        Coordinate b2
    )
    {
        // Line segment geometry algorithm to find intersection point
        double ua_t = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
        double ub_t = (a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X);
        double u_b = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);

        // If u_b equals 0, the lines are coincident (overlapping)
        // In that case, we don't consider it a kink
        if (Math.Abs(u_b) < double.Epsilon)
            return null;

        ua_t = ua_t / u_b;
        ub_t = ub_t / u_b;

        // Check if intersection occurs within both line segments
        if (ua_t >= 0 && ua_t <= 1 && ub_t >= 0 && ub_t <= 1)
        {
            var x = a1.X + ua_t * (a2.X - a1.X);
            var y = a1.Y + ua_t * (a2.Y - a1.Y);

            // Only consider proper intersections (not at endpoints)
            var isEndpoint =
                (Math.Abs(x - a1.X) < double.Epsilon && Math.Abs(y - a1.Y) < double.Epsilon)
                || (Math.Abs(x - a2.X) < double.Epsilon && Math.Abs(y - a2.Y) < double.Epsilon)
                || (Math.Abs(x - b1.X) < double.Epsilon && Math.Abs(y - b1.Y) < double.Epsilon)
                || (Math.Abs(x - b2.X) < double.Epsilon && Math.Abs(y - b2.Y) < double.Epsilon);

            if (!isEndpoint)
                return new Coordinate(x, y);
        }

        return null;
    }
}
