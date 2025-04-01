namespace DotTerritory;

/// <summary>
/// Contains utility methods shared across multiple Territory classes
/// </summary>
internal static class TerritoryUtils
{
    /// <summary>
    /// Creates a grid of cells from a point grid
    /// </summary>
    public static List<(Coordinate, Coordinate, Coordinate, Coordinate)> CreateGrid(
        Point[] pointGrid,
        Dictionary<(double X, double Y), double> zValues
    )
    {
        // Find the unique x and y values in the grid
        var xValues = zValues.Keys.Select(p => p.X).Distinct().OrderBy(x => x).ToList();
        var yValues = zValues.Keys.Select(p => p.Y).Distinct().OrderBy(y => y).ToList();

        // Create cells from adjacent grid points
        var cells = new List<(Coordinate, Coordinate, Coordinate, Coordinate)>();

        for (int x = 0; x < xValues.Count - 1; x++)
        {
            for (int y = 0; y < yValues.Count - 1; y++)
            {
                var bottomLeftKey = (xValues[x], yValues[y]);
                var bottomRightKey = (xValues[x + 1], yValues[y]);
                var topRightKey = (xValues[x + 1], yValues[y + 1]);
                var topLeftKey = (xValues[x], yValues[y + 1]);

                // Only create cells where we have all corner values
                if (
                    zValues.ContainsKey(bottomLeftKey)
                    && zValues.ContainsKey(bottomRightKey)
                    && zValues.ContainsKey(topRightKey)
                    && zValues.ContainsKey(topLeftKey)
                )
                {
                    var bottomLeft = new Coordinate(bottomLeftKey.Item1, bottomLeftKey.Item2);
                    var bottomRight = new Coordinate(bottomRightKey.Item1, bottomRightKey.Item2);
                    var topRight = new Coordinate(topRightKey.Item1, topRightKey.Item2);
                    var topLeft = new Coordinate(topLeftKey.Item1, topLeftKey.Item2);

                    cells.Add((bottomLeft, bottomRight, topRight, topLeft));
                }
            }
        }

        return cells;
    }

    /// <summary>
    /// Checks if two points are close enough to be considered the same
    /// </summary>
    public static bool ArePointsClose(Coordinate p1, Coordinate p2, double tolerance = 1e-8)
    {
        var distX = p1.X - p2.X;
        var distY = p1.Y - p2.Y;
        return (distX * distX + distY * distY) < tolerance * tolerance;
    }
}
