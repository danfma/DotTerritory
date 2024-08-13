namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Smooths a {@link Polygon} or {@link MultiPolygon}. Based on [Chaikin's algorithm](http://graphics.cs.ucdavis.edu/education/CAGDNotes/Chaikins-Algorithm/Chaikins-Algorithm.html).
    /// Warning: may create degenerate polygons.
    /// </summary>
    public static Polygon Smooth(Polygon polygon, int iterations = 1)
    {
        var shell = Smooth(polygon.ExteriorRing, iterations);
        var holes = polygon.Holes.Select(hole => Smooth(hole, iterations)).ToArray();

        return new Polygon(shell, holes);
    }

    public static LinearRing Smooth(LineString ring, int iterations = 1)
    {
        var newCoordinates = new List<Coordinate>((ring.NumPoints - 1) * 2);

        for (var i = 0; i < ring.NumPoints; i++)
        {
            newCoordinates.Add(ring.GetCoordinateN(i));
        }

        for (var i = 0; i < iterations; i++)
        {
            for (var j = 0; j < newCoordinates.Count - 1; j++)
            {
                var current = newCoordinates[j];
                var next = newCoordinates[j + 1];

                newCoordinates[j] = new Coordinate(
                    0.75 * current.X + 0.25 * next.X,
                    0.75 * current.Y + 0.25 * next.Y
                );

                newCoordinates.Insert(
                    j + 1,
                    new Coordinate(
                        0.25 * current.X + 0.75 * next.X,
                        0.25 * current.Y + 0.75 * next.Y
                    )
                );

                j += 1;
            }
        }

        newCoordinates[^1] = newCoordinates[0];

        return new LinearRing(newCoordinates.ToArray());
    }
}
