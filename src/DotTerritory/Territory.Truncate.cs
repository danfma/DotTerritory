namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Truncates the coordinates of a Geometry to a specific precision.
    /// Mimics Turf.js truncate function.
    /// </summary>
    /// <typeparam name="T">The type of the geometry.</typeparam>
    /// <param name="geometry">Geometry to be truncated.</param>
    /// <param name="precision">The number of decimal places to keep (default 6).</param>
    /// <param name="coordinates">Number of dimensions to truncate (2 for [x, y], 3 for [x, y, z], default 2).</param>
    /// <param name="mutate">Allows GeoJSON input to be mutated (default false). NTS geometries are generally mutated by Apply methods.</param>
    /// <returns>The truncated geometry. Note: If mutate is false (default for Turf compatibility), a new geometry instance is returned. If mutate is true, the input NTS geometry instance might be modified.</returns>
    public static T Truncate<T>(
        T geometry,
        int precision = 6,
        int coordinates = 2,
        bool mutate = false
    )
        where T : Geometry
    {
        ArgumentNullException.ThrowIfNull(geometry);

        if (precision < 0)
            throw new ArgumentOutOfRangeException(
                nameof(precision),
                "Precision must be a positive number"
            );
        if (coordinates < 2)
            throw new ArgumentOutOfRangeException(
                nameof(coordinates),
                "Coordinates must be at least 2"
            );

        double factor = Math.Pow(10, precision);

        var targetGeometry = mutate ? geometry : (T)geometry.Copy();

        var filter = new TruncateCoordinateFilter(factor, coordinates);
        targetGeometry.Apply(filter);

        // NTS Apply doesn't guarantee modification in-place if the filter indicates no change
        // but Truncate should always return the potentially modified geometry
        return targetGeometry;
    }

    private class TruncateCoordinateFilter : ICoordinateSequenceFilter
    {
        private readonly double _factor;
        private readonly int _dimensions;
        private bool _geometryChanged; // Track if any coordinate was actually changed

        public TruncateCoordinateFilter(double factor, int dimensions)
        {
            _factor = factor;
            _dimensions = dimensions;
            Done = false;
            _geometryChanged = false; // Initialize to false
        }

        public bool Done { get; private set; }
        public bool GeometryChanged => _geometryChanged; // Implement the required property

        public void Filter(CoordinateSequence seq, int i)
        {
            double originalX = seq.GetOrdinate(i, Ordinate.X);
            double originalY = seq.GetOrdinate(i, Ordinate.Y);
            double truncatedX = Math.Truncate(originalX * _factor) / _factor;
            double truncatedY = Math.Truncate(originalY * _factor) / _factor;

            if (truncatedX != originalX)
            {
                seq.SetOrdinate(i, Ordinate.X, truncatedX);
                _geometryChanged = true;
            }
            if (truncatedY != originalY)
            {
                seq.SetOrdinate(i, Ordinate.Y, truncatedY);
                _geometryChanged = true;
            }

            // Truncate Z
            if (_dimensions >= 3 && seq.HasZ && seq.Dimension >= 3)
            {
                double originalZ = seq.GetOrdinate(i, Ordinate.Z);
                double truncatedZ = Math.Truncate(originalZ * _factor) / _factor;
                if (truncatedZ != originalZ)
                {
                    seq.SetOrdinate(i, Ordinate.Z, truncatedZ);
                    _geometryChanged = true;
                }
            }

            // Truncate M
            if (_dimensions >= 4 && seq.HasM && seq.Dimension >= 4)
            {
                double originalM = seq.GetOrdinate(i, Ordinate.M);
                double truncatedM = Math.Truncate(originalM * _factor) / _factor;
                if (truncatedM != originalM)
                {
                    seq.SetOrdinate(i, Ordinate.M, truncatedM);
                    _geometryChanged = true;
                }
            }

            // If it's the last coordinate in the sequence, mark as Done
            if (i == seq.Count - 1)
            {
                Done = true;
            }
        }
    }
}
