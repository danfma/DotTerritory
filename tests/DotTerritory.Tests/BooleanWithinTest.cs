using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanWithinTest
{
    [Test]
    public async Task SmallerPolygonWithinLargerPolygonShouldReturnTrue()
    {
        // Arrange
        var smallPolygon = new Polygon(
            new LinearRing([
                new Coordinate(2, 2),
                new Coordinate(2, 4),
                new Coordinate(4, 4),
                new Coordinate(4, 2),
                new Coordinate(2, 2),
            ])
        );

        var largePolygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 10),
                new Coordinate(10, 10),
                new Coordinate(10, 0),
                new Coordinate(0, 0),
            ])
        );

        // Act
        var result = Territory.BooleanWithin(smallPolygon, largePolygon);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task LargerPolygonNotWithinSmallerPolygonShouldReturnFalse()
    {
        // Arrange
        var smallPolygon = new Polygon(
            new LinearRing([
                new Coordinate(2, 2),
                new Coordinate(2, 4),
                new Coordinate(4, 4),
                new Coordinate(4, 2),
                new Coordinate(2, 2),
            ])
        );

        var largePolygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 10),
                new Coordinate(10, 10),
                new Coordinate(10, 0),
                new Coordinate(0, 0),
            ])
        );

        // Act
        var result = Territory.BooleanWithin(largePolygon, smallPolygon);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task PointWithinPolygonShouldReturnTrue()
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
        var result = Territory.BooleanWithin(point, polygon);

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
        var result = Territory.BooleanWithin(point, polygon);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task LineStringWithinPolygonShouldReturnTrue()
    {
        // Arrange
        var lineString = new LineString([new Coordinate(1, 1), new Coordinate(4, 4)]);

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
        var result = Territory.BooleanWithin(lineString, polygon);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task LineStringPartiallyOutsidePolygonShouldReturnFalse()
    {
        // Arrange
        var lineString = new LineString([new Coordinate(1, 1), new Coordinate(10, 10)]);

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
        var result = Territory.BooleanWithin(lineString, polygon);

        // Assert
        await Assert.That(result).IsFalse();
    }
}
