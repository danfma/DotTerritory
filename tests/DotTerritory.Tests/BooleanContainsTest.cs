using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanContainsTest
{
    [Test]
    public async Task PolygonShouldContainPoint()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(1, 1),
                new Coordinate(1, 2),
                new Coordinate(2, 2),
                new Coordinate(2, 1),
                new Coordinate(1, 1),
            ])
        );

        var point = new Point(1.5, 1.5);

        // Act
        var result = Territory.BooleanContains(polygon, point);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task PolygonShouldNotContainOutsidePoint()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(1, 1),
                new Coordinate(1, 2),
                new Coordinate(2, 2),
                new Coordinate(2, 1),
                new Coordinate(1, 1),
            ])
        );

        var point = new Point(3, 3);

        // Act
        var result = Territory.BooleanContains(polygon, point);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task LargerPolygonShouldContainSmallerPolygon()
    {
        // Arrange
        var largePolygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 10),
                new Coordinate(10, 10),
                new Coordinate(10, 0),
                new Coordinate(0, 0),
            ])
        );

        var smallPolygon = new Polygon(
            new LinearRing([
                new Coordinate(2, 2),
                new Coordinate(2, 4),
                new Coordinate(4, 4),
                new Coordinate(4, 2),
                new Coordinate(2, 2),
            ])
        );

        // Act
        var result = Territory.BooleanContains(largePolygon, smallPolygon);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task PolygonWithHoleShouldNotContainPointInHole()
    {
        // Arrange
        var exteriorRing = new LinearRing([
            new Coordinate(0, 0),
            new Coordinate(0, 10),
            new Coordinate(10, 10),
            new Coordinate(10, 0),
            new Coordinate(0, 0),
        ]);

        var interiorRing = new LinearRing([
            new Coordinate(2, 2),
            new Coordinate(2, 4),
            new Coordinate(4, 4),
            new Coordinate(4, 2),
            new Coordinate(2, 2),
        ]);

        var polygon = new Polygon(exteriorRing, [interiorRing]);
        var point = new Point(3, 3);

        // Act
        var result = Territory.BooleanContains(polygon, point);

        // Assert
        await Assert.That(result).IsFalse();
    }
}
