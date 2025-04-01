using NetTopologySuite.Geometries;
using Shouldly;
using Xunit;

namespace DotTerritory.Tests;

public class BooleanClockwiseTests
{
    [Fact]
    public void TestBooleanClockwise_ClockwiseRing_ReturnsTrue()
    {
        // Arrange
        // Clockwise ring (in geographic coordinates, negative area)
        var coordinates = new[]
        {
            new Coordinate(-75.343, 39.984),
            new Coordinate(-75.343, 38.984), // go south
            new Coordinate(-74.343, 38.984), // go east
            new Coordinate(-74.343, 39.984), // go north
            new Coordinate(-75.343, 39.984) // go west and close the ring
        };

        // Act
        var isClockwise = Territory.BooleanClockwise(coordinates);

        // Assert
        isClockwise.ShouldBeTrue();
    }

    [Fact]
    public void TestBooleanClockwise_CounterClockwiseRing_ReturnsFalse()
    {
        // Arrange
        // Counter-clockwise ring (in geographic coordinates, positive area)
        var coordinates = new[]
        {
            new Coordinate(-75.343, 39.984),
            new Coordinate(-74.343, 39.984), // go east
            new Coordinate(-74.343, 38.984), // go south
            new Coordinate(-75.343, 38.984), // go west
            new Coordinate(-75.343, 39.984) // go north and close the ring
        };

        // Act
        var isClockwise = Territory.BooleanClockwise(coordinates);

        // Assert
        isClockwise.ShouldBeFalse();
    }

    [Fact]
    public void TestBooleanClockwise_LinearRing_ReturnsCorrectWindingOrder()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var coordinates = new[]
        {
            new Coordinate(-75.343, 39.984),
            new Coordinate(-75.343, 38.984),
            new Coordinate(-74.343, 38.984),
            new Coordinate(-74.343, 39.984),
            new Coordinate(-75.343, 39.984)
        };
        var ring = geometryFactory.CreateLinearRing(coordinates);

        // Act
        var isClockwise = Territory.BooleanClockwise(ring);

        // Assert
        isClockwise.ShouldBeTrue();
    }

    [Fact]
    public void TestBooleanClockwise_Polygon_UsesExteriorRing()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var coordinates = new[]
        {
            new Coordinate(-75.343, 39.984),
            new Coordinate(-74.343, 39.984),
            new Coordinate(-74.343, 38.984),
            new Coordinate(-75.343, 38.984),
            new Coordinate(-75.343, 39.984)
        };
        var ring = geometryFactory.CreateLinearRing(coordinates);
        var polygon = geometryFactory.CreatePolygon(ring);

        // Act
        var isClockwise = Territory.BooleanClockwise(polygon);

        // Assert
        isClockwise.ShouldBeFalse();
    }

    [Fact]
    public void TestBooleanClockwise_InvalidRing_ThrowsException()
    {
        // Arrange
        // Ring with fewer than 4 coordinates (minimum for a valid ring)
        var coordinates = new[]
        {
            new Coordinate(-75.343, 39.984),
            new Coordinate(-74.343, 39.984),
            new Coordinate(-74.343, 38.984)
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => Territory.BooleanClockwise(coordinates));
    }

    [Fact]
    public void TestBooleanClockwise_NotClosedRing_ThrowsException()
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
        Should.Throw<ArgumentException>(() => Territory.BooleanClockwise(coordinates));
    }
}
