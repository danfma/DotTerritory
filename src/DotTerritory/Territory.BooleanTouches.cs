using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Boolean-touches returns true if the geometries have at least one point in common, but their interiors do not intersect.
    /// If the geometries have dimension 0, the function returns false.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature or Geometry</param>
    /// <param name="feature2">GeoJSON Feature or Geometry</param>
    /// <returns>true/false</returns>
    public static bool BooleanTouches(Geometry feature1, Geometry feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        // Use NetTopologySuite's Touches method which implements the same DE-9IM pattern
        // that Turf's BooleanTouches uses
        return feature1.Touches(feature2);
    }

    /// <summary>
    /// Boolean-touches returns true if the geometries have at least one point in common, but their interiors do not intersect.
    /// If the geometries have dimension 0, the function returns false.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <returns>true/false</returns>
    public static bool BooleanTouches(IFeature feature1, IFeature feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanTouches(feature1.Geometry, feature2.Geometry);
    }

    /// <summary>
    /// Boolean-touches returns true if the geometries have at least one point in common, but their interiors do not intersect.
    /// If the geometries have dimension 0, the function returns false.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Geometry</param>
    /// <returns>true/false</returns>
    public static bool BooleanTouches(IFeature feature1, Geometry feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanTouches(feature1.Geometry, feature2);
    }

    /// <summary>
    /// Boolean-touches returns true if the geometries have at least one point in common, but their interiors do not intersect.
    /// If the geometries have dimension 0, the function returns false.
    /// </summary>
    /// <param name="feature1">GeoJSON Geometry</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <returns>true/false</returns>
    public static bool BooleanTouches(Geometry feature1, IFeature feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanTouches(feature1, feature2.Geometry);
    }
}