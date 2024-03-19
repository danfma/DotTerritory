using NetTopologySuite.Geometries;
using UnitsNet;
using UnitsNet.Units;

namespace DotTerritory;

public static class Territory
{
    public static Length ToLength(Angle angle)
    {
        return Length.FromMeters(angle.Radians * TerritoryConfiguration.EarthRadius.Meters);
    }

    public static Angle ToRadians(Length distance)
    {
        return Angle.FromRadians(distance.Meters / TerritoryConfiguration.EarthRadius.Meters);
    }

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

    public static Length GetLength(LineString line)
    {
        var length = Length.FromMeters(0);

        for (var i = 0; i < line.NumPoints - 1; i++)
        {
            var from = line[i];
            var to = line[i + 1];

            length += Distance(from, to);
        }

        return length;
    }

    public static Length GetLength(MultiLineString multiLines)
    {
        var length = Length.FromMeters(0);

        return multiLines
            .Geometries
            .OfType<LineString>()
            .Aggregate(length, (current, segment) => current + GetLength(segment));
    }

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

    public static Point Along(LineString line, Length distance)
    {
        if (distance < Length.Zero)
        {
            line = new LineString(line.Coordinates.Reverse().ToArray());
            distance *= -1;
        }

        var travelled = Length.FromMeters(0);
        var coordinates = line.Coordinates;

        for (var i = 0; i < coordinates.Length; i++)
        {
            if (distance >= travelled && i == coordinates.Length - 1)
            {
                break;
            }

            if (travelled >= distance)
            {
                var overshot = distance - travelled;

                if (Length.Zero.Equals(overshot, Length.Zero))
                {
                    return new Point(coordinates[i]);
                }

                var bearing = Bearing(coordinates[i], coordinates[i - 1]);
                var direction = bearing - Angle.FromDegrees(180);
                var interpolated = Destination(coordinates[i], overshot, direction);

                return interpolated;
            }

            travelled += Distance(coordinates[i], coordinates[i + 1]);
        }

        return new Point(coordinates[^1]);
    }
}
