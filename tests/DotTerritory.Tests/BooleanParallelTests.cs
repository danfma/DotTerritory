namespace DotTerritory.Tests;

public class BooleanParallelTests
{
    [Fact]
    public void TestBooleanParallel_SameDirection_ReturnsTrue()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line1 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(1, 1) }
        );
        var line2 = geometryFactory.CreateLineString(
            new[] { new Coordinate(1, 0), new Coordinate(2, 1) }
        );

        // Act
        var result = Territory.BooleanParallel(line1, line2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void TestBooleanParallel_OppositeDirection_ReturnsTrue()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line1 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(1, 1) }
        );
        var line2 = geometryFactory.CreateLineString(
            new[] { new Coordinate(2, 1), new Coordinate(1, 0) }
        );

        // Act
        var result = Territory.BooleanParallel(line1, line2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void TestBooleanParallel_NotParallel_ReturnsFalse()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line1 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(1, 1) }
        );
        var line2 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(1, 0) }
        );

        // Act
        var result = Territory.BooleanParallel(line1, line2);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void TestBooleanParallel_MultipleSegments_ChecksAllSegments()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line1 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(1, 1), new Coordinate(2, 2) }
        );

        // Same direction, all segments parallel
        var line2 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 1), new Coordinate(1, 2), new Coordinate(2, 3) }
        );

        // Different direction, not all segments parallel
        var line3 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(1, 1), new Coordinate(2, 0) }
        );

        // Act
        var resultParallel = Territory.BooleanParallel(line1, line2);
        var resultNotParallel = Territory.BooleanParallel(line1, line3);

        // Assert
        resultParallel.ShouldBeTrue();
        resultNotParallel.ShouldBeFalse();
    }

    [Fact]
    public void TestBooleanParallel_DifferentSegmentCounts_ComparesCommonSegments()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line1 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(1, 1), new Coordinate(2, 2) }
        );

        var line2 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 1), new Coordinate(1, 2) }
        );

        // Act
        var result = Territory.BooleanParallel(line1, line2);

        // Assert
        result.ShouldBeTrue(); // Only the first segment is compared
    }

    [Fact]
    public void TestBooleanParallel_WithCustomThreshold_WorksCorrectly()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line1 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(10, 10) }
        );

        // Almost parallel (difference of ~5.7 degrees)
        var line2 = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(10, 11) }
        );

        // Act
        var resultWithDefaultThreshold = Territory.BooleanParallel(line1, line2); // Default threshold is 1 degree
        var resultWithLargerThreshold = Territory.BooleanParallel(
            line1,
            line2,
            options => options with { Threshold = 6 }
        );

        // Assert
        resultWithDefaultThreshold.ShouldBeFalse(); // Not parallel with 1 degree threshold
        resultWithLargerThreshold.ShouldBeTrue(); // Parallel with 6 degree threshold
    }

    [Fact]
    public void TestBooleanParallel_SinglePointLines_ReturnsFalse()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line1 = geometryFactory.CreateLineString(new[] { new Coordinate(0, 0) });
        var line2 = geometryFactory.CreateLineString(new[] { new Coordinate(1, 1) });

        // Act
        var result = Territory.BooleanParallel(line1, line2);

        // Assert
        result.ShouldBeFalse(); // Single point lines don't have a direction
    }

    [Fact]
    public void TestBooleanParallel_ZeroLengthSegments_SkipsThoseSegments()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line1 = geometryFactory.CreateLineString(
            new[]
            {
                new Coordinate(0, 0),
                new Coordinate(0, 0), // Zero-length segment
                new Coordinate(1, 1),
            }
        );
        var line2 = geometryFactory.CreateLineString(
            new[]
            {
                new Coordinate(1, 0),
                new Coordinate(1, 0), // Zero-length segment
                new Coordinate(2, 1),
            }
        );

        // Act
        var result = Territory.BooleanParallel(line1, line2);

        // Assert
        result.ShouldBeTrue(); // Should skip zero-length segments and compare the others
    }
}
