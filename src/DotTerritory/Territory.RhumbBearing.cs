namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes two coordinates and finds the bearing angle between them along a Rhumb line
    /// i.e. the angle measured in degrees start the north line (0 degrees)
    /// </summary>
    /// <param name="startPoint">The start point</param>
    /// <param name="endPoint">The end point</param>
    /// <param name="configure">A function to configure the calculation</param>
    /// <returns>Bearing from the north in decimal degrees, between -180 and +180 degrees (positive clockwise)</returns>
    /// <example>
    /// <code>
    /// var point1 = new Coordinate(-75.343, 39.984);
    /// var point2 = new Coordinate(-75.534, 39.123);
    /// var bearing = Territory.RhumbBearing(point1, point2); //= 9.71 degrees
    /// </code>
    /// </example>
    public static Angle RhumbBearing(
        Coordinate startPoint,
        Coordinate endPoint,
        Func<RhumbBearingOptions, RhumbBearingOptions>? configure = null
    )
    {
        var options = configure?.Invoke(RhumbBearingOptions.Empty) ?? RhumbBearingOptions.Empty;

        var bear360 = options.Final
            ? CalculateRhumbBearing(endPoint, startPoint)
            : CalculateRhumbBearing(startPoint, endPoint);

        var bear180 = bear360 > Angles.Pi ? -(Angles.TwoPi - bear360) : bear360;

        return bear180.ToUnit(AngleUnit.Degree);
    }

    private static Angle CalculateRhumbBearing(Coordinate startPoint, Coordinate endPoint)
    {
        var phi1 = Angle.FromDegrees(startPoint.Y).ToUnit(AngleUnit.Radian);
        var phi2 = Angle.FromDegrees(endPoint.Y).ToUnit(AngleUnit.Radian);
        var deltaLambda = Angle.FromDegrees(endPoint.X - startPoint.X).ToUnit(AngleUnit.Radian);

        if (deltaLambda > Angles.Pi)
        {
            deltaLambda -= Angles.TwoPi;
        }

        if (deltaLambda < -Angles.Pi)
        {
            deltaLambda += Angles.TwoPi;
        }

        var deltaPsi = Math.Log(
            Math.Tan(phi2.Radians / 2 + Math.PI / 4) / Math.Tan(phi1.Radians / 2 + Math.PI / 4)
        );

        var theta = Math.Atan2(deltaLambda.Radians, deltaPsi);
        var result = (Angle.FromRadians(theta).Degrees + 360) % 360;

        return Angle.FromDegrees(result);
    }

    public readonly record struct RhumbBearingOptions(bool Final = false)
    {
        public static readonly RhumbBearingOptions Empty = new();
    }
}
