using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanIntersectsTest
{
    [Test]
    public async Task IntersectingPolygonsShouldReturnTrue()
    {
        // Arrange
        var polygon1 = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        var polygon2 = new Polygon(
            new LinearRing([
                new Coordinate(3, 3),
                new Coordinate(3, 8),
                new Coordinate(8, 8),
                new Coordinate(8, 3),
                new Coordinate(3, 3),
            ])
        );

        // Act
        var result = Territory.BooleanIntersects(polygon1, polygon2);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task NonIntersectingPolygonsShouldReturnFalse()
    {
        // Arrange
        var polygon1 = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        var polygon2 = new Polygon(
            new LinearRing([
                new Coordinate(6, 6),
                new Coordinate(6, 11),
                new Coordinate(11, 11),
                new Coordinate(11, 6),
                new Coordinate(6, 6),
            ])
        );

        // Act
        var result = Territory.BooleanIntersects(polygon1, polygon2);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task PointInPolygonShouldReturnTrue()
    {
        // Arrange
        var point = new Point(2.5, 2.5);

        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        // Act
        var result = Territory.BooleanIntersects(point, polygon);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task PointOutsidePolygonShouldReturnFalse()
    {
        // Arrange
        var point = new Point(10, 10);

        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        // Act
        var result = Territory.BooleanIntersects(point, polygon);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IntersectingLineStringsShouldReturnTrue()
    {
        // Arrange
        var lineString1 = new LineString([new Coordinate(0, 0), new Coordinate(10, 10)]);

        var lineString2 = new LineString([new Coordinate(0, 10), new Coordinate(10, 0)]);

        // Act
        var result = Territory.BooleanIntersects(lineString1, lineString2);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task NonIntersectingLineStringsShouldReturnFalse()
    {
        // Arrange
        var lineString1 = new LineString([new Coordinate(0, 0), new Coordinate(5, 5)]);

        var lineString2 = new LineString([new Coordinate(6, 6), new Coordinate(10, 10)]);

        // Act
        var result = Territory.BooleanIntersects(lineString1, lineString2);

        // Assert
        await Assert.That(result).IsFalse();
    }
}
