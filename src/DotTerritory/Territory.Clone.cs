using NetTopologySuite.Geometries;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Creates a deep copy of the input NTS Geometry.
    /// Mimics Turf.js clone function.
    /// </summary>
    /// <typeparam name="T">The type of the geometry.</typeparam>
    /// <param name="geometry">The geometry to clone.</param>
    /// <param name="useNtsClone">If true (default), uses the NTS built-in Clone method. If false, attempts a manual deep copy (less efficient, might be needed for specific cases).</param>
    /// <returns>A deep copy of the input geometry.</returns>
    /// <exception cref="ArgumentNullException">Thrown if geometry is null.</exception>
    public static T Clone<T>(T geometry, bool useNtsClone = true)
        where T : Geometry
    {
        ArgumentNullException.ThrowIfNull(geometry);

        if (useNtsClone)
        {
            // NTS Geometries have a built-in Clone() method which should perform a deep copy.
            return (T)geometry.Copy(); // NTS uses Copy() for deep cloning
        }
        else
        {
            // Manual deep copy (example for Point, extend for others if needed)
            // This is generally less efficient and more error-prone than NTS Copy()
            if (geometry is Point p)
            {
                return (T)(Geometry)new Point(p.Coordinate.Copy());
            }
            if (geometry is LineString l)
            {
                return (T)(Geometry)new LineString(l.Coordinates.Select(c => c.Copy()).ToArray());
            }
            if (geometry is Polygon poly)
            {
                var shell = (LinearRing)poly.ExteriorRing.Copy();
                var holes = poly.InteriorRings.Select(h => (LinearRing)h.Copy()).ToArray();
                return (T)(Geometry)new Polygon(shell, holes);
            }
            // Add manual cloning for MultiPoint, MultiLineString, MultiPolygon, GeometryCollection if !useNtsClone

            // Fallback to NTS Copy if manual implementation is missing
            return (T)geometry.Copy();
        }
    }
}
