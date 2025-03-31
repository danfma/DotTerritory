using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes two points and returns a point midway between them.
    /// The midpoint is calculated geodesically, meaning the curvature of the earth is taken into account.
    /// </summary>
    /// <param name="point1">First point</param>
    /// <param name="point2">Second point</param>
    /// <returns>A point midway between point1 and point2</returns>
    public static Point Midpoint(Point point1, Point point2)
    {
        if (point1 == null)
            throw new ArgumentNullException(nameof(point1), "First point is required");
        
        if (point2 == null)
            throw new ArgumentNullException(nameof(point2), "Second point is required");

        // Get coordinates from points
        var coord1 = point1.Coordinate;
        var coord2 = point2.Coordinate;

        // Calculate distance and bearing between the two points
        var dist = Distance(coord1, coord2);
        var bearing = Bearing(coord1, coord2);
        
        // Calculate the destination point that is halfway along the distance in the direction of the bearing
        return Destination(coord1, dist / 2, bearing);
    }
    
    /// <summary>
    /// Takes two point features and returns a point midway between them.
    /// The midpoint is calculated geodesically, meaning the curvature of the earth is taken into account.
    /// </summary>
    /// <param name="point1">First point feature</param>
    /// <param name="point2">Second point feature</param>
    /// <returns>A point midway between point1 and point2</returns>
    public static Point Midpoint(IFeature point1, IFeature point2)
    {
        if (point1 == null)
            throw new ArgumentNullException(nameof(point1), "First point feature is required");
        
        if (point2 == null)
            throw new ArgumentNullException(nameof(point2), "Second point feature is required");
            
        if (!(point1.Geometry is Point p1))
            throw new ArgumentException("First geometry must be a Point", nameof(point1));
            
        if (!(point2.Geometry is Point p2))
            throw new ArgumentException("Second geometry must be a Point", nameof(point2));

        return Midpoint(p1, p2);
    }
    
    /// <summary>
    /// Takes a point and a point feature and returns a point midway between them.
    /// The midpoint is calculated geodesically, meaning the curvature of the earth is taken into account.
    /// </summary>
    /// <param name="point1">First point</param>
    /// <param name="point2">Second point feature</param>
    /// <returns>A point midway between point1 and point2</returns>
    public static Point Midpoint(Point point1, IFeature point2)
    {
        if (point1 == null)
            throw new ArgumentNullException(nameof(point1), "First point is required");
        
        if (point2 == null)
            throw new ArgumentNullException(nameof(point2), "Second point feature is required");
            
        if (!(point2.Geometry is Point p2))
            throw new ArgumentException("Second geometry must be a Point", nameof(point2));

        return Midpoint(point1, p2);
    }
    
    /// <summary>
    /// Takes a point feature and a point and returns a point midway between them.
    /// The midpoint is calculated geodesically, meaning the curvature of the earth is taken into account.
    /// </summary>
    /// <param name="point1">First point feature</param>
    /// <param name="point2">Second point</param>
    /// <returns>A point midway between point1 and point2</returns>
    public static Point Midpoint(IFeature point1, Point point2)
    {
        if (point1 == null)
            throw new ArgumentNullException(nameof(point1), "First point feature is required");
        
        if (point2 == null)
            throw new ArgumentNullException(nameof(point2), "Second point is required");
            
        if (!(point1.Geometry is Point p1))
            throw new ArgumentException("First geometry must be a Point", nameof(point1));

        return Midpoint(p1, point2);
    }
}