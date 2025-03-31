using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Determine whether two geometries are equal.
    /// Geometries are considered equal when their coordinates are the same,
    /// and in the same order for each component.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature or Geometry</param>
    /// <param name="feature2">GeoJSON Feature or Geometry</param>
    /// <param name="precision">The precision used for comparing coordinates, in decimal digits. Default is 6.</param>
    /// <returns>true/false</returns>
    public static bool BooleanEqual(Geometry feature1, Geometry feature2, int precision = 6)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        if (precision < 0)
            throw new ArgumentException("Precision must be a positive number", nameof(precision));

        // Make sure we're comparing the same geometry types
        if (feature1.GeometryType != feature2.GeometryType)
            return false;

        // Use NetTopologySuite's EqualsExact method with a tolerance based on precision
        double tolerance = Math.Pow(10, -precision);
        return feature1.EqualsExact(feature2, tolerance);
    }

    /// <summary>
    /// Determine whether two geometries are equal.
    /// Geometries are considered equal when their coordinates are the same,
    /// and in the same order for each component.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <param name="precision">The precision used for comparing coordinates, in decimal digits. Default is 6.</param>
    /// <returns>true/false</returns>
    public static bool BooleanEqual(IFeature feature1, IFeature feature2, int precision = 6)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanEqual(feature1.Geometry, feature2.Geometry, precision);
    }

    /// <summary>
    /// Determine whether two geometries are equal.
    /// Geometries are considered equal when their coordinates are the same,
    /// and in the same order for each component.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Geometry</param>
    /// <param name="precision">The precision used for comparing coordinates, in decimal digits. Default is 6.</param>
    /// <returns>true/false</returns>
    public static bool BooleanEqual(IFeature feature1, Geometry feature2, int precision = 6)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanEqual(feature1.Geometry, feature2, precision);
    }

    /// <summary>
    /// Determine whether two geometries are equal.
    /// Geometries are considered equal when their coordinates are the same,
    /// and in the same order for each component.
    /// </summary>
    /// <param name="feature1">GeoJSON Geometry</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <param name="precision">The precision used for comparing coordinates, in decimal digits. Default is 6.</param>
    /// <returns>true/false</returns>
    public static bool BooleanEqual(Geometry feature1, IFeature feature2, int precision = 6)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanEqual(feature1, feature2.Geometry, precision);
    }
}