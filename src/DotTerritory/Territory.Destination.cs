namespace DotTerritory;

public static partial class Territory
{
    public static Point Destination(Coordinate origin, Length distance, Angle bearing)
    {
        var longitude = Angle.FromDegrees(origin[0]).ToUnit(AngleUnit.Radian);
        var latitude = Angle.FromDegrees(origin[1]).ToUnit(AngleUnit.Radian);
        var radians = ToRadians(distance);

        var latitude2 = Math.Asin(
            Math.Sin(latitude.Radians) * Math.Cos(radians.Radians)
                + Math.Cos(latitude.Radians) * Math.Sin(radians.Radians) * Math.Cos(bearing.Radians)
        );

        var longitude2 =
            longitude.Radians
            + Math.Atan2(
                Math.Sin(bearing.Radians) * Math.Sin(radians.Radians) * Math.Cos(latitude.Radians),
                Math.Cos(radians.Radians) - Math.Sin(latitude.Radians) * Math.Sin(latitude2)
            );

        return new Point(
            Angle.FromRadians(longitude2).Degrees,
            Angle.FromRadians(latitude2).Degrees
        );
    }
}
