namespace DotTerritory.Tests;

public class PointToLineDistanceTests
{
    [Fact]
    public void TestPointToLineDistance_PointOnLine_ReturnsZero()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(1, 2));
        var line = geometryFactory.CreateLineString(
            new[] { new Coordinate(1, 1), new Coordinate(1, 2), new Coordinate(1, 3) }
        );

        // Act
        var distance = Territory.PointToLineDistance(point, line);

        // Assert
        distance.Meters.ShouldBe(0, 1e-10);
    }

    [Fact]
    public void TestPointToLineDistance_PointNotOnLine_ReturnsCorrectDistance()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(0, 0));
        var line = geometryFactory.CreateLineString(
            new[] { new Coordinate(1, 0), new Coordinate(1, 1) }
        );

        // Act
        var distance = Territory.PointToLineDistance(point, line);

        // Assert
        distance.Meters.ShouldBe(1, 1e-10); // The point is 1 unit away from the line
    }

    [Fact]
    public void TestPointToLineDistance_WithDifferentUnits_ReturnsCorrectUnit()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(0, 0));
        var line = geometryFactory.CreateLineString(
            new[] { new Coordinate(1000, 0), new Coordinate(1000, 1000) }
        );

        // Act
        var distanceKm = Territory.PointToLineDistance(point, line, LengthUnit.Kilometer);

        // Assert
        distanceKm.Kilometers.ShouldBe(1, 1e-10); // 1000 meters = 1 kilometer
        distanceKm.Unit.ShouldBe(LengthUnit.Kilometer);
    }

    [Fact]
    public void TestPointToLineDistance_EmptyLine_ReturnsZero()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(0, 0));
        var line = geometryFactory.CreateLineString(Array.Empty<Coordinate>());

        // Act
        var distance = Territory.PointToLineDistance(point, line);

        // Assert
        distance.Meters.ShouldBe(0);
    }

    [Fact]
    public void TestPointToLineDistance_SinglePointLine_ReturnsStraightLineDistance()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(0, 0));
        var line = geometryFactory.CreateLineString(new[] { new Coordinate(3, 4) });

        // Act
        var distance = Territory.PointToLineDistance(point, line);

        // Assert
        distance.Meters.ShouldBe(5, 1e-10); // Distance from (0,0) to (3,4) is 5 using Pythagorean theorem
    }

    [Fact]
    public void TestPointToLineDistance_MultiSegmentLine_ReturnsShortestSegmentDistance()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(5, 5));
        var line = geometryFactory.CreateLineString(
            new[]
            {
                new Coordinate(0, 0),
                new Coordinate(10, 0),
                new Coordinate(10, 10),
                new Coordinate(0, 10),
                new Coordinate(0, 0)
            }
        );

        // Act
        var distance = Territory.PointToLineDistance(point, line);

        // Assert
        // The shortest distance is to the segment (0,0)-(10,0), which is 5 units
        distance.Meters.ShouldBe(5, 1e-10);
    }

    [Fact]
    public void TestPointToLineDistance_WithCoordinateOverload_ReturnsCorrectDistance()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = new Coordinate(0, 0);
        var line = geometryFactory.CreateLineString(
            new[] { new Coordinate(1, 0), new Coordinate(1, 1) }
        );

        // Act
        var distance = Territory.PointToLineDistance(point, line);

        // Assert
        distance.Meters.ShouldBe(1, 1e-10);
    }

    [Fact]
    public void TestPointToSegmentDistance_PointOnSegment_ReturnsZero()
    {
        // Arrange
        var point = new Coordinate(1, 1);
        var start = new Coordinate(0, 0);
        var end = new Coordinate(2, 2);

        // Act
        var distance = Territory.PointToSegmentDistance(point, start, end);

        // Assert
        distance.Meters.ShouldBe(0, 1e-10);
    }

    [Fact]
    public void TestPointToSegmentDistance_PointNotOnSegment_ReturnsShortestDistance()
    {
        // Arrange
        var point = new Coordinate(0, 1);
        var start = new Coordinate(0, 0);
        var end = new Coordinate(2, 0);

        // Act
        var distance = Territory.PointToSegmentDistance(point, start, end);

        // Assert
        distance.Meters.ShouldBe(1, 1e-10); // The point is 1 unit above the segment
    }

    [Fact]
    public void TestPointToSegmentDistance_PointBeyondSegmentEnd_ReturnsDistanceToEndpoint()
    {
        // Arrange
        var point = new Coordinate(3, 0);
        var start = new Coordinate(0, 0);
        var end = new Coordinate(2, 0);

        // Act
        var distance = Territory.PointToSegmentDistance(point, start, end);

        // Assert
        distance.Meters.ShouldBe(1, 1e-10); // The point is 1 unit beyond the end of the segment
    }

    [Fact]
    public void TestPointToSegmentDistance_PointBeforeSegmentStart_ReturnsDistanceToStartpoint()
    {
        // Arrange
        var point = new Coordinate(-1, 0);
        var start = new Coordinate(0, 0);
        var end = new Coordinate(2, 0);

        // Act
        var distance = Territory.PointToSegmentDistance(point, start, end);

        // Assert
        distance.Meters.ShouldBe(1, 1e-10); // The point is 1 unit before the start of the segment
    }
}
