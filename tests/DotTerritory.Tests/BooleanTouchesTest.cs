using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanTouchesTest
{
    [Test]
    public async Task TouchingPolygonsShouldReturnTrue()
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
                new Coordinate(5, 0),
                new Coordinate(5, 5),
                new Coordinate(10, 5),
                new Coordinate(10, 0),
                new Coordinate(5, 0),
            ])
        );

        // Act
        var result = Territory.BooleanTouches(polygon1, polygon2);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task OverlappingPolygonsShouldReturnFalse()
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
        var result = Territory.BooleanTouches(polygon1, polygon2);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task DisjointPolygonsShouldReturnFalse()
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
        var result = Territory.BooleanTouches(polygon1, polygon2);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task PointTouchingPolygonBoundaryShouldReturnTrue()
    {
        // Arrange
        var point = new Point(5, 2.5);

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
        var result = Territory.BooleanTouches(point, polygon);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task PointInsidePolygonShouldReturnFalse()
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
        var result = Territory.BooleanTouches(point, polygon);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TouchingLineStringsShouldReturnTrue()
    {
        // Arrange
        var lineString1 = new LineString([new Coordinate(0, 0), new Coordinate(5, 5)]);

        var lineString2 = new LineString([new Coordinate(5, 5), new Coordinate(10, 10)]);

        // Act
        var result = Territory.BooleanTouches(lineString1, lineString2);

        // Assert
        await Assert.That(result).IsTrue();
    }
}
