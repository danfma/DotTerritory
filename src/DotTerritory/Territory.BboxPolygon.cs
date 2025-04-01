namespace DotTerritory;

public static partial class Territory
{
    public static Polygon BboxPolygon(BBox bbox)
    {
        return new Polygon(
            new LinearRing(
                [
                    new Coordinate(bbox.West, bbox.South),
                    new Coordinate(bbox.East, bbox.South),
                    new Coordinate(bbox.East, bbox.North),
                    new Coordinate(bbox.West, bbox.North),
                    new Coordinate(bbox.West, bbox.South),
                ]
            )
        );
    }
}
