namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Calculates the distance along a rhumb line between two points in degrees, radians, miles, or kilometers.
    /// </summary>
    /// <param name="from">The origin point</param>
    /// <param name="to">The destination point</param>
    /// <param name="units">The units in which to return the distance</param>
    /// <returns>Distance between points</returns>
    /// <example>
    /// <code>
    /// var from = new Coordinate(-75.343, 39.984);
    /// var to = new Coordinate(-75.534, 39.123);
    /// var distance = Territory.RhumbDistance(from, to, LengthUnit.Kilometer);
    /// // => 97.994
    /// </code>
    /// </example>
    public static Length RhumbDistance(
        Coordinate from,
        Coordinate to,
        LengthUnit units = LengthUnit.Meter
    )
    {
        // Taken from http://www.movable-type.co.uk/scripts/latlong.html
        var phi1 = Angle.FromDegrees(from.Y).Radians;
        var phi2 = Angle.FromDegrees(to.Y).Radians;
        var deltaPhi = phi2 - phi1;
        var deltaLambda = Angle.FromDegrees(Math.Abs(to.X - from.X)).Radians;

        // If deltaLambda is over 180°, take the shorter rhumb line across the anti-meridian
        if (deltaLambda > Math.PI)
            deltaLambda = 2 * Math.PI - deltaLambda;

        // On Mercator projection, longitude distances shrink by latitude; q is the 'stretch factor'
        // q becomes ill-conditioned along E-W line (0/0); use empirical tolerance to avoid it
        double q;
        if (Math.Abs(deltaPhi) < 1e-10)
        {
            q = Math.Cos(phi1);
        }
        else
        {
            var deltaPsi = Math.Log(
                Math.Tan(phi2 / 2 + Math.PI / 4) / Math.Tan(phi1 / 2 + Math.PI / 4)
            );
            q = deltaPhi / deltaPsi;
        }

        // Distance is Pythagoras on 'stretched' Mercator projection
        var delta = Math.Sqrt(deltaPhi * deltaPhi + q * q * deltaLambda * deltaLambda); // Angular distance in radians
        var distance = delta * EarthRadius.As(units); // Distance in specified units

        return Length.From(distance, units);
    }

    /// <summary>
    /// Calculates the distance along a rhumb line between two points in degrees, radians, miles, or kilometers.
    /// </summary>
    /// <param name="longitude1">Longitude of the first point</param>
    /// <param name="latitude1">Latitude of the first point</param>
    /// <param name="longitude2">Longitude of the second point</param>
    /// <param name="latitude2">Latitude of the second point</param>
    /// <param name="units">The units in which to return the distance</param>
    /// <returns>Distance between points</returns>
    public static Length RhumbDistance(
        double longitude1,
        double latitude1,
        double longitude2,
        double latitude2,
        LengthUnit units = LengthUnit.Meter
    )
    {
        return RhumbDistance(
            new Coordinate(longitude1, latitude1),
            new Coordinate(longitude2, latitude2),
            units
        );
    }
}
