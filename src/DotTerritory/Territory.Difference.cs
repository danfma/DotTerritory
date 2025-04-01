// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.Difference.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Finds the difference between two geometries.
    /// The difference represents the part of geometry1 that does not intersect with geometry2.
    /// </summary>
    /// <param name="geometry1">The base geometry</param>
    /// <param name="geometry2">The geometry to subtract from the base geometry</param>
    /// <returns>The difference geometry</returns>
    public static Geometry Difference(Geometry geometry1, Geometry geometry2)
    {
        return geometry1.Difference(geometry2);
    }

    /// <summary>
    /// Finds the difference between a geometry and an array of geometries.
    /// The difference represents the part of the original geometry that does not intersect with any of the geometries.
    /// </summary>
    /// <param name="geometry">The base geometry</param>
    /// <param name="geometries">An array of geometries to subtract from the base geometry</param>
    /// <returns>The difference geometry</returns>
    public static Geometry Difference(Geometry geometry, Geometry[] geometries)
    {
        if (geometries == null || geometries.Length == 0)
        {
            return geometry;
        }

        var result = geometry;

        // Apply the difference with each geometry in sequence
        foreach (var subtractionGeometry in geometries)
        {
            result = result.Difference(subtractionGeometry);
        }

        return result;
    }

    /// <summary>
    /// Finds the difference between a polygon and another polygon.
    /// The difference represents the part of polygon1 that does not intersect with polygon2.
    /// </summary>
    /// <param name="polygon1">The base polygon</param>
    /// <param name="polygon2">The polygon to subtract from the base polygon</param>
    /// <returns>The difference geometry</returns>
    public static Geometry Difference(Polygon polygon1, Polygon polygon2)
    {
        return polygon1.Difference(polygon2);
    }
}
