using NetTopologySuite.Geometries;
using UnitsNet;
using UnitsNet.Units;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Returns true if each segment of the first LineString is parallel to the corresponding segment of the second LineString.
    /// </summary>
    /// <param name="line1">First LineString</param>
    /// <param name="line2">Second LineString</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>True if the LineStrings are parallel, false otherwise</returns>
    /// <example>
    /// <code>
    /// var line1 = geometryFactory.CreateLineString(new[] {
    ///     new Coordinate(0, 0),
    ///     new Coordinate(1, 1)
    /// });
    /// var line2 = geometryFactory.CreateLineString(new[] {
    ///     new Coordinate(1, 0),
    ///     new Coordinate(2, 1)
    /// });
    /// var isParallel = Territory.BooleanParallel(line1, line2);
    /// // => true
    /// </code>
    /// </example>
    public static bool BooleanParallel(
        LineString line1,
        LineString line2,
        Func<BooleanParallelOptions, BooleanParallelOptions>? configure = null
    )
    {
        var options =
            configure?.Invoke(BooleanParallelOptions.Empty) ?? BooleanParallelOptions.Empty;

        // Lines with only one point don't have a direction
        if (line1.NumPoints <= 1 || line2.NumPoints <= 1)
            return false;

        // If the lines have different number of segments, compare only corresponding segments
        var minSegments = Math.Min(line1.NumPoints - 1, line2.NumPoints - 1);

        for (int i = 0; i < minSegments; i++)
        {
            var start1 = line1.GetCoordinateN(i);
            var end1 = line1.GetCoordinateN(i + 1);
            var start2 = line2.GetCoordinateN(i);
            var end2 = line2.GetCoordinateN(i + 1);

            // Skip zero-length segments
            if (start1.Equals2D(end1) || start2.Equals2D(end2))
                continue;

            // Calculate bearings of both segments
            var bearing1 = CalcBearing(start1, end1);
            var bearing2 = CalcBearing(start2, end2);

            // Calculate absolute angle difference
            var angleDiff = Math.Abs(bearing1 - bearing2);

            // Normalize angle difference to 0-180 range
            while (angleDiff > 180)
                angleDiff = Math.Abs(angleDiff - 360);

            // Check if segments are parallel within threshold
            // Parallel lines have either the same bearing (0°) or opposite bearing (180°)
            if (angleDiff > options.Threshold && Math.Abs(angleDiff - 180) > options.Threshold)
                return false;
        }

        return true;
    }

    // Helper method to calculate bearing in degrees
    private static double CalcBearing(Coordinate start, Coordinate end)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        var angle = Math.Atan2(dy, dx) * 180 / Math.PI;

        // Normalize to 0-360
        return (angle + 360) % 360;
    }

    /// <summary>
    /// Options for BooleanParallel
    /// </summary>
    /// <param name="Threshold">Maximum angle difference in degrees for lines to be considered parallel (default: 1)</param>
    public record struct BooleanParallelOptions(double Threshold = 1)
    {
        public static readonly BooleanParallelOptions Empty = new();
    }
}
