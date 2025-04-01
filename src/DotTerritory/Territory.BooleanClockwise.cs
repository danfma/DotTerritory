namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a ring and returns true or false whether or not the ring is clockwise or counter-clockwise.
    /// </summary>
    /// <param name="coordinates">Array of coordinates forming a ring</param>
    /// <returns>True if the ring is clockwise, false if counter-clockwise</returns>
    /// <example>
    /// <code>
    /// var ring = new Coordinate[] {
    ///     new Coordinate(-75.343, 39.984),
    ///     new Coordinate(-75.534, 39.123),
    ///     new Coordinate(-75.230, 39.760),
    ///     new Coordinate(-75.343, 39.984)
    /// };
    /// var isClockwise = Territory.BooleanClockwise(ring);
    /// </code>
    /// </example>
    public static bool BooleanClockwise(Coordinate[] coordinates)
    {
        if (coordinates.Length < 4)
            throw new ArgumentException(
                "Ring must contain at least 4 coordinates (first and last being the same)"
            );

        // Check if first and last coordinates are the same, if not, throw error
        if (
            coordinates[0].X != coordinates[coordinates.Length - 1].X
            || coordinates[0].Y != coordinates[coordinates.Length - 1].Y
        )
        {
            throw new ArgumentException("First and last coordinates in the ring must be the same");
        }

        // Calculate the signed area of the ring
        double signedArea = 0;
        for (int i = 0; i < coordinates.Length - 1; i++)
        {
            signedArea +=
                (coordinates[i + 1].X - coordinates[i].X)
                * (coordinates[i + 1].Y + coordinates[i].Y);
        }

        // If the signed area is positive, the ring is clockwise (in a Cartesian plane)
        // For a geographic coordinate system with lat/lng, we need to reverse this logic
        // since Y increases northward, unlike in Cartesian plane where Y increases downward
        return signedArea < 0;
    }

    /// <summary>
    /// Takes a linear ring or polygon and returns true or false whether or not it is clockwise or counter-clockwise.
    /// </summary>
    /// <param name="geometry">A LinearRing or Polygon geometry</param>
    /// <returns>True if the geometry is clockwise, false if counter-clockwise</returns>
    public static bool BooleanClockwise(Geometry geometry)
    {
        if (geometry is LinearRing ring)
        {
            return BooleanClockwise(ring.Coordinates);
        }
        else if (geometry is Polygon polygon)
        {
            return BooleanClockwise(polygon.ExteriorRing.Coordinates);
        }
        else
        {
            throw new ArgumentException("Geometry must be a LinearRing or Polygon");
        }
    }
}
