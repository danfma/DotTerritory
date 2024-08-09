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
}
