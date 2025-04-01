namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a Feature or FeatureCollection and returns a Point guaranteed to be on the surface of the feature.
    /// </summary>
    /// <param name="geometry">Feature or geometry to find a point on</param>
    /// <returns>A point on the surface of the input feature</returns>
    /// <example>
    /// <code>
    /// var polygon = GeometryFactory.CreatePolygon(new[] {
    ///     new Coordinate(-77, 39),
    ///     new Coordinate(-77, 38),
    ///     new Coordinate(-76, 38),
    ///     new Coordinate(-76, 39),
    ///     new Coordinate(-77, 39)
    /// });
    /// var pointOnFeature = Territory.PointOnFeature(polygon);
    /// </code>
    /// </example>
    public static Point PointOnFeature(Geometry geometry)
    {
        var geometryFactory = new GeometryFactory();

        // Check geometry type
        if (geometry is Point point)
        {
            return point;
        }

        if (geometry is LineString lineString)
        {
            // For LineString, find the midpoint of the line
            var lineLength = GetLength(lineString);
            return WalkAlong(lineString, lineLength * 0.5);
        }

        if (geometry is Polygon polygon)
        {
            // For Polygon, use the centroid if it's inside the polygon
            var centroid = polygon.Centroid;
            if (polygon.Contains(centroid))
            {
                return centroid;
            }

            // If centroid is outside polygon, find a point on the boundary
            if (polygon.Boundary is LineString boundary)
            {
                var boundaryLength = GetLength(boundary);
                return WalkAlong(boundary, boundaryLength * 0.5);
            }

            // If polygon boundary is a MultiLineString, use the first LineString
            if (polygon.Boundary is MultiLineString mlBoundary && mlBoundary.NumGeometries > 0)
            {
                var exteriorBoundary = (LineString)mlBoundary.GetGeometryN(0);
                var boundaryLength = GetLength(exteriorBoundary);
                return WalkAlong(exteriorBoundary, boundaryLength * 0.5);
            }
        }

        if (geometry is MultiPoint multiPoint && multiPoint.NumGeometries > 0)
        {
            // For MultiPoint, return the first point
            return (Point)multiPoint.GetGeometryN(0);
        }

        if (geometry is MultiLineString multiLineString && multiLineString.NumGeometries > 0)
        {
            // For MultiLineString, use the longest line
            LineString? longestLine = null;
            double maxLength = 0;

            for (int i = 0; i < multiLineString.NumGeometries; i++)
            {
                var line = (LineString)multiLineString.GetGeometryN(i);
                var length = GetLength(line).Meters;

                if (length > maxLength)
                {
                    maxLength = length;
                    longestLine = line;
                }
            }

            if (longestLine != null)
            {
                var lineLength = GetLength(longestLine);
                return WalkAlong(longestLine, lineLength * 0.5);
            }
        }

        if (geometry is MultiPolygon multiPolygon && multiPolygon.NumGeometries > 0)
        {
            // For MultiPolygon, find the polygon with the largest area
            Polygon? largestPolygon = null;
            double maxArea = 0;

            for (int i = 0; i < multiPolygon.NumGeometries; i++)
            {
                var poly = (Polygon)multiPolygon.GetGeometryN(i);
                var area = poly.Area;

                if (area > maxArea)
                {
                    maxArea = area;
                    largestPolygon = poly;
                }
            }

            if (largestPolygon != null)
            {
                return PointOnFeature(largestPolygon);
            }
        }

        if (geometry is GeometryCollection collection && collection.NumGeometries > 0)
        {
            // For GeometryCollection, recursively find a point on the first non-empty geometry
            for (int i = 0; i < collection.NumGeometries; i++)
            {
                var geom = collection.GetGeometryN(i);
                if (!geom.IsEmpty)
                {
                    return PointOnFeature(geom);
                }
            }
        }

        // Fallback: create a point at the center of the geometry's envelope
        var env = geometry.EnvelopeInternal;
        return geometryFactory.CreatePoint(
            new Coordinate((env.MinX + env.MaxX) / 2, (env.MinY + env.MaxY) / 2)
        );
    }
}
