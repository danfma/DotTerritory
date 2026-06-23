namespace DotTerritory.Tests;

public class BBoxTest
{
    [Test]
    public async Task ShouldMatchTurfBbox()
    {
        var line = new LineString([
            new Coordinate(-74, 40),
            new Coordinate(-78, 42),
            new Coordinate(-82, 35),
        ]);

        // turf.bbox(line) => [-82, 35, -74, 42]

        var bbox = Territory.Bbox(line);

        await Assert.That(bbox.West).IsEqualTo(-82);
        await Assert.That(bbox.South).IsEqualTo(35);
        await Assert.That(bbox.East).IsEqualTo(-74);
        await Assert.That(bbox.North).IsEqualTo(42);
    }
}
