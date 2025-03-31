using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a Point and a Polygon or MultiPolygon and determines if the point
    /// resides inside the polygon. The polygon can be convex or concave. The function accounts for holes.
    /// </summary>
    /// <param name="point">Input point</param>
    /// <param name="polygon">Input polygon or multipolygon</param>
    /// <param name="ignoreBoundary">True if polygon boundary should be ignored when determining if the point is inside the polygon</param>
    /// <returns>True if the Point is inside the Polygon; false if the Point is not inside the Polygon</returns>
    public static bool BooleanPointInPolygon(Point point, Polygon polygon, bool ignoreBoundary = false)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point), "Point is required");
        
        if (polygon == null)
            throw new ArgumentNullException(nameof(polygon), "Polygon is required");

        if (polygon.IsEmpty)
            return false;

        // Quick bounding box check
        var bbox = Bbox(polygon);
        if (!bbox.Contains(point.X, point.Y))
            return false;

        // Check if point is on boundary (if we care about boundaries)
        if (!ignoreBoundary && polygon.Boundary.Distance(point) < double.Epsilon)
            return true;

        // Otherwise, do the actual within check
        return polygon.Contains(point);
    }

    /// <summary>
    /// Takes a Point and a MultiPolygon and determines if the point
    /// resides inside the multipolygon. The multipolygon can be convex or concave. The function accounts for holes.
    /// </summary>
    /// <param name="point">Input point</param>
    /// <param name="multiPolygon">Input multipolygon</param>
    /// <param name="ignoreBoundary">True if polygon boundary should be ignored when determining if the point is inside the polygon</param>
    /// <returns>True if the Point is inside the MultiPolygon; false if the Point is not inside the MultiPolygon</returns>
    public static bool BooleanPointInPolygon(Point point, MultiPolygon multiPolygon, bool ignoreBoundary = false)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point), "Point is required");
        
        if (multiPolygon == null)
            throw new ArgumentNullException(nameof(multiPolygon), "MultiPolygon is required");

        if (multiPolygon.IsEmpty)
            return false;

        // Quick bounding box check
        var bbox = Bbox(multiPolygon);
        if (!bbox.Contains(point.X, point.Y))
            return false;

        // Check if point is on boundary (if we care about boundaries)
        if (!ignoreBoundary && multiPolygon.Boundary.Distance(point) < double.Epsilon)
            return true;

        // Check if point is inside any polygon
        foreach (var geom in multiPolygon.Geometries)
        {
            var polygon = (Polygon)geom;
            if (BooleanPointInPolygon(point, polygon, ignoreBoundary))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Takes a Point and a Polygon or MultiPolygon feature and determines if the point
    /// resides inside the polygon. The polygon can be convex or concave. The function accounts for holes.
    /// </summary>
    /// <param name="point">Input point</param>
    /// <param name="polygonFeature">Input polygon or multipolygon feature</param>
    /// <param name="ignoreBoundary">True if polygon boundary should be ignored when determining if the point is inside the polygon</param>
    /// <returns>True if the Point is inside the Polygon; false if the Point is not inside the Polygon</returns>
    public static bool BooleanPointInPolygon(Point point, IFeature polygonFeature, bool ignoreBoundary = false)
    {
        if (polygonFeature == null)
            throw new ArgumentNullException(nameof(polygonFeature), "Polygon feature is required");

        return polygonFeature.Geometry switch
        {
            Polygon polygon => BooleanPointInPolygon(point, polygon, ignoreBoundary),
            MultiPolygon multiPolygon => BooleanPointInPolygon(point, multiPolygon, ignoreBoundary),
            _ => throw new ArgumentException("Geometry must be a Polygon or MultiPolygon", nameof(polygonFeature))
        };
    }

    /// <summary>
    /// Takes a Point feature and a Polygon or MultiPolygon and determines if the point
    /// resides inside the polygon. The polygon can be convex or concave. The function accounts for holes.
    /// </summary>
    /// <param name="pointFeature">Input point feature</param>
    /// <param name="polygon">Input polygon or multipolygon</param>
    /// <param name="ignoreBoundary">True if polygon boundary should be ignored when determining if the point is inside the polygon</param>
    /// <returns>True if the Point is inside the Polygon; false if the Point is not inside the Polygon</returns>
    public static bool BooleanPointInPolygon(IFeature pointFeature, Geometry polygon, bool ignoreBoundary = false)
    {
        if (pointFeature == null)
            throw new ArgumentNullException(nameof(pointFeature), "Point feature is required");

        if (pointFeature.Geometry is not Point point)
            throw new ArgumentException("Geometry must be a Point", nameof(pointFeature));

        return polygon switch
        {
            Polygon p => BooleanPointInPolygon(point, p, ignoreBoundary),
            MultiPolygon mp => BooleanPointInPolygon(point, mp, ignoreBoundary),
            _ => throw new ArgumentException("Geometry must be a Polygon or MultiPolygon", nameof(polygon))
        };
    }

    /// <summary>
    /// Takes a Point feature and a Polygon or MultiPolygon feature and determines if the point
    /// resides inside the polygon. The polygon can be convex or concave. The function accounts for holes.
    /// </summary>
    /// <param name="pointFeature">Input point feature</param>
    /// <param name="polygonFeature">Input polygon or multipolygon feature</param>
    /// <param name="ignoreBoundary">True if polygon boundary should be ignored when determining if the point is inside the polygon</param>
    /// <returns>True if the Point is inside the Polygon; false if the Point is not inside the Polygon</returns>
    public static bool BooleanPointInPolygon(IFeature pointFeature, IFeature polygonFeature, bool ignoreBoundary = false)
    {
        if (pointFeature == null)
            throw new ArgumentNullException(nameof(pointFeature), "Point feature is required");

        if (pointFeature.Geometry is not Point point)
            throw new ArgumentException("Geometry must be a Point", nameof(pointFeature));

        return BooleanPointInPolygon(point, polygonFeature, ignoreBoundary);
    }
}