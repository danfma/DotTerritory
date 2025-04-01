namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Boolean-intersects returns True if the geometries intersect.
    /// It is the inverse of BooleanDisjoint.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature or Geometry</param>
    /// <param name="feature2">GeoJSON Feature or Geometry</param>
    /// <param name="ignoreSelfIntersections">Whether to ignore any self-intersections within the geometries</param>
    /// <returns>true/false</returns>
    public static bool BooleanIntersects(
        Geometry feature1,
        Geometry feature2,
        bool ignoreSelfIntersections = true
    )
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        // BooleanIntersects is the inverse of BooleanDisjoint
        return !BooleanDisjoint(feature1, feature2, ignoreSelfIntersections);
    }

    /// <summary>
    /// Boolean-intersects returns True if the geometries intersect.
    /// It is the inverse of BooleanDisjoint.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <param name="ignoreSelfIntersections">Whether to ignore any self-intersections within the geometries</param>
    /// <returns>true/false</returns>
    public static bool BooleanIntersects(
        IFeature feature1,
        IFeature feature2,
        bool ignoreSelfIntersections = true
    )
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanIntersects(feature1.Geometry, feature2.Geometry, ignoreSelfIntersections);
    }

    /// <summary>
    /// Boolean-intersects returns True if the geometries intersect.
    /// It is the inverse of BooleanDisjoint.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Geometry</param>
    /// <param name="ignoreSelfIntersections">Whether to ignore any self-intersections within the geometries</param>
    /// <returns>true/false</returns>
    public static bool BooleanIntersects(
        IFeature feature1,
        Geometry feature2,
        bool ignoreSelfIntersections = true
    )
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanIntersects(feature1.Geometry, feature2, ignoreSelfIntersections);
    }

    /// <summary>
    /// Boolean-intersects returns True if the geometries intersect.
    /// It is the inverse of BooleanDisjoint.
    /// </summary>
    /// <param name="feature1">GeoJSON Geometry</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <param name="ignoreSelfIntersections">Whether to ignore any self-intersections within the geometries</param>
    /// <returns>true/false</returns>
    public static bool BooleanIntersects(
        Geometry feature1,
        IFeature feature2,
        bool ignoreSelfIntersections = true
    )
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanIntersects(feature1, feature2.Geometry, ignoreSelfIntersections);
    }
}
