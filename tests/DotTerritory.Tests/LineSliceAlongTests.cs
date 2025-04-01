namespace DotTerritory.Tests;

public class LineSliceAlongTests
{
    [Fact]
    public void TestLineSliceAlong_SliceMiddlePortion_ReturnsCorrectSegment()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var coordinates = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(10, 0),
            new Coordinate(20, 0)
        };
        var line = geometryFactory.CreateLineString(coordinates);
        var startDistance = Length.FromMeters(5);
        var stopDistance = Length.FromMeters(15);

        // Act
        var sliced = Territory.LineSliceAlong(line, startDistance, stopDistance);

        // Assert
        sliced.Coordinates.Length.ShouldBe(2);
        sliced.Coordinates[0].X.ShouldBe(5, 0.0001);
        sliced.Coordinates[0].Y.ShouldBe(0, 0.0001);
        sliced.Coordinates[1].X.ShouldBe(15, 0.0001);
        sliced.Coordinates[1].Y.ShouldBe(0, 0.0001);
    }

    [Fact]
    public void TestLineSliceAlong_SliceEntireLine_ReturnsFullLine()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var coordinates = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(10, 0),
            new Coordinate(20, 0)
        };
        var line = geometryFactory.CreateLineString(coordinates);
        var startDistance = Length.FromMeters(0);
        var stopDistance = Length.FromMeters(20);

        // Act
        var sliced = Territory.LineSliceAlong(line, startDistance, stopDistance);

        // Assert
        sliced.Coordinates.Length.ShouldBe(3);
        sliced.Coordinates[0].X.ShouldBe(0, 0.0001);
        sliced.Coordinates[0].Y.ShouldBe(0, 0.0001);
        sliced.Coordinates[1].X.ShouldBe(10, 0.0001);
        sliced.Coordinates[1].Y.ShouldBe(0, 0.0001);
        sliced.Coordinates[2].X.ShouldBe(20, 0.0001);
        sliced.Coordinates[2].Y.ShouldBe(0, 0.0001);
    }

    [Fact]
    public void TestLineSliceAlong_StartEqualsStop_ReturnsPointAsLine()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var coordinates = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(10, 0),
            new Coordinate(20, 0)
        };
        var line = geometryFactory.CreateLineString(coordinates);
        var distance = Length.FromMeters(5);

        // Act
        var sliced = Territory.LineSliceAlong(line, distance, distance);

        // Assert
        // Should return a point as a LineString with the same start and end
        sliced.Coordinates.Length.ShouldBe(2);
        sliced.Coordinates[0].X.ShouldBe(5, 0.0001);
        sliced.Coordinates[0].Y.ShouldBe(0, 0.0001);
        sliced.Coordinates[1].X.ShouldBe(5, 0.0001);
        sliced.Coordinates[1].Y.ShouldBe(0, 0.0001);
    }

    [Fact]
    public void TestLineSliceAlong_StartGreaterThanStop_SwapsThem()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var coordinates = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(10, 0),
            new Coordinate(20, 0)
        };
        var line = geometryFactory.CreateLineString(coordinates);
        var startDistance = Length.FromMeters(15);
        var stopDistance = Length.FromMeters(5);

        // Act
        var sliced = Territory.LineSliceAlong(line, startDistance, stopDistance);

        // Assert - should swap start and stop
        sliced.Coordinates.Length.ShouldBe(2);
        sliced.Coordinates[0].X.ShouldBe(5, 0.0001);
        sliced.Coordinates[0].Y.ShouldBe(0, 0.0001);
        sliced.Coordinates[1].X.ShouldBe(15, 0.0001);
        sliced.Coordinates[1].Y.ShouldBe(0, 0.0001);
    }

    [Fact]
    public void TestLineSliceAlong_DistancesOutOfRange_ClampsToBounds()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var coordinates = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(10, 0),
            new Coordinate(20, 0)
        };
        var line = geometryFactory.CreateLineString(coordinates);
        var startDistance = Length.FromMeters(-5); // Negative, should clamp to 0
        var stopDistance = Length.FromMeters(25); // Beyond length, should clamp to 20

        // Act
        var sliced = Territory.LineSliceAlong(line, startDistance, stopDistance);

        // Assert - should clamp distances to line bounds
        sliced.Coordinates.Length.ShouldBe(3);
        sliced.Coordinates[0].X.ShouldBe(0, 0.0001);
        sliced.Coordinates[0].Y.ShouldBe(0, 0.0001);
        sliced.Coordinates[1].X.ShouldBe(10, 0.0001);
        sliced.Coordinates[1].Y.ShouldBe(0, 0.0001);
        sliced.Coordinates[2].X.ShouldBe(20, 0.0001);
        sliced.Coordinates[2].Y.ShouldBe(0, 0.0001);
    }

    [Fact]
    public void TestLineSliceAlong_WithComplexLine_ReturnsCorrectSegment()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var coordinates = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(10, 10),
            new Coordinate(20, 0),
            new Coordinate(30, 10)
        };
        var line = geometryFactory.CreateLineString(coordinates);

        // Calculate segment lengths
        var segment1Length = Territory.Distance(coordinates[0], coordinates[1], LengthUnit.Meter);
        var segment2Length = Territory.Distance(coordinates[1], coordinates[2], LengthUnit.Meter);

        // Start in the middle of the first segment, end in the middle of the second segment
        var startDistance = Length.FromMeters(segment1Length.Meters / 2);
        var stopDistance = Length.FromMeters(segment1Length.Meters + segment2Length.Meters / 2);

        // Act
        var sliced = Territory.LineSliceAlong(line, startDistance, stopDistance);

        // Assert
        sliced.Coordinates.Length.ShouldBeGreaterThan(1);

        // First coordinate should be half way along the first segment
        sliced.Coordinates[0].X.ShouldBe(5, 0.0001);
        sliced.Coordinates[0].Y.ShouldBe(5, 0.0001);

        // Middle coordinate should be the middle vertex
        sliced.Coordinates[1].X.ShouldBe(10, 0.0001);
        sliced.Coordinates[1].Y.ShouldBe(10, 0.0001);

        // Last coordinate should be half way along the second segment
        sliced.Coordinates[2].X.ShouldBe(15, 0.0001);
        sliced.Coordinates[2].Y.ShouldBe(5, 0.0001);
    }
}
