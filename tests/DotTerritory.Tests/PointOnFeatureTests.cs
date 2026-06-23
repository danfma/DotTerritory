namespace DotTerritory.Tests;

public class PointOnFeatureTests
{
    [Test]
    public async Task TestPointOnFeature_Point_ReturnsSamePoint()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(10, 10));

        // Act
        var result = Territory.PointOnFeature(point);

        // Assert
        await Assert.That(result).IsEqualTo(point);
    }

    [Test]
    public async Task TestPointOnFeature_Polygon_ReturnsPointInsidePolygon()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var polygon = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(0, 0),
                new Coordinate(10, 0),
                new Coordinate(10, 10),
                new Coordinate(0, 10),
                new Coordinate(0, 0),
            }
        );

        // Act
        var result = Territory.PointOnFeature(polygon);

        // Assert
        // Should be at or near the centroid (5, 5)
        await Assert.That(result.X).IsBetween(4.9, 5.1);
        await Assert.That(result.Y).IsBetween(4.9, 5.1);

        // Verify point is inside the polygon
        await Assert.That(polygon.Contains(result)).IsTrue();
    }

    [Test]
    public async Task TestPointOnFeature_MultiPoint_ReturnsFirstPoint()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var multiPoint = geometryFactory.CreateMultiPointFromCoords(
            new[] { new Coordinate(1, 1), new Coordinate(2, 2), new Coordinate(3, 3) }
        );

        // Act
        var result = Territory.PointOnFeature(multiPoint);

        // Assert
        await Assert.That(result.X).IsEqualTo(1);
        await Assert.That(result.Y).IsEqualTo(1);
    }

    [Test]
    public async Task TestPointOnFeature_MultiPolygon_ReturnsPointInLargestPolygon()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();

        var poly1 = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(0, 0),
                new Coordinate(5, 0),
                new Coordinate(5, 5),
                new Coordinate(0, 5),
                new Coordinate(0, 0),
            }
        );

        var poly2 = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(10, 10),
                new Coordinate(20, 10),
                new Coordinate(20, 20),
                new Coordinate(10, 20),
                new Coordinate(10, 10),
            }
        );

        var multiPoly = geometryFactory.CreateMultiPolygon(new[] { poly1, poly2 });

        // Act
        var result = Territory.PointOnFeature(multiPoly);

        // Assert
        // Should be in the larger polygon (poly2)
        await Assert.That(result.X).IsBetween(14.9, 15.1); // Should be around (15, 15)
        await Assert.That(result.Y).IsBetween(14.9, 15.1);

        // Verify point is in one of the polygons
        await Assert.That(multiPoly.Contains(result)).IsTrue();
    }
}
