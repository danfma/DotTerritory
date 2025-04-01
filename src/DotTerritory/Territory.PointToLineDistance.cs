namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Returns the minimum distance between a point and a line segment.
    /// </summary>
    /// <param name="point">The point</param>
    /// <param name="line">The line</param>
    /// <param name="units">The units of the returned distance</param>
    /// <param name="options">Additional options</param>
    /// <returns>The minimum distance between the point and the line</returns>
    /// <example>
    /// <code>
    /// var pt = new Point(new Coordinate(0, 0));
    /// var line = new LineString(new[] {
    ///     new Coordinate(1, 1),
    ///     new Coordinate(1, 2),
    ///     new Coordinate(1, 3),
    ///     new Coordinate(1, 4)
    /// });
    /// var distance = Territory.PointToLineDistance(pt, line, LengthUnit.Kilometer);
    /// </code>
    /// </example>
    public static Length PointToLineDistance(
        Point point,
        LineString line,
        LengthUnit units = LengthUnit.Meter,
        Func<PointToLineDistanceOptions, PointToLineDistanceOptions>? configure = null
    )
    {
        var options =
            configure?.Invoke(PointToLineDistanceOptions.Empty) ?? PointToLineDistanceOptions.Empty;

        // If it's a point-to-point comparison (single vertex line)
        if (line.NumPoints <= 1)
        {
            if (line.NumPoints == 1)
            {
                return Distance(point.Coordinate, line.GetCoordinateN(0)).ToUnit(units);
            }

            return Length.Zero;
        }

        var minDistance = double.MaxValue;

        // For each segment in the line, calculate the distance to the point
        for (int i = 0; i < line.NumPoints - 1; i++)
        {
            var start = line.GetCoordinateN(i);
            var end = line.GetCoordinateN(i + 1);

            var distance = PointToSegmentDistance(point.Coordinate, start, end, units);

            if (distance.Value < minDistance)
            {
                minDistance = distance.Value;
            }
        }

        return Length.From(minDistance, units);
    }

    /// <summary>
    /// Returns the minimum distance between a point and a line segment.
    /// </summary>
    /// <param name="point">The point coordinates</param>
    /// <param name="line">The line</param>
    /// <param name="units">The units of the returned distance</param>
    /// <param name="options">Additional options</param>
    /// <returns>The minimum distance between the point and the line</returns>
    public static Length PointToLineDistance(
        Coordinate point,
        LineString line,
        LengthUnit units = LengthUnit.Meter,
        Func<PointToLineDistanceOptions, PointToLineDistanceOptions>? configure = null
    )
    {
        return PointToLineDistance(new Point(point), line, units, configure);
    }

    /// <summary>
    /// Calculates the minimum distance between a point and a line segment.
    /// </summary>
    /// <param name="point">The point</param>
    /// <param name="segmentStart">Start point of the line segment</param>
    /// <param name="segmentEnd">End point of the line segment</param>
    /// <param name="units">The units of the returned distance</param>
    /// <returns>The minimum distance between the point and the segment</returns>
    public static Length PointToSegmentDistance(
        Coordinate point,
        Coordinate segmentStart,
        Coordinate segmentEnd,
        LengthUnit units = LengthUnit.Meter
    )
    {
        // If start and end are the same point, just return the distance to that point
        if (segmentStart.Equals2D(segmentEnd))
        {
            return Distance(point, segmentStart).ToUnit(units);
        }

        // Calculate the closest point on the segment to the given point
        // Based on the projection of the point vector onto the segment vector

        // Create vectors
        var p = new double[] { point.X, point.Y };
        var s1 = new double[] { segmentStart.X, segmentStart.Y };
        var s2 = new double[] { segmentEnd.X, segmentEnd.Y };

        // Calculate the squared length of the segment (avoid square root for performance)
        var segmentLengthSquared = DistanceSquared(s1, s2);

        // If segment length is essentially zero, return distance to start point
        if (segmentLengthSquared < 1e-10)
        {
            return Distance(point, segmentStart).ToUnit(units);
        }

        // Calculate projection parameter (t)
        var t =
            ((p[0] - s1[0]) * (s2[0] - s1[0]) + (p[1] - s1[1]) * (s2[1] - s1[1]))
            / segmentLengthSquared;

        // Clamp t to [0, 1] to ensure the projected point is on the segment
        t = Math.Max(0, Math.Min(1, t));

        // Calculate the closest point on the segment
        var closestPoint = new Coordinate(s1[0] + t * (s2[0] - s1[0]), s1[1] + t * (s2[1] - s1[1]));

        // Return the distance to the closest point
        return Distance(point, closestPoint).ToUnit(units);
    }

    // Helper method to calculate squared distance between two points given as arrays
    private static double DistanceSquared(double[] p1, double[] p2)
    {
        var dx = p2[0] - p1[0];
        var dy = p2[1] - p1[1];
        return dx * dx + dy * dy;
    }

    /// <summary>
    /// Options for PointToLineDistance
    /// </summary>
    /// <param name="SegmentOnly">Whether to calculate the distance to line segments only,
    /// or consider the extended lines (default: true = segments only)</param>
    public record struct PointToLineDistanceOptions(bool SegmentOnly = true)
    {
        public static readonly PointToLineDistanceOptions Empty = new();
    }
}
