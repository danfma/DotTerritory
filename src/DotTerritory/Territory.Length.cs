namespace DotTerritory;

public static partial class Territory
{
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
            .Geometries.OfType<LineString>()
            .Aggregate(length, (current, segment) => current + GetLength(segment));
    }
}
