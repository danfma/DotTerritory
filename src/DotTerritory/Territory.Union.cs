// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.Union.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes two or more polygons and returns a combined polygon.
    /// If the input polygons are not contiguous, this function returns a MultiPolygon.
    /// </summary>
    /// <param name="polygon1">First polygon</param>
    /// <param name="polygon2">Second polygon</param>
    /// <returns>Combined polygon or multipolygon</returns>
    public static Geometry Union(Polygon polygon1, Polygon polygon2)
    {
        return polygon1.Union(polygon2);
    }

    /// <summary>
    /// Takes an array of polygons and returns a combined polygon.
    /// If the input polygons are not contiguous, this function returns a MultiPolygon.
    /// </summary>
    /// <param name="polygons">Array of polygons</param>
    /// <returns>Combined polygon or multipolygon</returns>
    public static Geometry Union(Polygon[] polygons)
    {
        if (polygons == null || polygons.Length == 0)
        {
            throw new ArgumentException("At least one polygon must be provided", nameof(polygons));
        }

        if (polygons.Length == 1)
        {
            return polygons[0];
        }

        // Start with the first polygon
        Geometry result = polygons[0];

        // Union with each subsequent polygon
        for (int i = 1; i < polygons.Length; i++)
        {
            result = result.Union(polygons[i]);
        }

        return result;
    }

    /// <summary>
    /// Takes an array of geometries and returns their union.
    /// </summary>
    /// <param name="geometries">Array of geometries</param>
    /// <returns>Combined geometry</returns>
    public static Geometry Union(Geometry[] geometries)
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

        // Union with each subsequent geometry
        for (int i = 1; i < geometries.Length; i++)
        {
            result = result.Union(geometries[i]);
        }

        return result;
    }

    /// <summary>
    /// Takes a feature collection and returns the union of all geometries.
    /// </summary>
    /// <param name="featureCollection">FeatureCollection containing geometries</param>
    /// <returns>Combined geometry</returns>
    internal static Geometry Union(IEnumerable<IFeature> featureCollection)
    {
        var features = featureCollection?.ToList() ?? new List<IFeature>();
        if (features.Count == 0)
        {
            throw new ArgumentException(
                "FeatureCollection must contain at least one feature",
                nameof(featureCollection)
            );
        }

        // Extract geometries from features
        var geometries = new List<Geometry>();
        foreach (var feature in features)
        {
            if (feature.Geometry != null)
            {
                geometries.Add(feature.Geometry);
            }
        }

        return Union(geometries.ToArray());
    }
}
