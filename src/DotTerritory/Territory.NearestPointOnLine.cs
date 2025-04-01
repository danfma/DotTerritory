// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.NearestPointOnLine.cs
namespace DotTerritory;

public class PointOnLine
{
    public Point Point { get; }
    public int Index { get; }
    public Length Distance { get; }

    public PointOnLine(Point point, int index, Length distance)
    {
        Point = point;
        Index = index;
        Distance = distance;
    }
}

public static partial class Territory
{
    /// <summary>
    /// Takes a Point and a LineString and returns the closest Point on the LineString.
    /// </summary>
    /// <param name="line">The LineString to find the nearest point on</param>
    /// <param name="point">The Point to find the nearest point to</param>
    /// <returns>A PointOnLine object containing the nearest point, its index, and distance</returns>
    public static PointOnLine NearestPointOnLine(LineString line, Point point)
    {
        var coords = line.Coordinates;
        var pointCoord = point.Coordinate;

        // If there's only one coordinate in the line, return that
        if (coords.Length == 1)
        {
            var closestPoint = new Point(coords[0]);
            var distance = Distance(pointCoord, coords[0]);
            return new PointOnLine(closestPoint, 0, distance);
        }

        var minDistance = double.MaxValue;
        var nearestIndex = 0;
        Coordinate? nearestCoord = null;

        // Find the nearest segment and point
        for (var i = 0; i < coords.Length - 1; i++)
        {
            var start = coords[i];
            var end = coords[i + 1];

            var onSegment = PointOnSegment(pointCoord, start, end);
            var distance = Distance(pointCoord, onSegment).Meters;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCoord = onSegment;
                nearestIndex = i;
            }
        }

        return new PointOnLine(
            new Point(nearestCoord!),
            nearestIndex,
            Length.FromMeters(minDistance)
        );
    }

    /// <summary>
    /// Returns the closest point on a line segment to the specified point
    /// </summary>
    private static Coordinate PointOnSegment(Coordinate point, Coordinate start, Coordinate end)
    {
        var x = point.X;
        var y = point.Y;
        var x1 = start.X;
        var y1 = start.Y;
        var x2 = end.X;
        var y2 = end.Y;

        var dx = x2 - x1;
        var dy = y2 - y1;

        // Line is just a point
        if (Math.Abs(dx) < double.Epsilon && Math.Abs(dy) < double.Epsilon)
        {
            return new Coordinate(x1, y1);
        }

        // Calculate the parametric position (t) of the projection of the point on the line
        var t = ((x - x1) * dx + (y - y1) * dy) / (dx * dx + dy * dy);

        // Restrict to line segment
        if (t < 0)
            t = 0;
        if (t > 1)
            t = 1;

        // Calculate the projected point
        var projX = x1 + t * dx;
        var projY = y1 + t * dy;

        return new Coordinate(projX, projY);
    }
}
