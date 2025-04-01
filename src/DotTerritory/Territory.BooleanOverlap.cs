using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Boolean-overlap returns True if the geometries share some but not all points,
    /// and the intersection has the same dimension as the geometries themselves.
    ///
    /// Supported geometry pairs for a=feature1, b=feature2:
    /// - MultiPoint/MultiPoint
    /// - LineString/LineString
    /// - MultiLineString/MultiLineString
    /// - MultiLineString/LineString
    /// - Polygon/Polygon
    /// - MultiPolygon/MultiPolygon
    /// - MultiPolygon/Polygon
    /// </summary>
    /// <param name="feature1">GeoJSON Feature or Geometry</param>
    /// <param name="feature2">GeoJSON Feature or Geometry</param>
    /// <returns>true/false</returns>
    public static bool BooleanOverlap(Geometry feature1, Geometry feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        // If the geometries are exactly equal, they don't overlap
        if (BooleanEqual(feature1, feature2))
            return false;

        // Use NetTopologySuite's Overlaps method which implements the same DE-9IM pattern
        // that Turf's BooleanOverlap uses
        return feature1.Overlaps(feature2);
    }

    /// <summary>
    /// Boolean-overlap returns True if the geometries share some but not all points,
    /// and the intersection has the same dimension as the geometries themselves.
    ///
    /// Supported geometry pairs for a=feature1, b=feature2:
    /// - MultiPoint/MultiPoint
    /// - LineString/LineString
    /// - MultiLineString/MultiLineString
    /// - MultiLineString/LineString
    /// - Polygon/Polygon
    /// - MultiPolygon/MultiPolygon
    /// - MultiPolygon/Polygon
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <returns>true/false</returns>
    public static bool BooleanOverlap(IFeature feature1, IFeature feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanOverlap(feature1.Geometry, feature2.Geometry);
    }

    /// <summary>
    /// Boolean-overlap returns True if the geometries share some but not all points,
    /// and the intersection has the same dimension as the geometries themselves.
    ///
    /// Supported geometry pairs for a=feature1, b=feature2:
    /// - MultiPoint/MultiPoint
    /// - LineString/LineString
    /// - MultiLineString/MultiLineString
    /// - MultiLineString/LineString
    /// - Polygon/Polygon
    /// - MultiPolygon/MultiPolygon
    /// - MultiPolygon/Polygon
    /// </summary>
    /// <param name="feature1">GeoJSON Feature</param>
    /// <param name="feature2">GeoJSON Geometry</param>
    /// <returns>true/false</returns>
    public static bool BooleanOverlap(IFeature feature1, Geometry feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanOverlap(feature1.Geometry, feature2);
    }

    /// <summary>
    /// Boolean-overlap returns True if the geometries share some but not all points,
    /// and the intersection has the same dimension as the geometries themselves.
    ///
    /// Supported geometry pairs for a=feature1, b=feature2:
    /// - MultiPoint/MultiPoint
    /// - LineString/LineString
    /// - MultiLineString/MultiLineString
    /// - MultiLineString/LineString
    /// - Polygon/Polygon
    /// - MultiPolygon/MultiPolygon
    /// - MultiPolygon/Polygon
    /// </summary>
    /// <param name="feature1">GeoJSON Geometry</param>
    /// <param name="feature2">GeoJSON Feature</param>
    /// <returns>true/false</returns>
    public static bool BooleanOverlap(Geometry feature1, IFeature feature2)
    {
        if (feature1 == null)
            throw new ArgumentNullException(nameof(feature1), "Feature1 is required");

        if (feature2 == null)
            throw new ArgumentNullException(nameof(feature2), "Feature2 is required");

        return BooleanOverlap(feature1, feature2.Geometry);
    }
}
