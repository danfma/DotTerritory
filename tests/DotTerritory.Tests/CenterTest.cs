using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class CenterTest
{
    [Test]
    public async Task PolygonCenterShouldBeCorrect()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 10),
                new Coordinate(10, 10),
                new Coordinate(10, 0),
                new Coordinate(0, 0),
            ])
        );

        // Act
        var center = Territory.Center(polygon);

        // Assert
        await Assert.That(center.X).IsEqualTo(5);
        await Assert.That(center.Y).IsEqualTo(5);
    }

    [Test]
    public async Task LineStringCenterShouldBeCorrect()
    {
        // Arrange
        var lineString = new LineString([new Coordinate(0, 0), new Coordinate(10, 10)]);

        // Act
        var center = Territory.Center(lineString);

        // Assert
        await Assert.That(center.X).IsEqualTo(5);
        await Assert.That(center.Y).IsEqualTo(5);
    }

    [Test]
    public async Task PointCenterShouldBeTheSamePoint()
    {
        // Arrange
        var point = new Point(5, 5);

        // Act
        var center = Territory.Center(point);

        // Assert
        await Assert.That(center.X).IsEqualTo(5);
        await Assert.That(center.Y).IsEqualTo(5);
    }

    [Test]
    public async Task MultiPointCenterShouldBeCorrect()
    {
        // Arrange
        var points = new MultiPoint([new Point(0, 0), new Point(10, 10)]);

        // Act
        var center = Territory.Center(points);

        // Assert
        await Assert.That(center.X).IsEqualTo(5);
        await Assert.That(center.Y).IsEqualTo(5);
    }

    [Test]
    public async Task IrregularPolygonCenterShouldBeCorrect()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(-10, -10),
                new Coordinate(-10, 10),
                new Coordinate(10, 10),
                new Coordinate(10, -10),
                new Coordinate(-10, -10),
            ])
        );

        // Act
        var center = Territory.Center(polygon);

        // Assert
        await Assert.That(center.X).IsEqualTo(0);
        await Assert.That(center.Y).IsEqualTo(0);
    }
}
