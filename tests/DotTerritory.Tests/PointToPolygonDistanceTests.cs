namespace DotTerritory.Tests;

public class PointToPolygonDistanceTests
{
    [Test]
    public async Task TestPointToPolygonDistance_PointInsidePolygon_ReturnsZero()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(1.5, 1.5));
        var polygon = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(1, 1),
                new Coordinate(2, 1),
                new Coordinate(2, 2),
                new Coordinate(1, 2),
                new Coordinate(1, 1),
            }
        );

        // Act
        var distance = Territory.PointToPolygonDistance(point, polygon);

        // Assert
        await Assert.That(distance.Meters).IsEqualTo(0);
    }

    [Test]
    public async Task TestPointToPolygonDistance_PointOnBoundary_ReturnsZero()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(1, 1.5));
        var polygon = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(1, 1),
                new Coordinate(2, 1),
                new Coordinate(2, 2),
                new Coordinate(1, 2),
                new Coordinate(1, 1),
            }
        );

        // Act
        var distance = Territory.PointToPolygonDistance(point, polygon);

        // Assert
        await Assert.That(distance.Meters).IsEqualTo(0).Within(1e-10);
    }

    [Test]
    public async Task TestPointToPolygonDistance_PointInsideMultiPolygon_ReturnsZero()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(1.5, 1.5));

        var poly1 = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(1, 1),
                new Coordinate(2, 1),
                new Coordinate(2, 2),
                new Coordinate(1, 2),
                new Coordinate(1, 1),
            }
        );

        var poly2 = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(3, 3),
                new Coordinate(4, 3),
                new Coordinate(4, 4),
                new Coordinate(3, 4),
                new Coordinate(3, 3),
            }
        );

        var multiPolygon = geometryFactory.CreateMultiPolygon(new[] { poly1, poly2 });

        // Act
        var distance = Territory.PointToPolygonDistance(point, multiPolygon);

        // Assert
        await Assert.That(distance.Meters).IsEqualTo(0);
    }
}
