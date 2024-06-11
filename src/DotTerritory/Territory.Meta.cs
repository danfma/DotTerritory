namespace DotTerritory;

public static partial class Territory
{
    public static IEnumerable<LineSegment> GetSegments(LinearRing circuit)
    {
        var coordinates = circuit.Coordinates;
        var nextCoordinates = coordinates.Skip(1);

        foreach (var (current, next) in coordinates.Zip(nextCoordinates))
        {
            yield return new LineSegment(current, next);
        }
    }
}
