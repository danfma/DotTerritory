using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Boolean-crosses returns true if the intersection of the two geometries results in a geometry
    /// whose dimension is less than the maximum dimension of the two input geometries and the 
    /// intersection set is interior to both geometries.
    /// 
    /// Supported geometry pairs for a=feature1, b=feature2:
    /// - MultiPoint/LineString
    /// - MultiPoint/Polygon
    /// - LineString/LineString
    /// - LineString/Polygon
    /// </summary>
    /// <param name="feature1">GeoJSON Feature or Geometry</param>
    /// <param name="feature2">GeoJSON Feature or Geometry</param>
    /// <returns>true/false</returns>
    public static bool BooleanCrosses(Geometry feature1, Geometry feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        // Use NetTopologySuite's Crosses method which implements the same DE-9IM pattern
        // that Turf's BooleanCrosses uses
        return feature1.Crosses(feature2);
    }

    /// <summary>
    /// Boolean-crosses returns true if the intersection of the two geometries results in a geometry
    /// whose dimension is less than the maximum dimension of the two input geometries and the 
    /// intersection set is interior to both geometries.
    /// 
    /// Supported geometry pairs for a=feature1, b=feature2:
    /// - MultiPoint/LineString
    /// - MultiPoint/Polygon
    /// - LineString/LineString
    /// - LineString/Polygon
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <returns>true/false</returns>
    public static bool BooleanCrosses(IFeature feature1, IFeature feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanCrosses(feature1.Geometry, feature2.Geometry);
    }

    /// <summary>
    /// Boolean-crosses returns true if the intersection of the two geometries results in a geometry
    /// whose dimension is less than the maximum dimension of the two input geometries and the 
    /// intersection set is interior to both geometries.
    /// 
    /// Supported geometry pairs for a=feature1, b=feature2:
    /// - MultiPoint/LineString
    /// - MultiPoint/Polygon
    /// - LineString/LineString
    /// - LineString/Polygon
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Geometry</param>
    /// <returns>true/false</returns>
    public static bool BooleanCrosses(IFeature feature1, Geometry feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanCrosses(feature1.Geometry, feature2);
    }

    /// <summary>
    /// Boolean-crosses returns true if the intersection of the two geometries results in a geometry
    /// whose dimension is less than the maximum dimension of the two input geometries and the 
    /// intersection set is interior to both geometries.
    /// 
    /// Supported geometry pairs for a=feature1, b=feature2:
    /// - MultiPoint/LineString
    /// - MultiPoint/Polygon
    /// - LineString/LineString
    /// - LineString/Polygon
    /// </summary>
    /// <param name="feature1">GeoJSON Geometry</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <returns>true/false</returns>
    public static bool BooleanCrosses(Geometry feature1, IFeature feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanCrosses(feature1, feature2.Geometry);
    }
}