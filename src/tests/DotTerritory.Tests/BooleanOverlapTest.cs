using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanOverlapTest
{
    [Fact]
    public void OverlappingPolygonsShouldReturnTrue()
    {
        // Arrange
        var polygon1 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 5),
                    new Coordinate(5, 5),
                    new Coordinate(5, 0),
                    new Coordinate(0, 0),
                ]
            )
        );

        var polygon2 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(3, 3),
                    new Coordinate(3, 8),
                    new Coordinate(8, 8),
                    new Coordinate(8, 3),
                    new Coordinate(3, 3),
                ]
            )
        );

        // Act
        var result = Territory.BooleanOverlap(polygon1, polygon2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void NonOverlappingPolygonsShouldReturnFalse()
    {
        // Arrange
        var polygon1 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 5),
                    new Coordinate(5, 5),
                    new Coordinate(5, 0),
                    new Coordinate(0, 0),
                ]
            )
        );

        var polygon2 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(6, 6),
                    new Coordinate(6, 11),
                    new Coordinate(11, 11),
                    new Coordinate(11, 6),
                    new Coordinate(6, 6),
                ]
            )
        );

        // Act
        var result = Territory.BooleanOverlap(polygon1, polygon2);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void ContainedPolygonShouldReturnFalse()
    {
        // Arrange
        var polygon1 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 10),
                    new Coordinate(10, 10),
                    new Coordinate(10, 0),
                    new Coordinate(0, 0),
                ]
            )
        );

        var polygon2 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(2, 2),
                    new Coordinate(2, 8),
                    new Coordinate(8, 8),
                    new Coordinate(8, 2),
                    new Coordinate(2, 2),
                ]
            )
        );

        // Act
        var result = Territory.BooleanOverlap(polygon1, polygon2);

        // Assert
        // BooleanOverlap returns false when one geometry completely contains the other
        result.ShouldBeFalse();
    }

    [Fact]
    public void OverlappingLineStringsShouldReturnTrue()
    {
        // Arrange
        var lineString1 = new LineString([new Coordinate(0, 0), new Coordinate(5, 5)]);

        var lineString2 = new LineString([new Coordinate(3, 3), new Coordinate(8, 8)]);

        // Act
        var result = Territory.BooleanOverlap(lineString1, lineString2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void NonOverlappingLineStringsShouldReturnFalse()
    {
        // Arrange
        var lineString1 = new LineString([new Coordinate(0, 0), new Coordinate(5, 5)]);

        var lineString2 = new LineString([new Coordinate(6, 6), new Coordinate(10, 10)]);

        // Act
        var result = Territory.BooleanOverlap(lineString1, lineString2);

        // Assert
        result.ShouldBeFalse();
    }
}
