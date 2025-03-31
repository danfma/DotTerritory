using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BBoxTest
{
    [Fact]
    public void ShouldMatchTurfBbox()
    {
        var line = new LineString(
            [new Coordinate(-74, 40), new Coordinate(-78, 42), new Coordinate(-82, 35)]
        );

        // turf.bbox(line) => [-82, 35, -74, 42]

        var bbox = Territory.Bbox(line);

        bbox.West.ShouldBe(-82);
        bbox.South.ShouldBe(35);
        bbox.East.ShouldBe(-74);
        bbox.North.ShouldBe(42);
    }
}
