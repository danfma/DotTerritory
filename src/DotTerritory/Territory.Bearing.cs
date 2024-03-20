using NetTopologySuite.Geometries;

namespace DotTerritory;

public static partial class Territory
{
    public static Angle Bearing(Coordinate from, Coordinate to, bool final = false)
    {
        if (final)
        {
            return CalculateFinalBearing(from, to);
        }

        var longitude1 = Angle.FromDegrees(from.X).Radians;
        var longitude2 = Angle.FromDegrees(to.X).Radians;
        var latitude1 = Angle.FromDegrees(from.Y).Radians;
        var latitude2 = Angle.FromDegrees(to.Y).Radians;
        var longitudeDiff = longitude2 - longitude1;

        var a = Math.Sin(longitudeDiff) * Math.Cos(latitude2);
        var b =
            Math.Cos(latitude1) * Math.Sin(latitude2)
            - Math.Sin(latitude1) * Math.Cos(latitude2) * Math.Cos(longitudeDiff);

        var radians = Math.Atan2(a, b);

        return Angle.FromRadians(radians).ToUnit(AngleUnit.Degree);

        static Angle CalculateFinalBearing(Coordinate start, Coordinate end)
        {
            var bear = Bearing(end, start);
            bear = Angle.FromDegrees((bear + Angle.FromDegrees(180)).Degrees % 360);

            return bear;
        }
    }
}
