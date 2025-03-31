using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Executes an action on each feature in a feature collection.
    /// Similar to the meta.featureEach function in Turf.js.
    /// </summary>
    /// <param name="featureCollection">The feature collection to iterate over</param>
    /// <param name="action">The action to execute for each feature</param>
    public static void FeatureEach(FeatureCollection featureCollection, Action<IFeature, int> action)
    {
        if (featureCollection == null)
            throw new ArgumentNullException(nameof(featureCollection));
        
        if (action == null)
            throw new ArgumentNullException(nameof(action));
            
        for (int i = 0; i < featureCollection.Count; i++)
        {
            var feature = featureCollection[i];
            action(feature, i);
        }
    }

    /// <summary>
    /// Executes an action on each coordinate in a geometry.
    /// Similar to the meta.coordEach function in Turf.js.
    /// </summary>
    /// <param name="geometry">The geometry to iterate over</param>
    /// <param name="action">The action to execute for each coordinate</param>
    /// <param name="excludeWrapCoord">Whether to exclude the final coordinate of LinearRings that wraps back around to the first coordinate</param>
    public static void CoordEach(Geometry geometry, Action<Coordinate, int> action, bool excludeWrapCoord = false)
    {
        if (geometry == null)
            throw new ArgumentNullException(nameof(geometry));
        
        if (action == null)
            throw new ArgumentNullException(nameof(action));
        
        int coordIndex = 0;
        CoordinateSequenceVisitor(geometry, (coord, isWrapCoord) => {
            if (!isWrapCoord || !excludeWrapCoord)
            {
                action(coord, coordIndex);
                coordIndex++;
            }
        });
    }
    
    /// <summary>
    /// Executes an action on each coordinate in a feature.
    /// Similar to the meta.coordEach function in Turf.js.
    /// </summary>
    /// <param name="feature">The feature to iterate over</param>
    /// <param name="action">The action to execute for each coordinate</param>
    /// <param name="excludeWrapCoord">Whether to exclude the final coordinate of LinearRings that wraps back around to the first coordinate</param>
    public static void CoordEach(IFeature feature, Action<Coordinate, int> action, bool excludeWrapCoord = false)
    {
        if (feature == null)
            throw new ArgumentNullException(nameof(feature));
        
        CoordEach(feature.Geometry, action, excludeWrapCoord);
    }
    
    /// <summary>
    /// Executes an action on each coordinate in a feature collection.
    /// Similar to the meta.coordEach function in Turf.js.
    /// </summary>
    /// <param name="featureCollection">The feature collection to iterate over</param>
    /// <param name="action">The action to execute for each coordinate</param>
    /// <param name="excludeWrapCoord">Whether to exclude the final coordinate of LinearRings that wraps back around to the first coordinate</param>
    public static void CoordEach(FeatureCollection featureCollection, Action<Coordinate, int> action, bool excludeWrapCoord = false)
    {
        if (featureCollection == null)
            throw new ArgumentNullException(nameof(featureCollection));
        
        int coordIndex = 0;
        FeatureEach(featureCollection, (feature, _) => {
            CoordEach(feature.Geometry, (coord, _) => {
                action(coord, coordIndex);
                coordIndex++;
            }, excludeWrapCoord);
        });
    }
    
    private static void CoordinateSequenceVisitor(Geometry geometry, Action<Coordinate, bool> action)
    {
        switch (geometry)
        {
            case Point point:
                action(point.Coordinate, false);
                break;
                
            case LineString lineString:
                foreach (var coord in lineString.Coordinates)
                    action(coord, false);
                break;
                
            case Polygon polygon:
                // Process exterior ring
                var exteriorRing = polygon.ExteriorRing.Coordinates;
                for (int i = 0; i < exteriorRing.Length; i++)
                {
                    // Check if this is the last coordinate that wraps back to the first
                    bool isWrapCoord = (i == exteriorRing.Length - 1) && 
                                       (exteriorRing[i].X == exteriorRing[0].X && exteriorRing[i].Y == exteriorRing[0].Y);
                    action(exteriorRing[i], isWrapCoord);
                }
                
                // Process interior rings (holes)
                for (int r = 0; r < polygon.NumInteriorRings; r++)
                {
                    var ring = polygon.GetInteriorRingN(r).Coordinates;
                    for (int i = 0; i < ring.Length; i++)
                    {
                        // Check if this is the last coordinate that wraps back to the first
                        bool isWrapCoord = (i == ring.Length - 1) && 
                                          (ring[i].X == ring[0].X && ring[i].Y == ring[0].Y);
                        action(ring[i], isWrapCoord);
                    }
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
                    action(coord, false);
                break;
        }
    }
}