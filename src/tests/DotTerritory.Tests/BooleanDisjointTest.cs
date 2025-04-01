using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanDisjointTest
{
    [Fact]
    public void DisjointPolygonsShouldReturnTrue()
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
                    new Coordinate(10, 10),
                    new Coordinate(10, 15),
                    new Coordinate(15, 15),
                    new Coordinate(15, 10),
                    new Coordinate(10, 10),
                ]
            )
        );

        // Act
        var result = Territory.BooleanDisjoint(polygon1, polygon2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void OverlappingPolygonsShouldReturnFalse()
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
        var result = Territory.BooleanDisjoint(polygon1, polygon2);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void DisjointPointsAndPolygonShouldReturnTrue()
    {
        // Arrange
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

        var point = new Point(10, 10);

        // Act
        var result = Territory.BooleanDisjoint(polygon, point);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void PointInsidePolygonShouldReturnFalse()
    {
        // Arrange
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

        var point = new Point(2.5, 2.5);

        // Act
        var result = Territory.BooleanDisjoint(polygon, point);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void LineStringsWithoutIntersectionShouldReturnTrue()
    {
        // Arrange
        var lineString1 = new LineString([new Coordinate(0, 0), new Coordinate(5, 5)]);

        var lineString2 = new LineString([new Coordinate(10, 0), new Coordinate(15, 5)]);

        // Act
        var result = Territory.BooleanDisjoint(lineString1, lineString2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IntersectingLineStringsShouldReturnFalse()
    {
        // Arrange
        var lineString1 = new LineString([new Coordinate(0, 0), new Coordinate(5, 5)]);

        var lineString2 = new LineString([new Coordinate(0, 5), new Coordinate(5, 0)]);

        // Act
        var result = Territory.BooleanDisjoint(lineString1, lineString2);

        // Assert
        result.ShouldBeFalse();
    }
}
