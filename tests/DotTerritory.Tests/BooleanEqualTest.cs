using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanEqualTest
{
    [Test]
    public async Task IdenticalPolygonsShouldBeEqual()
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
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        // Act
        var result = Territory.BooleanEqual(polygon1, polygon2);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task DifferentPolygonsShouldNotBeEqual()
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
                new Coordinate(1, 1),
                new Coordinate(1, 6),
                new Coordinate(6, 6),
                new Coordinate(6, 1),
                new Coordinate(1, 1),
            ])
        );

        // Act
        var result = Territory.BooleanEqual(polygon1, polygon2);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task SameGeometryWithSamePointOrderShouldBeEqual()
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
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        // Act
        var result = Territory.BooleanEqual(polygon1, polygon2);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task SamePolygonWithDifferentPointOrderShouldNotBeEqual()
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
                new Coordinate(0, 0),
                new Coordinate(5, 0),
                new Coordinate(5, 5),
                new Coordinate(0, 5),
                new Coordinate(0, 0),
            ])
        );

        // Act
        var result = Territory.BooleanEqual(polygon1, polygon2);

        // Assert
        // BooleanEqual uses EqualsExact, which requires the exact same coordinate order
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IdenticalPointsShouldBeEqual()
    {
        // Arrange
        var point1 = new Point(5, 5);
        var point2 = new Point(5, 5);

        // Act
        var result = Territory.BooleanEqual(point1, point2);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task DifferentPointsShouldNotBeEqual()
    {
        // Arrange
        var point1 = new Point(5, 5);
        var point2 = new Point(6, 6);

        // Act
        var result = Territory.BooleanEqual(point1, point2);

        // Assert
        await Assert.That(result).IsFalse();
    }
}
