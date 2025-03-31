namespace DotTerritory.Tests;

public class BBoxPolygonTest
{
    [Fact]
    public void ShouldMatchTurfBboxPolygon()
    {
        var bbox = new BBox(0, 0, 10, 10);

        // turf.bboxPolygon(bbox) => [ [ [ 0, 0 ], [ 10, 0 ], [ 10, 10 ], [ 0, 10 ], [ 0, 0 ] ] ]

        var polygon = Territory.BboxPolygon(bbox);

        polygon.Coordinates.Length.ShouldBe(5);

        polygon.Coordinates[0].X.ShouldBe(0);
        polygon.Coordinates[0].Y.ShouldBe(0);

        polygon.Coordinates[1].X.ShouldBe(10);
        polygon.Coordinates[1].Y.ShouldBe(0);

        polygon.Coordinates[2].X.ShouldBe(10);
        polygon.Coordinates[2].Y.ShouldBe(10);

        polygon.Coordinates[3].X.ShouldBe(0);
        polygon.Coordinates[3].Y.ShouldBe(10);

        polygon.Coordinates[4].X.ShouldBe(0);
        polygon.Coordinates[4].Y.ShouldBe(0);
    }
}
