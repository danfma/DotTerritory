using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Boolean-contains returns True if the second geometry is completely contained by the first geometry.
    /// The interiors of both geometries must intersect and, the interior and boundary of the secondary (geometry b)
    /// must not intersect the exterior of the primary (geometry a).
    /// Boolean-contains returns the exact opposite result of BooleanWithin.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature or Geometry</param>
    /// <param name="feature2">GeoJSON Feature or Geometry</param>
    /// <returns>true/false</returns>
    public static bool BooleanContains(Geometry feature1, Geometry feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        // Use NetTopologySuite's Contains method which implements the same DE-9IM pattern
        // that Turf's BooleanContains uses
        return feature1.Contains(feature2);
    }

    /// <summary>
    /// Boolean-contains returns True if the second geometry is completely contained by the first geometry.
    /// The interiors of both geometries must intersect and, the interior and boundary of the secondary (geometry b)
    /// must not intersect the exterior of the primary (geometry a).
    /// Boolean-contains returns the exact opposite result of BooleanWithin.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <returns>true/false</returns>
    public static bool BooleanContains(IFeature feature1, IFeature feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanContains(feature1.Geometry, feature2.Geometry);
    }

    /// <summary>
    /// Boolean-contains returns True if the second geometry is completely contained by the first geometry.
    /// The interiors of both geometries must intersect and, the interior and boundary of the secondary (geometry b)
    /// must not intersect the exterior of the primary (geometry a).
    /// Boolean-contains returns the exact opposite result of BooleanWithin.
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Geometry</param>
    /// <returns>true/false</returns>
    public static bool BooleanContains(IFeature feature1, Geometry feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanContains(feature1.Geometry, feature2);
    }

    /// <summary>
    /// Boolean-contains returns True if the second geometry is completely contained by the first geometry.
    /// The interiors of both geometries must intersect and, the interior and boundary of the secondary (geometry b)
    /// must not intersect the exterior of the primary (geometry a).
    /// Boolean-contains returns the exact opposite result of BooleanWithin.
    /// </summary>
    /// <param name="feature1">GeoJSON Geometry</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <returns>true/false</returns>
    public static bool BooleanContains(Geometry feature1, IFeature feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanContains(feature1, feature2.Geometry);
    }
}
