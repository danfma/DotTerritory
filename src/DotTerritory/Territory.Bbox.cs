using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    public static BBox Bbox(FeatureCollection featureCollection)
    {
        return featureCollection.Aggregate(default(BBox), (bbox, feature) => bbox + Bbox(feature));
    }

    public static BBox Bbox(IFeature feature)
    {
        return Bbox(feature.Geometry);
    }

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
