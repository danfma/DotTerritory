using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanCrossesTest
{
    [Test]
    public async Task LineStringCrossingPolygonShouldReturnTrue()
    {
        // Arrange
        var lineString = new LineString([new Coordinate(-1, 2.5), new Coordinate(6, 2.5)]);

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
        var result = Territory.BooleanCrosses(lineString, polygon);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task LineStringWithinPolygonShouldReturnFalse()
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
        var result = Territory.BooleanCrosses(lineString, polygon);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task LineStringOutsidePolygonShouldReturnFalse()
    {
        // Arrange
        var lineString = new LineString([new Coordinate(6, 6), new Coordinate(10, 10)]);

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
        var result = Territory.BooleanCrosses(lineString, polygon);

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
        var result = Territory.BooleanCrosses(lineString1, lineString2);

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
        var result = Territory.BooleanCrosses(lineString1, lineString2);

        // Assert
        await Assert.That(result).IsFalse();
    }
}
