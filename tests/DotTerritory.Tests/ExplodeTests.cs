namespace DotTerritory.Tests;

public class ExplodeTests
{
    [Test]
    public async Task TestExplode_Point_ReturnsSinglePointCollection()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(1, 2));

        // Act
        var result = Territory.Explode(point);

        // Assert
        await Assert.That(result.NumGeometries).IsEqualTo(1);
        await Assert.That(result.GetGeometryN(0)).IsTypeOf<Point>();
        await Assert.That(result.GetGeometryN(0).Coordinate).IsEqualTo(new Coordinate(1, 2));
    }

    [Test]
    public async Task TestExplode_LineString_ReturnsAllVerticesAsPoints()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line = geometryFactory.CreateLineString([
            new Coordinate(0, 0),
            new Coordinate(1, 1),
            new Coordinate(2, 2),
        ]);

        // Act
        var result = Territory.Explode(line);

        // Assert
        await Assert.That(result.NumGeometries).IsEqualTo(3);

        for (int i = 0; i < result.NumGeometries; i++)
        {
            var geometry = result.GetGeometryN(i);
            await Assert.That(geometry).IsTypeOf<Point>();
            await Assert.That(geometry.Coordinate).IsEqualTo(line.GetCoordinateN(i));
        }
    }

    [Test]
    public async Task TestExplode_Polygon_ReturnsAllVerticesAsPoints()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var polygon = geometryFactory.CreatePolygon([
            new Coordinate(0, 0),
            new Coordinate(1, 0),
            new Coordinate(1, 1),
            new Coordinate(0, 1),
            new Coordinate(0, 0),
        ]);

        // Act
        var result = Territory.Explode(polygon);

        // Assert
        await Assert.That(result.NumGeometries).IsEqualTo(5);

        for (int i = 0; i < result.NumGeometries; i++)
        {
            var geometry = result.GetGeometryN(i);
            await Assert.That(geometry).IsTypeOf<Point>();
            await Assert.That(geometry.Coordinate).IsEqualTo(polygon.Coordinates[i]);
        }
    }

    [Test]
    public async Task TestExplode_GeometryCollection_ReturnsAllVerticesAsPoints()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(0, 0));
        var line = geometryFactory.CreateLineString([new Coordinate(1, 1), new Coordinate(2, 2)]);
        var collection = geometryFactory.CreateGeometryCollection([point, line]);

        // Act
        var result = Territory.Explode(collection);

        // Assert
        await Assert.That(result.NumGeometries).IsEqualTo(3); // 1 point + 2 points from the line

        // First point from the point geometry
        await Assert.That(result.GetGeometryN(0)).IsTypeOf<Point>();
        await Assert.That(result.GetGeometryN(0).Coordinate).IsEqualTo(new Coordinate(0, 0));

        // Points from the line
        await Assert.That(result.GetGeometryN(1)).IsTypeOf<Point>();
        await Assert.That(result.GetGeometryN(1).Coordinate).IsEqualTo(new Coordinate(1, 1));

        await Assert.That(result.GetGeometryN(2)).IsTypeOf<Point>();
        await Assert.That(result.GetGeometryN(2).Coordinate).IsEqualTo(new Coordinate(2, 2));
    }

    [Test]
    public async Task TestExplode_EmptyGeometry_ReturnsEmptyCollection()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var emptyPoint = geometryFactory.CreatePoint();

        // Act
        var result = Territory.Explode(emptyPoint);

        // Assert
        await Assert.That(result.NumGeometries).IsEqualTo(0);
        await Assert.That(result.IsEmpty).IsTrue();
    }

    [Test]
    public async Task TestExplode_MultiPolygon_ReturnsAllVerticesAsPoints()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();

        var poly1 = geometryFactory.CreatePolygon([
            new Coordinate(0, 0),
            new Coordinate(1, 0),
            new Coordinate(1, 1),
            new Coordinate(0, 1),
            new Coordinate(0, 0),
        ]);

        var poly2 = geometryFactory.CreatePolygon([
            new Coordinate(2, 2),
            new Coordinate(3, 2),
            new Coordinate(3, 3),
            new Coordinate(2, 3),
            new Coordinate(2, 2),
        ]);

        var multiPolygon = geometryFactory.CreateMultiPolygon([poly1, poly2]);

        // Act
        var result = Territory.Explode(multiPolygon);

        // Assert
        await Assert.That(result.NumGeometries).IsEqualTo(10); // 5 points from poly1 + 5 points from poly2

        // Check the first few points
        await Assert.That(result.GetGeometryN(0)).IsTypeOf<Point>();
        await Assert.That(result.GetGeometryN(0).Coordinate).IsEqualTo(new Coordinate(0, 0));

        await Assert.That(result.GetGeometryN(5)).IsTypeOf<Point>();
        await Assert.That(result.GetGeometryN(5).Coordinate).IsEqualTo(new Coordinate(2, 2));
    }
}
