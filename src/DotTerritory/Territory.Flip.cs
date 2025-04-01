namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Flips the coordinates of a Geometry (x becomes y, y becomes x).
    /// Mimics Turf.js flip function.
    /// </summary>
    /// <typeparam name="T">The type of the geometry.</typeparam>
    /// <param name="geometry">Geometry to flip.</param>
    /// <param name="mutate">Allows GeoJSON input to be mutated (default false). NTS geometries are generally mutated by Apply methods.</param>
    /// <returns>The flipped geometry. Note: If mutate is false (default for Turf compatibility), a new geometry instance is returned. If mutate is true, the input NTS geometry instance might be modified.</returns>
    public static T Flip<T>(T geometry, bool mutate = false)
        where T : Geometry
    {
        ArgumentNullException.ThrowIfNull(geometry);

        var targetGeometry = mutate ? geometry : (T)geometry.Copy();

        targetGeometry.Apply(new FlipCoordinateFilter());
        targetGeometry.GeometryChanged(); // Notify NTS that geometry has changed

        return targetGeometry;
    }

    private class FlipCoordinateFilter : ICoordinateSequenceFilter
    {
        public FlipCoordinateFilter()
        {
            Done = false;
            GeometryChanged = true; // Assume changes will happen
        }

        public bool Done { get; private set; }
        public bool GeometryChanged { get; private set; }

        public void Filter(CoordinateSequence seq, int i)
        {
            double x = seq.GetOrdinate(i, Ordinate.X);
            double y = seq.GetOrdinate(i, Ordinate.Y);
            seq.SetOrdinate(i, Ordinate.X, y);
            seq.SetOrdinate(i, Ordinate.Y, x);

            // If it's the last coordinate in the sequence, mark as Done
            if (i == seq.Count - 1)
            {
                Done = true;
            }
        }
    }
}
