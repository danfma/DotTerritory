namespace DotTerritory;

public readonly record struct BBox(double West, double South, double East, double North)
{
    public static BBox operator +(BBox left, BBox right)
    {
        return new BBox(
            West: Math.Min(left.West, right.West),
            South: Math.Min(left.South, right.South),
            East: Math.Max(left.East, right.East),
            North: Math.Max(left.North, right.North)
        );
    }
    
    /// <summary>
    /// Checks if a point is contained within the bounding box.
    /// </summary>
    /// <param name="x">X coordinate of the point</param>
    /// <param name="y">Y coordinate of the point</param>
    /// <returns>True if the point is contained within the bounding box</returns>
    public bool Contains(double x, double y)
    {
        return x >= West && x <= East && y >= South && y <= North;
    }
}
