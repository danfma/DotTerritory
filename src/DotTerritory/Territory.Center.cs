using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a geometry or feature and returns the absolute center point of its bounding box.
    /// The center is calculated as the midpoint of the horizontal and vertical extents.
    /// </summary>
    /// <param name="geometry">GeoJSON Geometry or Feature</param>
    /// <returns>The center point of the bounding box of the input geometry</returns>
    public static Point Center(Geometry geometry)
    {
        if (geometry == null)
            throw new ArgumentNullException(nameof(geometry), "Geometry is required");

        // Calculate the bounding box
        var bbox = Bbox(geometry);
        
        // Calculate the center of the bounding box
        var x = (bbox.West + bbox.East) / 2;
        var y = (bbox.South + bbox.North) / 2;
        
        return NetTopologySuite.Geometries.GeometryFactory.Default.CreatePoint(new Coordinate(x, y));
    }
    
    /// <summary>
    /// Takes a geometry or feature and returns the absolute center point of its bounding box.
    /// The center is calculated as the midpoint of the horizontal and vertical extents.
    /// </summary>
    /// <param name="feature">GeoJSON Feature</param>
    /// <returns>The center point of the bounding box of the input feature</returns>
    public static Point Center(IFeature feature)
    {
        if (feature == null)
            throw new ArgumentNullException(nameof(feature), "Feature is required");

        return Center(feature.Geometry);
    }
    
    /// <summary>
    /// Takes a feature collection and returns the absolute center point of its bounding box.
    /// The center is calculated as the midpoint of the horizontal and vertical extents.
    /// </summary>
    /// <param name="featureCollection">GeoJSON FeatureCollection</param>
    /// <returns>The center point of the bounding box of the input feature collection</returns>
    public static Point Center(FeatureCollection featureCollection)
    {
        if (featureCollection == null)
            throw new ArgumentNullException(nameof(featureCollection), "FeatureCollection is required");
            
        if (featureCollection.Count == 0)
            throw new ArgumentException("FeatureCollection is empty", nameof(featureCollection));
        
        // Calculate the bounding box of the feature collection
        var bbox = Bbox(featureCollection);
        
        // Calculate the center of the bounding box
        var x = (bbox.West + bbox.East) / 2;
        var y = (bbox.South + bbox.North) / 2;
        
        return NetTopologySuite.Geometries.GeometryFactory.Default.CreatePoint(new Coordinate(x, y));
    }
}