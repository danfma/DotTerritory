// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.Convex.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Calculates the convex hull of a geometry or a collection of geometries.
    /// The convex hull is the smallest convex polygon that contains all the points in the input geometry.
    /// </summary>
    /// <param name="geometry">The input geometry or geometry collection</param>
    /// <returns>A convex polygon</returns>
    public static Polygon Convex(Geometry geometry)
    {
        // NetTopologySuite's ConvexHull implementation
        return (Polygon)geometry.ConvexHull();
    }

    /// <summary>
    /// Calculates the convex hull of a set of points.
    /// The convex hull is the smallest convex polygon that contains all the points.
    /// </summary>
    /// <param name="points">Array of points</param>
    /// <returns>A convex polygon</returns>
    public static Polygon Convex(Point[] points)
    {
        if (points.Length == 0)
        {
            throw new ArgumentException("No points provided for convex hull calculation");
        }

        // Create a MultiPoint from the array of points
        var multiPoint = new MultiPoint(points);

        // Calculate the convex hull
        return (Polygon)multiPoint.ConvexHull();
    }

    /// <summary>
    /// Calculates the convex hull of a set of coordinates.
    /// The convex hull is the smallest convex polygon that contains all the points.
    /// </summary>
    /// <param name="coordinates">Array of coordinates</param>
    /// <returns>A convex polygon</returns>
    public static Polygon Convex(Coordinate[] coordinates)
    {
        if (coordinates.Length == 0)
        {
            throw new ArgumentException("No coordinates provided for convex hull calculation");
        }

        // Create an array of Points from the coordinates
        var points = new Point[coordinates.Length];
        for (var i = 0; i < coordinates.Length; i++)
        {
            points[i] = new Point(coordinates[i]);
        }

        return Convex(points);
    }
}
