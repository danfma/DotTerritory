using FluentAssertions;
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

        bbox.West.Should().Be(-82);
        bbox.South.Should().Be(35);
        bbox.East.Should().Be(-74);
        bbox.North.Should().Be(42);
    }
}
