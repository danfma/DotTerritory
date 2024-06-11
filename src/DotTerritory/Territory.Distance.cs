namespace DotTerritory;

public static partial class Territory
{
    public static Length Distance(Coordinate from, Coordinate to)
    {
        var dLat = Angle.FromDegrees(to[1] - from[1]).ToUnit(AngleUnit.Radian);
        var dLon = Angle.FromDegrees(to[0] - from[0]).ToUnit(AngleUnit.Radian);
        var lat1 = Angle.FromDegrees(from[1]).ToUnit(AngleUnit.Radian);
        var lat2 = Angle.FromDegrees(to[1]).ToUnit(AngleUnit.Radian);

        var a =
            Math.Pow(Math.Sin(dLat.Radians / 2), 2)
            + Math.Pow(Math.Sin(dLon.Radians / 2), 2)
                * Math.Cos(lat1.Radians)
                * Math.Cos(lat2.Radians);

        var radians = Angle.FromRadians(2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)));

        return ToLength(radians);
    }
}
