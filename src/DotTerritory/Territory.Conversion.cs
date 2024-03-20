namespace DotTerritory;

public static partial class Territory
{
    public static Length ToLength(Angle angle)
    {
        return Length.FromMeters(angle.Radians * EarthRadius.Meters);
    }

    public static Angle ToRadians(Length distance)
    {
        return Angle.FromRadians(distance.Meters / EarthRadius.Meters);
    }
}
