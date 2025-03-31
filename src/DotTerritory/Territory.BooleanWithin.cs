using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Boolean-within returns true if the first geometry is completely within the second geometry.
    /// The interiors of both geometries must intersect and, the interior and boundary of the primary (geometry a)
    /// must not intersect the exterior of the secondary (geometry b).
    /// Boolean-within returns the exact opposite result of BooleanContains.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature or Geometry</param>
    /// <param name="feature2">GeoJSON Feature or Geometry</param>
    /// <returns>true/false</returns>
    public static bool BooleanWithin(Geometry feature1, Geometry feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        // Use NetTopologySuite's Within method which implements the same DE-9IM pattern
        // that Turf's BooleanWithin uses
        return feature1.Within(feature2);
    }

    /// <summary>
    /// Boolean-within returns true if the first geometry is completely within the second geometry.
    /// The interiors of both geometries must intersect and, the interior and boundary of the primary (geometry a)
    /// must not intersect the exterior of the secondary (geometry b).
    /// Boolean-within returns the exact opposite result of BooleanContains.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <returns>true/false</returns>
    public static bool BooleanWithin(IFeature feature1, IFeature feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanWithin(feature1.Geometry, feature2.Geometry);
    }

    /// <summary>
    /// Boolean-within returns true if the first geometry is completely within the second geometry.
    /// The interiors of both geometries must intersect and, the interior and boundary of the primary (geometry a)
    /// must not intersect the exterior of the secondary (geometry b).
    /// Boolean-within returns the exact opposite result of BooleanContains.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Geometry</param>
    /// <returns>true/false</returns>
    public static bool BooleanWithin(IFeature feature1, Geometry feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanWithin(feature1.Geometry, feature2);
    }

    /// <summary>
    /// Boolean-within returns true if the first geometry is completely within the second geometry.
    /// The interiors of both geometries must intersect and, the interior and boundary of the primary (geometry a)
    /// must not intersect the exterior of the secondary (geometry b).
    /// Boolean-within returns the exact opposite result of BooleanContains.
    /// </summary>
    /// <param name="feature1">GeoJSON Geometry</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <returns>true/false</returns>
    public static bool BooleanWithin(Geometry feature1, IFeature feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");
        
        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanWithin(feature1, feature2.Geometry);
    }
}