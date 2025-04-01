using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries.Utilities;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Rewinds the order of coordinates in a Polygon, MultiPolygon (and optionally GeometryCollection)
    /// to follow the right-hand rule (counter-clockwise exterior rings, clockwise interior rings)
    /// as recommended by the GeoJSON specification.
    /// Mimics Turf.js rewind function.
    /// </summary>
    /// <typeparam name="T">The type of the geometry.</typeparam>
    /// <param name="geometry">The input geometry (Polygon, MultiPolygon, or GeometryCollection).</param>
    /// <param name="reverse">Set to true to enforce the left-hand rule (clockwise exterior) (default false).</param>
    /// <param name="mutate">Allows GeoJSON input to be mutated (default false). NTS geometries are generally mutated when using GeometryEditor if changes occur.</param>
    /// <returns>The rewound geometry. Note: GeometryEditor typically returns a new instance if modifications occurred.</returns>
    public static T Rewind<T>(T geometry, bool reverse = false, bool mutate = false)
        where T : Geometry
    {
        ArgumentNullException.ThrowIfNull(geometry);

        // Note: The 'mutate' parameter from Turf is less relevant when using GeometryEditor,
        // as it often creates new instances anyway if changes are needed.
        // If strict non-mutation is required even if no rewind happens, clone first.
        var geometryToEdit = mutate ? geometry : (T)geometry.Copy();

        var editor = new GeometryEditor(); // Use default factory from the input geometry
        var operation = new RewindOperation(reverse);

        // Edit returns the potentially modified geometry
        var result = editor.Edit(geometryToEdit, operation);

        return (T)result;
    }

    /// <summary>
    /// Operation for GeometryEditor to rewind Polygons.
    /// </summary>
    private class RewindOperation : GeometryEditor.IGeometryEditorOperation
    {
        private readonly bool _reverse;

        public RewindOperation(bool reverse)
        {
            _reverse = reverse;
        }

        public Geometry Edit(Geometry geometry, GeometryFactory factory)
        {
            if (geometry is Polygon polygon)
            {
                return RewindPolygonInternal(polygon, _reverse, factory);
            }
            // GeometryEditor handles iterating through MultiPolygons and GeometryCollections,
            // applying this Edit operation to each Polygon found within.
            // Other geometry types (Points, LineStrings) are returned unchanged by default.
            return geometry;
        }

        private Polygon RewindPolygonInternal(
            Polygon polygon,
            bool reverse,
            GeometryFactory factory
        )
        {
            LinearRing? newExteriorRing = null;
            var newInteriorRings = new List<LinearRing>();
            bool changed = false;

            // --- Process Exterior Ring ---
            var exteriorRing = (LinearRing)polygon.ExteriorRing; // Cast
            bool exteriorIsCCW = Orientation.IsCCW(exteriorRing.CoordinateSequence);
            bool shouldReverseExterior = exteriorIsCCW == reverse;

            if (shouldReverseExterior)
            {
                newExteriorRing = CreateReversedRing(exteriorRing, factory);
                changed = true;
            }
            else
            {
                newExteriorRing = (LinearRing)exteriorRing.Copy(); // Keep as is, but copy
            }

            // --- Process Interior Rings ---
            for (int i = 0; i < polygon.NumInteriorRings; i++)
            {
                var hole = (LinearRing)polygon.GetInteriorRingN(i); // Cast
                bool holeIsCCW = Orientation.IsCCW(hole.CoordinateSequence);
                bool shouldReverseHole = holeIsCCW != reverse;

                if (shouldReverseHole)
                {
                    newInteriorRings.Add(CreateReversedRing(hole, factory));
                    changed = true;
                }
                else
                {
                    newInteriorRings.Add((LinearRing)hole.Copy()); // Keep as is, but copy
                }
            }

            // Return new Polygon only if changes were made, otherwise return original (or its copy)
            return changed
                ? factory.CreatePolygon(newExteriorRing, newInteriorRings.ToArray())
                : polygon;
        }

        private LinearRing CreateReversedRing(LinearRing ring, GeometryFactory factory)
        {
            var originalSeq = ring.CoordinateSequence;
            var reversedCoords = new Coordinate[originalSeq.Count];
            int last = originalSeq.Count - 1;
            for (int i = 0; i <= last; i++)
            {
                // Create new Coordinate objects for the reversed sequence
                reversedCoords[i] = new Coordinate();
                reversedCoords[i].X = originalSeq.GetOrdinate(last - i, Ordinate.X);
                reversedCoords[i].Y = originalSeq.GetOrdinate(last - i, Ordinate.Y);
                if (originalSeq.HasZ)
                    reversedCoords[i].Z = originalSeq.GetOrdinate(last - i, Ordinate.Z);
                if (originalSeq.HasM)
                    reversedCoords[i].M = originalSeq.GetOrdinate(last - i, Ordinate.M);
            }
            // Create sequence using the factory provided by the Edit operation
            var newSeq = factory.CoordinateSequenceFactory.Create(reversedCoords);
            return factory.CreateLinearRing(newSeq);
        }
    }
}
