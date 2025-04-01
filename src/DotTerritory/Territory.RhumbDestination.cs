namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Returns the destination point having traveled the given distance along a rhumb line from the
    /// origin point with the given bearing.
    /// </summary>
    /// <param name="origin">Starting point</param>
    /// <param name="distance">Distance from the starting point</param>
    /// <param name="bearing">Constant bearing angle ranging from -180 to 180 degrees from north</param>
    /// <param name="options">Optional parameters: units (of the distance)</param>
    /// <returns>Destination point</returns>
    /// <example>
    /// <code>
    /// var origin = new Coordinate(-75.343, 39.984);
    /// var distance = Length.FromKilometers(50);
    /// var bearing = Angle.FromDegrees(90);
    /// var destination = Territory.RhumbDestination(origin, distance, bearing);
    /// // => Coordinate(-74.398, 39.984)
    /// </code>
    /// </example>
    public static Coordinate RhumbDestination(
        Coordinate origin,
        Length distance,
        Angle bearing,
        Func<RhumbDestinationOptions, RhumbDestinationOptions>? configure = null
    )
    {
        var options =
            configure?.Invoke(RhumbDestinationOptions.Empty) ?? RhumbDestinationOptions.Empty;
        var phi1 = Angle.FromDegrees(origin.Y).Radians;
        var lambda1 = Angle.FromDegrees(origin.X).Radians;
        var bearingRad = bearing.Radians;
        var distanceInMeters = distance.Meters;
        var radius = EarthRadius.Meters;
        var delta = distanceInMeters / radius; // Angular distance in radians

        // Calculate new latitude
        var phi2 = phi1 + delta * Math.Cos(bearingRad);

        // Check for some edge cases
        if (Math.Abs(phi2) > Math.PI / 2)
            phi2 = phi2 > 0 ? Math.PI - phi2 : -Math.PI - phi2;

        // Calculate latitude where we cross the 90 or -90 degree lines
        var deltaPhi = Math.Log(
            Math.Tan(phi2 / 2 + Math.PI / 4) / Math.Tan(phi1 / 2 + Math.PI / 4)
        );

        // Calculate q - the east-west component of distance
        var q = Math.Abs(deltaPhi) > 1e-10 ? deltaPhi / Math.Sin(bearingRad) : Math.Cos(phi1);

        // Calculate new longitude
        var deltaLambda = delta * Math.Sin(bearingRad) / q;
        var lambda2 = lambda1 + deltaLambda;

        // Normalize longitude to range -180 to 180
        lambda2 = (lambda2 + 3 * Math.PI) % (2 * Math.PI) - Math.PI;

        // Convert back to degrees
        var newLat = Angle.FromRadians(phi2).Degrees;
        var newLng = Angle.FromRadians(lambda2).Degrees;

        // Return point object
        return new Coordinate(newLng, newLat);
    }

    /// <summary>
    /// Returns the destination point having traveled the given distance along a rhumb line from the
    /// origin point with the given bearing.
    /// </summary>
    /// <param name="longitude">Longitude of the starting point</param>
    /// <param name="latitude">Latitude of the starting point</param>
    /// <param name="distance">Distance from the starting point</param>
    /// <param name="bearing">Constant bearing angle</param>
    /// <param name="options">Optional parameters: units (of the distance)</param>
    /// <returns>Destination point</returns>
    public static Coordinate RhumbDestination(
        double longitude,
        double latitude,
        Length distance,
        Angle bearing,
        Func<RhumbDestinationOptions, RhumbDestinationOptions>? configure = null
    )
    {
        return RhumbDestination(new Coordinate(longitude, latitude), distance, bearing, configure);
    }

    /// <summary>
    /// RhumbDestination options
    /// </summary>
    /// <param name="Units">Length units of the distance (defaults to meters)</param>
    public record struct RhumbDestinationOptions(LengthUnit Units = LengthUnit.Meter)
    {
        public static readonly RhumbDestinationOptions Empty = new();
    }
}
