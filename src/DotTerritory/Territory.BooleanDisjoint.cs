using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Boolean-disjoint returns True if the intersection of the two geometries is an empty set.
    /// That is, the geometries do not share any space together.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature or Geometry</param>
    /// <param name="feature2">GeoJSON Feature or Geometry</param>
    /// <param name="ignoreSelfIntersections">Whether to ignore any self-intersections within the geometries</param>
    /// <returns>true/false</returns>
    public static bool BooleanDisjoint(Geometry feature1, Geometry feature2, bool ignoreSelfIntersections = true)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        // Use NetTopologySuite's Disjoint method which implements the same DE-9IM pattern
        // that Turf's BooleanDisjoint uses
        return feature1.Disjoint(feature2);
    }

    /// <summary>
    /// Boolean-disjoint returns True if the intersection of the two geometries is an empty set.
    /// That is, the geometries do not share any space together.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <param name="ignoreSelfIntersections">Whether to ignore any self-intersections within the geometries</param>
    /// <returns>true/false</returns>
    public static bool BooleanDisjoint(IFeature feature1, IFeature feature2, bool ignoreSelfIntersections = true)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanDisjoint(feature1.Geometry, feature2.Geometry, ignoreSelfIntersections);
    }

    /// <summary>
    /// Boolean-disjoint returns True if the intersection of the two geometries is an empty set.
    /// That is, the geometries do not share any space together.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Geometry</param>
    /// <param name="ignoreSelfIntersections">Whether to ignore any self-intersections within the geometries</param>
    /// <returns>true/false</returns>
    public static bool BooleanDisjoint(IFeature feature1, Geometry feature2, bool ignoreSelfIntersections = true)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanDisjoint(feature1.Geometry, feature2, ignoreSelfIntersections);
    }

    /// <summary>
    /// Boolean-disjoint returns True if the intersection of the two geometries is an empty set.
    /// That is, the geometries do not share any space together.
    /// </summary>
    /// <param name="feature1">GeoJSON Geometry</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <param name="ignoreSelfIntersections">Whether to ignore any self-intersections within the geometries</param>
    /// <returns>true/false</returns>
    public static bool BooleanDisjoint(Geometry feature1, IFeature feature2, bool ignoreSelfIntersections = true)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanDisjoint(feature1, feature2.Geometry, ignoreSelfIntersections);
    }
}