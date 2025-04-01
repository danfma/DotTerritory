namespace DotTerritory.Tests;

public class ExplodeTests
{
    [Fact]
    public void TestExplode_Point_ReturnsSinglePointCollection()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(1, 2));

        // Act
        var result = Territory.Explode(point);

        // Assert
        result.NumGeometries.ShouldBe(1);
        result.GetGeometryN(0).ShouldBeOfType<Point>();
        result.GetGeometryN(0).Coordinate.ShouldBe(new Coordinate(1, 2));
    }

    [Fact]
    public void TestExplode_LineString_ReturnsAllVerticesAsPoints()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line = geometryFactory.CreateLineString(
            [new Coordinate(0, 0), new Coordinate(1, 1), new Coordinate(2, 2)]
        );

        // Act
        var result = Territory.Explode(line);

        // Assert
        result.NumGeometries.ShouldBe(3);

        for (int i = 0; i < result.NumGeometries; i++)
        {
            var geometry = result.GetGeometryN(i);
            geometry.ShouldBeOfType<Point>();
            geometry.Coordinate.ShouldBe(line.GetCoordinateN(i));
        }
    }

    [Fact]
    public void TestExplode_Polygon_ReturnsAllVerticesAsPoints()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var polygon = geometryFactory.CreatePolygon(
            [
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0)
            ]
        );

        // Act
        var result = Territory.Explode(polygon);

        // Assert
        result.NumGeometries.ShouldBe(5);

        for (int i = 0; i < result.NumGeometries; i++)
        {
            var geometry = result.GetGeometryN(i);
            geometry.ShouldBeOfType<Point>();
            geometry.Coordinate.ShouldBe(polygon.GetCoordinateN(i));
        }
    }

    [Fact]
    public void TestExplode_GeometryCollection_ReturnsAllVerticesAsPoints()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(0, 0));
        var line = geometryFactory.CreateLineString([new Coordinate(1, 1), new Coordinate(2, 2)]);
        var collection = geometryFactory.CreateGeometryCollection([point, line]);

        // Act
        var result = Territory.Explode(collection);

        // Assert
        result.NumGeometries.ShouldBe(3); // 1 point + 2 points from the line

        // First point from the point geometry
        result.GetGeometryN(0).ShouldBeOfType<Point>();
        result.GetGeometryN(0).Coordinate.ShouldBe(new Coordinate(0, 0));

        // Points from the line
        result.GetGeometryN(1).ShouldBeOfType<Point>();
        result.GetGeometryN(1).Coordinate.ShouldBe(new Coordinate(1, 1));

        result.GetGeometryN(2).ShouldBeOfType<Point>();
        result.GetGeometryN(2).Coordinate.ShouldBe(new Coordinate(2, 2));
    }

    [Fact]
    public void TestExplode_EmptyGeometry_ReturnsEmptyCollection()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var emptyPoint = geometryFactory.CreatePoint();

        // Act
        var result = Territory.Explode(emptyPoint);

        // Assert
        result.NumGeometries.ShouldBe(0);
        result.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void TestExplode_MultiPolygon_ReturnsAllVerticesAsPoints()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();

        var poly1 = geometryFactory.CreatePolygon(
            [
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0)
            ]
        );

        var poly2 = geometryFactory.CreatePolygon(
            [
                new Coordinate(2, 2),
                new Coordinate(3, 2),
                new Coordinate(3, 3),
                new Coordinate(2, 3),
                new Coordinate(2, 2)
            ]
        );

        var multiPolygon = geometryFactory.CreateMultiPolygon([poly1, poly2]);

        // Act
        var result = Territory.Explode(multiPolygon);

        // Assert
        result.NumGeometries.ShouldBe(10); // 5 points from poly1 + 5 points from poly2

        // Check the first few points
        result.GetGeometryN(0).ShouldBeOfType<Point>();
        result.GetGeometryN(0).Coordinate.ShouldBe(new Coordinate(0, 0));

        result.GetGeometryN(5).ShouldBeOfType<Point>();
        result.GetGeometryN(5).Coordinate.ShouldBe(new Coordinate(2, 2));
    }
}
