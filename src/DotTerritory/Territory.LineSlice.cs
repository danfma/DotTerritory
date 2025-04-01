// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.LineSlice.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a line, start point, and end point and returns a line segment between those points.
    /// If the start and end points match exactly with positions in the line geometry, the existing
    /// coordinates are used rather than introducing new points.
    /// </summary>
    /// <param name="startPoint">The starting point of the slice</param>
    /// <param name="endPoint">The ending point of the slice</param>
    /// <param name="line">The line to slice</param>
    /// <returns>Sliced line segment</returns>
    public static LineString LineSlice(Point startPoint, Point endPoint, LineString line)
    {
        var coords = line.Coordinates;
        var startVertex = NearestPointOnLine(line, startPoint);
        var endVertex = NearestPointOnLine(line, endPoint);

        // Sort the vertices by their position on the line
        var startVertexIndex = startVertex.Index;
        var endVertexIndex = endVertex.Index;
        var (lowerIndex, upperIndex) =
            startVertexIndex <= endVertexIndex
                ? (startVertexIndex, endVertexIndex)
                : (endVertexIndex, startVertexIndex);

        // Extract the relevant portion of the line
        var newCoordinates = new List<Coordinate>();

        // Add the start vertex
        var startCoordinate =
            startVertexIndex <= endVertexIndex
                ? startVertex.Point.Coordinate
                : endVertex.Point.Coordinate;
        newCoordinates.Add(startCoordinate);

        // Add any vertices between the start and end
        for (var i = lowerIndex + 1; i < upperIndex; i++)
        {
            newCoordinates.Add(coords[i]);
        }

        // Add the end vertex
        var endCoordinate =
            startVertexIndex <= endVertexIndex
                ? endVertex.Point.Coordinate
                : startVertex.Point.Coordinate;

        // Only add the end coordinate if it's different from the last one added
        if (newCoordinates.Count == 0 || !newCoordinates[^1].Equals(endCoordinate))
        {
            newCoordinates.Add(endCoordinate);
        }

        return new LineString(newCoordinates.ToArray());
    }

    /// <summary>
    /// Takes a line, start point, and end point and returns a line segment between those points
    /// </summary>
    /// <param name="startPoint">The starting point of the slice</param>
    /// <param name="endPoint">The ending point of the slice</param>
    /// <param name="line">The line to slice</param>
    /// <returns>Sliced line segment</returns>
    public static LineString LineSlice(Coordinate startPoint, Coordinate endPoint, LineString line)
    {
        return LineSlice(new Point(startPoint), new Point(endPoint), line);
    }
}
