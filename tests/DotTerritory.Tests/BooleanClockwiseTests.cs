using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanClockwiseTests
{
    [Test]
    public async Task TestBooleanClockwise_ClockwiseRing_ReturnsTrue()
    {
        // Arrange
        // Clockwise ring (in geographic coordinates, negative area)
        var coordinates = new[]
        {
            new Coordinate(-75.343, 39.984),
            new Coordinate(-75.343, 38.984), // go south
            new Coordinate(-74.343, 38.984), // go east
            new Coordinate(-74.343, 39.984), // go north
            new Coordinate(-75.343, 39.984), // go west and close the ring
        };

        // Act
        var isClockwise = Territory.BooleanClockwise(coordinates);

        // Assert
        await Assert.That(isClockwise).IsTrue();
    }

    [Test]
    public async Task TestBooleanClockwise_CounterClockwiseRing_ReturnsFalse()
    {
        // Arrange
        // Counter-clockwise ring (in geographic coordinates, positive area)
        var coordinates = new[]
        {
            new Coordinate(-75.343, 39.984),
            new Coordinate(-74.343, 39.984), // go east
            new Coordinate(-74.343, 38.984), // go south
            new Coordinate(-75.343, 38.984), // go west
            new Coordinate(-75.343, 39.984), // go north and close the ring
        };

        // Act
        var isClockwise = Territory.BooleanClockwise(coordinates);

        // Assert
        await Assert.That(isClockwise).IsFalse();
    }

    [Test]
    public async Task TestBooleanClockwise_LinearRing_ReturnsCorrectWindingOrder()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var coordinates = new[]
        {
            new Coordinate(-75.343, 39.984),
            new Coordinate(-75.343, 38.984),
            new Coordinate(-74.343, 38.984),
            new Coordinate(-74.343, 39.984),
            new Coordinate(-75.343, 39.984),
        };
        var ring = geometryFactory.CreateLinearRing(coordinates);

        // Act
        var isClockwise = Territory.BooleanClockwise(ring);

        // Assert
        await Assert.That(isClockwise).IsTrue();
    }

    [Test]
    public async Task TestBooleanClockwise_Polygon_UsesExteriorRing()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var coordinates = new[]
        {
            new Coordinate(-75.343, 39.984),
            new Coordinate(-74.343, 39.984),
            new Coordinate(-74.343, 38.984),
            new Coordinate(-75.343, 38.984),
            new Coordinate(-75.343, 39.984),
        };
        var ring = geometryFactory.CreateLinearRing(coordinates);
        var polygon = geometryFactory.CreatePolygon(ring);

        // Act
        var isClockwise = Territory.BooleanClockwise(polygon);

        // Assert
        await Assert.That(isClockwise).IsFalse();
    }

    [Test]
    public async Task TestBooleanClockwise_InvalidRing_ThrowsException()
    {
        // Arrange
        // Ring with fewer than 4 coordinates (minimum for a valid ring)
        var coordinates = new[]
        {
            new Coordinate(-75.343, 39.984),
            new Coordinate(-74.343, 39.984),
            new Coordinate(-74.343, 38.984),
        };

        // Act & Assert
        await Assert
            .That(() => Territory.BooleanClockwise(coordinates))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task TestBooleanClockwise_NotClosedRing_ThrowsException()
    {
        // Arrange
        // Ring where first and last coordinate are not the same
        var coordinates = new[]
        {
            new Coordinate(-75.343, 39.984),
            new Coordinate(-75.343, 38.984),
            new Coordinate(-74.343, 38.984),
            new Coordinate(-74.343, 39.984), // Should be -75.343, 39.984 to close the ring
        };

        // Act & Assert
        await Assert
            .That(() => Territory.BooleanClockwise(coordinates))
            .Throws<ArgumentException>();
    }
}
