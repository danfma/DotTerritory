using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a geometry or feature and returns the absolute centroid point of all coordinates.
    /// The centroid is simply the average of all coordinates, weighted by the number of occurrences.
    /// </summary>
    /// <param name="geometry">GeoJSON Geometry or Feature</param>
    /// <returns>The centroid point of the input geometry</returns>
    public static Point Centroid(Geometry geometry)
    {
        if (geometry == null)
            throw new ArgumentNullException(nameof(geometry), "Geometry is required");

        // TurfJS centroid is the arithmetic mean of all coordinates
        // This is different from NTS's centroid which is area-weighted
        double xSum = 0;
        double ySum = 0;
        int count = 0;
        
        // Use a coordinate sequence visitor pattern to enumerate all coordinates
        CoordinateSequenceVisitor(geometry, coord => {
            xSum += coord.X;
            ySum += coord.Y;
            count++;
        });
        
        if (count == 0)
            return new Point(0, 0); // Default for empty geometries
            
        return new Point(xSum / count, ySum / count);
    }
    
    private static void CoordinateSequenceVisitor(Geometry geometry, Action<Coordinate> action)
    {
        switch (geometry)
        {
            case Point point:
                action(point.Coordinate);
                break;
                
            case LineString lineString:
                foreach (var coord in lineString.Coordinates)
                    action(coord);
                break;
                
            case Polygon polygon:
                // Process exterior ring
                foreach (var coord in polygon.ExteriorRing.Coordinates)
                    action(coord);
                    
                // Process interior rings (holes)
                for (int i = 0; i < polygon.NumInteriorRings; i++)
                {
                    var hole = polygon.GetInteriorRingN(i);
                    foreach (var coord in hole.Coordinates)
                        action(coord);
                }
                break;
                
            case GeometryCollection collection:
                // Process all geometries in the collection
                foreach (var geom in collection.Geometries)
                    CoordinateSequenceVisitor(geom, action);
                break;
                
            default:
                // For any other geometry types, get all coordinates
                foreach (var coord in geometry.Coordinates)
                    action(coord);
                break;
        }
    }
    
    /// <summary>
    /// Takes a geometry or feature and returns the absolute centroid point of all coordinates.
    /// The centroid is simply the average of all coordinates, weighted by the number of occurrences.
    /// </summary>
    /// <param name="feature">GeoJSON Feature</param>
    /// <returns>The centroid point of the input feature</returns>
    public static Point Centroid(IFeature feature)
    {
        if (feature == null)
            throw new ArgumentNullException(nameof(feature), "Feature is required");

        return Centroid(feature.Geometry);
    }
    
    /// <summary>
    /// Takes a feature collection and returns the absolute centroid point of all coordinates.
    /// The centroid is simply the average of all coordinates, weighted by the number of occurrences.
    /// </summary>
    /// <param name="featureCollection">GeoJSON FeatureCollection</param>
    /// <returns>The centroid point of the input feature collection</returns>
    public static Point Centroid(FeatureCollection featureCollection)
    {
        if (featureCollection == null)
            throw new ArgumentNullException(nameof(featureCollection), "FeatureCollection is required");
            
        if (featureCollection.Count == 0)
            throw new ArgumentException("FeatureCollection is empty", nameof(featureCollection));
        
        // Calculate the centroid of all geometries combined
        var geometries = featureCollection.Select(f => f.Geometry).ToArray();
        var geometryCollection = NetTopologySuite.Geometries.GeometryFactory.Default.CreateGeometryCollection(geometries);
        
        return Centroid(geometryCollection);
    }
}