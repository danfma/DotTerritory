namespace DotTerritory.Tests;

public class BBoxPolygonTest
{
    [Test]
    public async Task ShouldMatchTurfBboxPolygon()
    {
        var bbox = new BBox(0, 0, 10, 10);

        // turf.bboxPolygon(bbox) => [ [ [ 0, 0 ], [ 10, 0 ], [ 10, 10 ], [ 0, 10 ], [ 0, 0 ] ] ]

        var polygon = Territory.BboxPolygon(bbox);

        await Assert.That(polygon.Coordinates.Length).IsEqualTo(5);

        await Assert.That(polygon.Coordinates[0].X).IsEqualTo(0);
        await Assert.That(polygon.Coordinates[0].Y).IsEqualTo(0);

        await Assert.That(polygon.Coordinates[1].X).IsEqualTo(10);
        await Assert.That(polygon.Coordinates[1].Y).IsEqualTo(0);

        await Assert.That(polygon.Coordinates[2].X).IsEqualTo(10);
        await Assert.That(polygon.Coordinates[2].Y).IsEqualTo(10);

        await Assert.That(polygon.Coordinates[3].X).IsEqualTo(0);
        await Assert.That(polygon.Coordinates[3].Y).IsEqualTo(10);

        await Assert.That(polygon.Coordinates[4].X).IsEqualTo(0);
        await Assert.That(polygon.Coordinates[4].Y).IsEqualTo(0);
    }
}
