using NetTopologySuite.Geometries;

namespace DotTerritory;

public static partial class Territory
{
    public static Polygon Circle(Coordinate center, Length radius, int steps = 64)
    {
        var coordinates = new Coordinate[steps + 1];

        for (var i = 0; i < steps; i++)
        {
            var angle = Angle.FromDegrees(i * -360.0 / steps).ToUnit(AngleUnit.Radian);
            var destination = Destination(center, radius, angle);

            coordinates[i] = destination.Coordinate;
        }

        coordinates[^1] = coordinates[0];

        return new Polygon(new LinearRing(coordinates));
    }
}