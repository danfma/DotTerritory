// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.Intersect.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Finds the intersection of two geometries.
    /// The intersection represents the parts of the geometries that overlap.
    /// </summary>
    /// <param name="geometry1">First geometry</param>
    /// <param name="geometry2">Second geometry</param>
    /// <returns>The intersection geometry</returns>
    public static Geometry Intersect(Geometry geometry1, Geometry geometry2)
    {
        return geometry1.Intersection(geometry2);
    }

    /// <summary>
    /// Finds the intersection of an array of geometries.
    /// The intersection represents the parts that are common to all input geometries.
    /// </summary>
    /// <param name="geometries">Array of geometries</param>
    /// <returns>The intersection geometry</returns>
    public static Geometry Intersect(Geometry[] geometries)
    {
        if (geometries == null || geometries.Length == 0)
        {
            throw new ArgumentException(
                "At least one geometry must be provided",
                nameof(geometries)
            );
        }

        if (geometries.Length == 1)
        {
            return geometries[0];
        }

        // Start with the first geometry
        Geometry result = geometries[0];

        // Intersect with each subsequent geometry
        for (int i = 1; i < geometries.Length; i++)
        {
            result = result.Intersection(geometries[i]);

            // If at any point we get an empty geometry, the final result will be empty
            if (result.IsEmpty)
            {
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Finds the intersection of two polygons.
    /// The intersection represents the parts of the polygons that overlap.
    /// </summary>
    /// <param name="polygon1">First polygon</param>
    /// <param name="polygon2">Second polygon</param>
    /// <returns>The intersection geometry</returns>
    public static Geometry Intersect(Polygon polygon1, Polygon polygon2)
    {
        return polygon1.Intersection(polygon2);
    }
}
