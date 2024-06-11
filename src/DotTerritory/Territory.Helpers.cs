namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Converts any bearing angle from the north line direction (positive clockwise)
    /// and returns an angle between 0-360 degrees (positive clockwise), 0 being the north line
    /// </summary>
    /// <param name="bearing">bearing angle - between -180 and +180 degrees</param>
    /// <returns>angle between 0 and 360 degrees</returns>
    public static Angle BearingToAzimuth(Angle bearing)
    {
        var angle = bearing.Degrees % 360;

        if (angle < 0)
        {
            angle += 360;
        }

        return Angle.FromDegrees(angle);
    }
}
