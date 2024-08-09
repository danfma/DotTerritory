using FluentAssertions;
using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BBoxPolygonTest
{
    [Fact]
    public void ShouldMatchTurfBboxPolygon()
    {
        var bbox = new BBox(0, 0, 10, 10);

        // turf.bboxPolygon(bbox) => [ [ [ 0, 0 ], [ 10, 0 ], [ 10, 10 ], [ 0, 10 ], [ 0, 0 ] ] ]

        var polygon = Territory.BboxPolygon(bbox);

        polygon.Coordinates.Length.Should().Be(5);

        polygon.Coordinates[0].X.Should().Be(0);
        polygon.Coordinates[0].Y.Should().Be(0);

        polygon.Coordinates[1].X.Should().Be(10);
        polygon.Coordinates[1].Y.Should().Be(0);

        polygon.Coordinates[2].X.Should().Be(10);
        polygon.Coordinates[2].Y.Should().Be(10);

        polygon.Coordinates[3].X.Should().Be(0);
        polygon.Coordinates[3].Y.Should().Be(10);

        polygon.Coordinates[4].X.Should().Be(0);
        polygon.Coordinates[4].Y.Should().Be(0);
    }
}
