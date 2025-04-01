using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a reference point and a collection of points, and returns the point from the collection closest to the reference point.
    /// </summary>
    /// <param name="targetPoint">The reference point</param>
    /// <param name="points">A collection of points to find the nearest one from</param>
    /// <returns>The closest point from the collection to the reference point</returns>
    public static (Point Point, Length Distance, int Index) NearestPoint(
        Point targetPoint,
        params Point[] points
    )
    {
        if (targetPoint == null)
            throw new ArgumentNullException(nameof(targetPoint), "Target point is required");

        if (points == null || points.Length == 0)
            throw new ArgumentException("Points collection is empty or null", nameof(points));

        var targetCoord = targetPoint.Coordinate;
        Length minDistance = Length.FromKilometers(double.MaxValue);
        int nearestIndex = -1;
        Point nearestPoint = null;

        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i];
            if (point == null)
                continue;

            var dist = Distance(targetCoord, point.Coordinate);
            if (dist.Kilometers < minDistance.Kilometers)
            {
                minDistance = dist;
                nearestIndex = i;
                nearestPoint = point;
            }
        }

        if (nearestPoint == null)
            throw new ArgumentException("No valid points found in the collection", nameof(points));

        return (nearestPoint, minDistance, nearestIndex);
    }

    /// <summary>
    /// Takes a reference point and a feature collection of points, and returns the point feature from the collection closest to the reference point.
    /// </summary>
    /// <param name="targetPoint">The reference point</param>
    /// <param name="pointFeatures">A feature collection of points to find the nearest one from</param>
    /// <returns>The closest point feature from the collection to the reference point, along with the distance and index</returns>
    public static (IFeature Feature, Length Distance, int Index) NearestPoint(
        Point targetPoint,
        FeatureCollection pointFeatures
    )
    {
        if (targetPoint == null)
            throw new ArgumentNullException(nameof(targetPoint), "Target point is required");

        if (pointFeatures == null || pointFeatures.Count == 0)
            throw new ArgumentException(
                "Feature collection is empty or null",
                nameof(pointFeatures)
            );

        var targetCoord = targetPoint.Coordinate;
        Length minDistance = Length.FromKilometers(double.MaxValue);
        int nearestIndex = -1;
        IFeature nearestFeature = null;

        for (int i = 0; i < pointFeatures.Count; i++)
        {
            var feature = pointFeatures[i];
            if (feature?.Geometry is not Point point)
                continue;

            var dist = Distance(targetCoord, point.Coordinate);
            if (dist.Kilometers < minDistance.Kilometers)
            {
                minDistance = dist;
                nearestIndex = i;
                nearestFeature = feature;
            }
        }

        if (nearestFeature == null)
            throw new ArgumentException(
                "No valid point features found in the collection",
                nameof(pointFeatures)
            );

        return (nearestFeature, minDistance, nearestIndex);
    }

    /// <summary>
    /// Takes a reference point feature and a collection of points, and returns the point from the collection closest to the reference point.
    /// </summary>
    /// <param name="targetPointFeature">The reference point feature</param>
    /// <param name="points">A collection of points to find the nearest one from</param>
    /// <returns>The closest point from the collection to the reference point, along with the distance and index</returns>
    public static (Point Point, Length Distance, int Index) NearestPoint(
        IFeature targetPointFeature,
        params Point[] points
    )
    {
        if (targetPointFeature == null)
            throw new ArgumentNullException(
                nameof(targetPointFeature),
                "Target point feature is required"
            );

        if (!(targetPointFeature.Geometry is Point targetPoint))
            throw new ArgumentException(
                "Target feature must have a Point geometry",
                nameof(targetPointFeature)
            );

        return NearestPoint(targetPoint, points);
    }

    /// <summary>
    /// Takes a reference point feature and a feature collection of points, and returns the point feature from the collection closest to the reference point.
    /// </summary>
    /// <param name="targetPointFeature">The reference point feature</param>
    /// <param name="pointFeatures">A feature collection of points to find the nearest one from</param>
    /// <returns>The closest point feature from the collection to the reference point, along with the distance and index</returns>
    public static (IFeature Feature, Length Distance, int Index) NearestPoint(
        IFeature targetPointFeature,
        FeatureCollection pointFeatures
    )
    {
        if (targetPointFeature == null)
            throw new ArgumentNullException(
                nameof(targetPointFeature),
                "Target point feature is required"
            );

        if (!(targetPointFeature.Geometry is Point targetPoint))
            throw new ArgumentException(
                "Target feature must have a Point geometry",
                nameof(targetPointFeature)
            );

        return NearestPoint(targetPoint, pointFeatures);
    }
}
