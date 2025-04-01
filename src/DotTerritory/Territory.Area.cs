namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// A constant used for converting degrees to radians.
    /// Represents the ratio of PI to 180.
    /// </summary>
    private static readonly double PiOver180 = Math.PI / 180.0;

    /// <summary>
    /// A constant factor used to compute the area of a polygon.
    /// It's derived from the square of the Earth's radius divided by 2.
    /// </summary>
    private static double Factor => EarthRadius.Meters * EarthRadius.Meters / 2.0;

    /// <summary>
    /// Calculates the area of a feature collection
    /// </summary>
    internal static Area Area(IEnumerable<IFeature> featureCollection)
    {
        var totalArea = UnitsNet.Area.Zero;

        foreach (var feature in featureCollection)
        {
            totalArea += Area(feature);
        }

        return totalArea;
    }

    /// <summary>
    /// Calculates the area of a feature
    /// </summary>
    public static Area Area(IFeature feature)
    {
        return Area(feature.Geometry);
    }

    /// <summary>
    /// Calculates the area of a geometry
    /// </summary>
    public static Area Area(Geometry geometry)
    {
        return geometry switch
        {
            Polygon polygon => CalculatePolygonArea(polygon),
            MultiPolygon multiPolygon => CalculateMultiPolygonArea(multiPolygon),
            _ => UnitsNet.Area.Zero,
        };
    }

    private static Area CalculatePolygonArea(Polygon polygon)
    {
        if (polygon.IsEmpty)
        {
            return UnitsNet.Area.Zero;
        }

        var total = Math.Abs(CalculateRingArea(polygon.ExteriorRing.Coordinates));

        total -= polygon.InteriorRings.Aggregate(
            0.0,
            (area, ring) => area + Math.Abs(CalculateRingArea(ring.Coordinates))
        );

        return UnitsNet.Area.FromSquareMeters(total);
    }

    private static Area CalculateMultiPolygonArea(MultiPolygon multiPolygon)
    {
        return multiPolygon.Geometries.Aggregate(
            UnitsNet.Area.Zero,
            (area, polygon) => area + CalculatePolygonArea((Polygon)polygon)
        );
    }

    private static double CalculateRingArea(Coordinate[] coordinates)
    {
        var coordinatesLength = coordinates.Length - 1;

        if (coordinatesLength <= 2)
        {
            return 0;
        }

        var total = 0.0;
        var i = 0;

        while (i < coordinatesLength)
        {
            var lower = coordinates[i];
            var middle = coordinates[(i + 1) % coordinatesLength];
            var upper = coordinates[(i + 2) % coordinatesLength];

            var lowerX = lower.X * PiOver180;
            var middleY = middle.Y * PiOver180;
            var upperX = upper.X * PiOver180;

            total += (upperX - lowerX) * Math.Sin(middleY);
            i++;
        }

        return total * Factor;
    }
}
