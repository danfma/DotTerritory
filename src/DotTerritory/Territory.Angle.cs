namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Finds the angle formed by two adjacent segments defined by 3 points. The result will be the (positive clockwise)
    /// angle with origin on the `startPoint-midPoint` segment, or its explementary angle if required.
    /// </summary>
    /// <param name="startPoint">start coordinate</param>
    /// <param name="midPoint">mid coordinate</param>
    /// <param name="endPoint">end coordinate</param>
    /// <param name="options">options</param>
    /// <returns>Angle between the provided points, or its explementary</returns>
    /// <example>
    /// <code>
    /// var startPoint = new Coordinate(5, 5);
    /// var midPoint = new Coordinate(5, 6);
    /// var endPoint = new Coordinate(3, 4);
    /// var result = Territory.GetAngle(startPoint, midPoint, endPoint); //= 45
    /// </code>
    /// </example>
    public static Angle GetAngle(
        Coordinate startPoint,
        Coordinate midPoint,
        Coordinate endPoint,
        Func<GetAngleOptions, GetAngleOptions>? configure = null
    )
    {
        var (explementary, mercator) =
            configure?.Invoke(GetAngleOptions.Empty) ?? GetAngleOptions.Empty;

        var azimuthA0 = BearingToAzimuth(
            !mercator ? Bearing(startPoint, midPoint) : RhumbBearing(startPoint, midPoint)
        );

        var azimuthB0 = BearingToAzimuth(
            !mercator ? Bearing(endPoint, midPoint) : RhumbBearing(endPoint, midPoint)
        );

        var angleA0 = (azimuthA0 - azimuthB0).Abs();

        if (explementary)
        {
            return Angles.TwoPi - angleA0;
        }

        return angleA0;
    }

    /// <summary>
    /// GetAngle options
    /// </summary>
    /// <param name="Explementary">Returns the explementary angle instead (360 - angle)</param>
    /// <param name="Mercator">Determines if the calculations should be performed over Mercator or WGS84 projection</param>
    public readonly record struct GetAngleOptions(bool Explementary = false, bool Mercator = false)
    {
        public static readonly GetAngleOptions Empty = new();
    }
}
