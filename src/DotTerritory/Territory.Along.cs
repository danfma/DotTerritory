using System.Diagnostics;

namespace DotTerritory;

public static partial class Territory
{
    public static Point WalkAlong(LineString line, Length distance)
    {
        if (distance < Length.Zero)
        {
            var reversed = line.Coordinates.ToArray();
            Array.Reverse(reversed);
            line = new LineString(reversed);
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

    public static Point WalkAlong(LinearRing circuit, Length distance)
    {
        var reverse = distance < Length.Zero;
        var reverseModifier = reverse ? -1 : 1;
        var travelled = Length.FromMeters(0);
        var coordinates = LoopCoordinates(circuit, reverse);

        distance *= reverseModifier;

        foreach (var (current, previous, next) in coordinates)
        {
            if (travelled >= distance)
            {
                var overshot = distance - travelled;

                if (Length.Zero.Equals(overshot, Length.Zero))
                {
                    return new Point(current);
                }

                var bearing = Bearing(current, previous);
                var direction = bearing - Angle.FromDegrees(180);
                var interpolated = Destination(current, overshot, direction);

                return interpolated;
            }

            travelled += Distance(current, next);
        }

        throw new UnreachableException("The loop should never end without finding a point");
    }

    private static IEnumerable<(
        Coordinate Current,
        Coordinate Previous,
        Coordinate Next
    )> LoopCoordinates(Geometry geometry, bool reverse)
    {
        // skipping the last coordinate because it will be the same as the first one
        var coordinates = new ArraySegment<Coordinate>(
            geometry.Coordinates,
            0,
            geometry.Coordinates.Length - 1
        );

        return LoopCoordinates(coordinates, reverse);
    }

    private static IEnumerable<(
        Coordinate Current,
        Coordinate Previous,
        Coordinate Next
    )> LoopCoordinates(ArraySegment<Coordinate> coordinates, bool reverse)
    {
        if (reverse)
        {
            var previousIndex = 1;
            var currentIndex = 0;
            var nextIndex = coordinates.Count - 1;

            while (true)
            {
                var previous = coordinates[previousIndex];
                var current = coordinates[currentIndex];
                var next = coordinates[nextIndex];

                yield return (current, previous, next);

                (previousIndex, currentIndex, nextIndex) = (
                    currentIndex,
                    nextIndex,
                    (--nextIndex + coordinates.Count) % coordinates.Count
                );
            }
        }
        else
        {
            var previousIndex = coordinates.Count - 1;
            var currentIndex = 0;
            var nextIndex = 1;

            while (true)
            {
                var previous = coordinates[previousIndex];
                var current = coordinates[currentIndex];
                var next = coordinates[nextIndex];

                yield return (current, previous, next);

                (previousIndex, currentIndex, nextIndex) = (
                    currentIndex,
                    nextIndex,
                    (nextIndex + 1) % coordinates.Count
                );
            }
        }
    }
}
