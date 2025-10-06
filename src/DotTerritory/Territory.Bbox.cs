namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Calculates the bounding box of a feature collection
    /// </summary>
    internal static BBox Bbox(IEnumerable<IFeature> featureCollection)
    {
        var features = featureCollection.ToList();
        if (features.Count == 0)
        {
            return default;
        }

        // Start with the bbox of the first feature
        var result = Bbox(features[0]);

        // Combine with the remaining features
        for (int i = 1; i < features.Count; i++)
        {
            result += Bbox(features[i]);
        }

        return result;
    }

    /// <summary>
    /// Calculates the bounding box of a feature
    /// </summary>
    public static BBox Bbox(IFeature feature)
    {
        return Bbox(feature.Geometry);
    }

    /// <summary>
    /// Calculates the bounding box of a geometry
    /// </summary>
    public static BBox Bbox(Geometry geometry)
    {
        if (geometry.IsEmpty)
        {
            return default;
        }

        return geometry.Coordinates.Aggregate(
            new BBox(
                West: double.MaxValue,
                South: double.MaxValue,
                East: double.MinValue,
                North: double.MinValue
            ),
            (current, coordinate) =>
                new BBox(
                    West: Math.Min(current.West, coordinate.X),
                    South: Math.Min(current.South, coordinate.Y),
                    East: Math.Max(current.East, coordinate.X),
                    North: Math.Max(current.North, coordinate.Y)
                )
        );
    }
}
