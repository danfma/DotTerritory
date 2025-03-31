using NetTopologySuite.Geometries;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Throws an ArgumentException if the geometry is not of the expected type.
    /// Mimics Turf.js invariant.featureOf function.
    /// </summary>
    /// <param name="geometry">The geometry to check.</param>
    /// <param name="expectedType">The expected NTS geometry type (e.g., typeof(Point), typeof(Polygon)).</param>
    /// <param name="name">Name of the argument being validated.</param>
    /// <exception cref="ArgumentException">Thrown if the geometry type doesn't match.</exception>
    public static void EnsureFeatureOf(Geometry geometry, Type expectedType, string name)
    {
        if (geometry == null)
        {
            throw new ArgumentNullException(name, $"Geometry cannot be null.");
        }
        if (!expectedType.IsAssignableFrom(geometry.GetType()))
        {
            throw new ArgumentException(
                $"{name} must be a {expectedType.Name}, but was {geometry.GeometryType}",
                name
            );
        }
    }

    /// <summary>
    /// Throws an ArgumentException if the geometry is not of the expected type string.
    /// Mimics Turf.js invariant.type function with specific type check.
    /// </summary>
    /// <param name="geometry">The geometry to check.</param>
    /// <param name="expectedType">The expected geometry type name (e.g., "Point", "Polygon").</param>
    /// <param name="name">Name of the argument being validated.</param>
    /// <exception cref="ArgumentException">Thrown if the geometry type name doesn't match.</exception>
    public static void EnsureType(Geometry geometry, string expectedType, string name)
    {
        if (geometry == null)
        {
            throw new ArgumentNullException(name, $"Geometry cannot be null.");
        }
        // NTS GeometryType might differ slightly from GeoJSON type names in casing, handle common cases
        string ntsType = geometry.GeometryType;
        bool match =
            ntsType.Equals(expectedType, StringComparison.OrdinalIgnoreCase)
            || (expectedType == "Point" && geometry is Point)
            || (expectedType == "LineString" && geometry is LineString)
            || (expectedType == "Polygon" && geometry is Polygon)
            || (expectedType == "MultiPoint" && geometry is MultiPoint)
            || (expectedType == "MultiLineString" && geometry is MultiLineString)
            || (expectedType == "MultiPolygon" && geometry is MultiPolygon)
            || (expectedType == "GeometryCollection" && geometry is GeometryCollection);

        if (!match)
        {
            throw new ArgumentException(
                $"{name} must be a {expectedType}, but was {geometry.GeometryType}",
                name
            );
        }
    }

    /// <summary>
    /// Get the GeoJSON type name for a NTS Geometry.
    /// Mimics Turf.js invariant.getType function.
    /// </summary>
    /// <param name="geometry">The geometry.</param>
    /// <returns>The GeoJSON type name (e.g., "Point", "Polygon").</returns>
    /// <exception cref="ArgumentNullException">Thrown if geometry is null.</exception>
    public static string GetType(Geometry geometry)
    {
        ArgumentNullException.ThrowIfNull(geometry);

        return geometry switch
        {
            Point _ => "Point",
            LineString _ => "LineString",
            Polygon _ => "Polygon",
            MultiPoint _ => "MultiPoint",
            MultiLineString _ => "MultiLineString",
            MultiPolygon _ => "MultiPolygon",
            GeometryCollection _ => "GeometryCollection",
            _ => geometry.GeometryType // Fallback, though NTS types usually map directly
        };
    }

    // Add other invariant functions as needed (e.g., collectionOf, coord)
}
