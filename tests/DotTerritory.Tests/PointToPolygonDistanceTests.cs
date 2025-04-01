namespace DotTerritory.Tests;

public class PointToPolygonDistanceTests
{
    [Fact]
    public void TestPointToPolygonDistance_PointInsidePolygon_ReturnsZero()
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
                new Coordinate(1, 1)
            }
        );

        // Act
        var distance = Territory.PointToPolygonDistance(point, polygon);

        // Assert
        distance.Meters.ShouldBe(0);
    }

    [Fact]
    public void TestPointToPolygonDistance_PointOutsidePolygon_ReturnsShortestDistance()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(0, 0));
        var polygon = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(1, 1),
                new Coordinate(2, 1),
                new Coordinate(2, 2),
                new Coordinate(1, 2),
                new Coordinate(1, 1)
            }
        );

        // Act
        var distance = Territory.PointToPolygonDistance(point, polygon);

        // Assert
        // The shortest distance is to the corner at (1,1), which is sqrt(2) ≈ 1.414 units away
        distance.Meters.ShouldBeInRange(1.41, 1.42);
    }

    [Fact]
    public void TestPointToPolygonDistance_PointOnBoundary_ReturnsZero()
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
                new Coordinate(1, 1)
            }
        );

        // Act
        var distance = Territory.PointToPolygonDistance(point, polygon);

        // Assert
        distance.Meters.ShouldBe(0, 1e-10);
    }

    [Fact]
    public void TestPointToPolygonDistance_WithDifferentUnits_ReturnsCorrectUnit()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(0, 0));
        var polygon = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(1000, 0),
                new Coordinate(2000, 0),
                new Coordinate(2000, 1000),
                new Coordinate(1000, 1000),
                new Coordinate(1000, 0)
            }
        );

        // Act
        var distanceKm = Territory.PointToPolygonDistance(point, polygon, LengthUnit.Kilometer);

        // Assert
        distanceKm.Kilometers.ShouldBe(1, 1e-10); // 1000 meters = 1 kilometer
        distanceKm.Unit.ShouldBe(LengthUnit.Kilometer);
    }

    [Fact]
    public void TestPointToPolygonDistance_WithCoordinateOverload_ReturnsCorrectDistance()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = new Coordinate(0, 0);
        var polygon = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(1, 1),
                new Coordinate(2, 1),
                new Coordinate(2, 2),
                new Coordinate(1, 2),
                new Coordinate(1, 1)
            }
        );

        // Act
        var distance = Territory.PointToPolygonDistance(point, polygon);

        // Assert
        distance.Meters.ShouldBeInRange(1.41, 1.42); // sqrt(2) ≈ 1.414
    }

    [Fact]
    public void TestPointToPolygonDistance_PolygonWithHole_PointInsideHole_ReturnsShortestDistanceToHoleBoundary()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(1.5, 1.5));

        // Create shell coordinates
        var shellCoords = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(3, 0),
            new Coordinate(3, 3),
            new Coordinate(0, 3),
            new Coordinate(0, 0)
        };

        // Create hole coordinates
        var holeCoords = new[]
        {
            new Coordinate(1, 1),
            new Coordinate(2, 1),
            new Coordinate(2, 2),
            new Coordinate(1, 2),
            new Coordinate(1, 1)
        };

        var shell = geometryFactory.CreateLinearRing(shellCoords);
        var hole = geometryFactory.CreateLinearRing(holeCoords);
        var polygon = geometryFactory.CreatePolygon(shell, new[] { hole });

        // Act
        var distance = Territory.PointToPolygonDistance(point, polygon);

        // Assert
        // The point is inside the hole, so it's not inside the polygon
        // The shortest distance is to the hole boundary, which is 0.5 units
        distance.Meters.ShouldBeInRange(0.49, 0.51);
    }

    [Fact]
    public void TestPointToPolygonDistance_MultiPolygon_ReturnsMinimumDistance()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(0, 0));

        var poly1 = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(1, 1),
                new Coordinate(2, 1),
                new Coordinate(2, 2),
                new Coordinate(1, 2),
                new Coordinate(1, 1)
            }
        );

        var poly2 = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(3, 3),
                new Coordinate(4, 3),
                new Coordinate(4, 4),
                new Coordinate(3, 4),
                new Coordinate(3, 3)
            }
        );

        var multiPolygon = geometryFactory.CreateMultiPolygon(new[] { poly1, poly2 });

        // Act
        var distance = Territory.PointToPolygonDistance(point, multiPolygon);

        // Assert
        // The shortest distance is to poly1 at (1,1), which is sqrt(2) ≈ 1.414 units away
        distance.Meters.ShouldBeInRange(1.41, 1.42);
    }

    [Fact]
    public void TestPointToPolygonDistance_PointInsideMultiPolygon_ReturnsZero()
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
                new Coordinate(1, 1)
            }
        );

        var poly2 = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(3, 3),
                new Coordinate(4, 3),
                new Coordinate(4, 4),
                new Coordinate(3, 4),
                new Coordinate(3, 3)
            }
        );

        var multiPolygon = geometryFactory.CreateMultiPolygon(new[] { poly1, poly2 });

        // Act
        var distance = Territory.PointToPolygonDistance(point, multiPolygon);

        // Assert
        distance.Meters.ShouldBe(0);
    }
}
