namespace DotTerritory.Tests;

public class BooleanParallelTests
{
    [Test]
    public async Task TestBooleanParallel_SameDirection_ReturnsTrue()
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
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TestBooleanParallel_OppositeDirection_ReturnsTrue()
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
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TestBooleanParallel_NotParallel_ReturnsFalse()
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
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TestBooleanParallel_MultipleSegments_ChecksAllSegments()
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
        await Assert.That(resultParallel).IsTrue();
        await Assert.That(resultNotParallel).IsFalse();
    }

    [Test]
    public async Task TestBooleanParallel_DifferentSegmentCounts_ComparesCommonSegments()
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
        await Assert.That(result).IsTrue(); // Only the first segment is compared
    }

    [Test]
    public async Task TestBooleanParallel_WithCustomThreshold_WorksCorrectly()
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
        await Assert.That(resultWithDefaultThreshold).IsFalse(); // Not parallel with 1 degree threshold
        await Assert.That(resultWithLargerThreshold).IsTrue(); // Parallel with 6 degree threshold
    }

    [Test]
    public async Task TestBooleanParallel_ZeroLengthSegments_SkipsThoseSegments()
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
        await Assert.That(result).IsTrue(); // Should skip zero-length segments and compare the others
    }
}
