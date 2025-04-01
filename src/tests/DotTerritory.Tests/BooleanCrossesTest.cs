using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanCrossesTest
{
    [Fact]
    public void LineStringCrossingPolygonShouldReturnTrue()
    {
        // Arrange
        var lineString = new LineString([new Coordinate(-1, 2.5), new Coordinate(6, 2.5),]);

        var polygon = new Polygon(
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

        // Act
        var result = Territory.BooleanCrosses(lineString, polygon);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void LineStringWithinPolygonShouldReturnFalse()
    {
        // Arrange
        var lineString = new LineString([new Coordinate(1, 1), new Coordinate(4, 4),]);

        var polygon = new Polygon(
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

        // Act
        var result = Territory.BooleanCrosses(lineString, polygon);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void LineStringOutsidePolygonShouldReturnFalse()
    {
        // Arrange
        var lineString = new LineString([new Coordinate(6, 6), new Coordinate(10, 10),]);

        var polygon = new Polygon(
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

        // Act
        var result = Territory.BooleanCrosses(lineString, polygon);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IntersectingLineStringsShouldReturnTrue()
    {
        // Arrange
        var lineString1 = new LineString([new Coordinate(0, 0), new Coordinate(10, 10),]);

        var lineString2 = new LineString([new Coordinate(0, 10), new Coordinate(10, 0),]);

        // Act
        var result = Territory.BooleanCrosses(lineString1, lineString2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void NonIntersectingLineStringsShouldReturnFalse()
    {
        // Arrange
        var lineString1 = new LineString([new Coordinate(0, 0), new Coordinate(5, 5),]);

        var lineString2 = new LineString([new Coordinate(6, 6), new Coordinate(10, 10),]);

        // Act
        var result = Territory.BooleanCrosses(lineString1, lineString2);

        // Assert
        result.ShouldBeFalse();
    }
}
