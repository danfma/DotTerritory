namespace DotTerritory.Tests;

public class PointOnFeatureTests
{
    [Fact]
    public void TestPointOnFeature_Point_ReturnsSamePoint()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(10, 10));

        // Act
        var result = Territory.PointOnFeature(point);

        // Assert
        result.ShouldBe(point);
    }

    [Fact]
    public void TestPointOnFeature_LineString_ReturnsPointOnLine()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(10, 10) }
        );

        // Act
        var result = Territory.PointOnFeature(line);

        // Assert
        result.X.ShouldBeInRange(4.9, 5.1); // Should be around (5, 5)
        result.Y.ShouldBeInRange(4.9, 5.1);

        // Verify point is on the line
        line.Distance(result).ShouldBeLessThan(0.0001);
    }

    [Fact]
    public void TestPointOnFeature_Polygon_ReturnsPointInsidePolygon()
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
        result.X.ShouldBeInRange(4.9, 5.1);
        result.Y.ShouldBeInRange(4.9, 5.1);

        // Verify point is inside the polygon
        polygon.Contains(result).ShouldBeTrue();
    }

    [Fact]
    public void TestPointOnFeature_PolygonWithHole_ReturnsPointInsidePolygon()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();

        // Create shell coordinates
        var shellCoords = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(10, 0),
            new Coordinate(10, 10),
            new Coordinate(0, 10),
            new Coordinate(0, 0),
        };

        // Create hole coordinates
        var holeCoords = new[]
        {
            new Coordinate(4, 4),
            new Coordinate(6, 4),
            new Coordinate(6, 6),
            new Coordinate(4, 6),
            new Coordinate(4, 4),
        };

        var shell = geometryFactory.CreateLinearRing(shellCoords);
        var hole = geometryFactory.CreateLinearRing(holeCoords);
        var polygon = geometryFactory.CreatePolygon(shell, new[] { hole });

        // Act
        var result = Territory.PointOnFeature(polygon);

        // Assert
        // Verify point is inside the polygon (but not in the hole)
        polygon.Contains(result).ShouldBeTrue();
    }

    [Fact]
    public void TestPointOnFeature_MultiPoint_ReturnsFirstPoint()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var multiPoint = geometryFactory.CreateMultiPoint(
            new[] { new Coordinate(1, 1), new Coordinate(2, 2), new Coordinate(3, 3) }
        );

        // Act
        var result = Territory.PointOnFeature(multiPoint);

        // Assert
        result.X.ShouldBe(1);
        result.Y.ShouldBe(1);
    }

    [Fact]
    public void TestPointOnFeature_MultiLineString_ReturnsPointOnLongestLine()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();

        var line1 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(5, 5) }
        );

        var line2 = geometryFactory.CreateLineString(
            new[] { new Coordinate(10, 10), new Coordinate(20, 20) }
        );

        var multiLine = geometryFactory.CreateMultiLineString(new[] { line1, line2 });

        // Act
        var result = Territory.PointOnFeature(multiLine);

        // Assert
        // Should be on the longer line (line2)
        result.X.ShouldBeInRange(14.9, 15.1); // Should be around (15, 15)
        result.Y.ShouldBeInRange(14.9, 15.1);

        // Verify point is on one of the lines
        multiLine.Distance(result).ShouldBeLessThan(0.0001);
    }

    [Fact]
    public void TestPointOnFeature_MultiPolygon_ReturnsPointInLargestPolygon()
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
        result.X.ShouldBeInRange(14.9, 15.1); // Should be around (15, 15)
        result.Y.ShouldBeInRange(14.9, 15.1);

        // Verify point is in one of the polygons
        multiPoly.Contains(result).ShouldBeTrue();
    }
}
