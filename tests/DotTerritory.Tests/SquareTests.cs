using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class SquareTests
{
    [Test]
    public async Task TestSquare_WhenWidthEqualsHeight_ReturnsOriginalBBox()
    {
        // Arrange
        var bbox = new BBox(-20, -20, 20, 20);

        // Act
        var squared = Territory.Square(bbox);

        // Assert
        await Assert.That(squared.West).IsEqualTo(-20);
        await Assert.That(squared.South).IsEqualTo(-20);
        await Assert.That(squared.East).IsEqualTo(20);
        await Assert.That(squared.North).IsEqualTo(20);
    }

    [Test]
    public async Task TestSquare_WhenWidthGreaterThanHeight_IncreasesHeight()
    {
        // Arrange
        var bbox = new BBox(-20, -10, 20, 10);

        // Act
        var squared = Territory.Square(bbox);

        // Assert
        await Assert.That(squared.West).IsEqualTo(-20);
        await Assert.That(squared.East).IsEqualTo(20);

        // Height should be expanded equally on both sides
        await Assert.That(squared.South).IsEqualTo(-20);
        await Assert.That(squared.North).IsEqualTo(20);
    }

    [Test]
    public async Task TestSquare_WhenHeightGreaterThanWidth_IncreasesWidth()
    {
        // Arrange
        var bbox = new BBox(-10, -20, 10, 20);

        // Act
        var squared = Territory.Square(bbox);

        // Assert
        await Assert.That(squared.South).IsEqualTo(-20);
        await Assert.That(squared.North).IsEqualTo(20);

        // Width should be expanded equally on both sides
        await Assert.That(squared.West).IsEqualTo(-20);
        await Assert.That(squared.East).IsEqualTo(20);
    }

    [Test]
    public async Task TestSquare_WithNegativeCoordinates_ReturnsCorrectSquare()
    {
        // Arrange
        var bbox = new BBox(-20, -20, -10, 0);

        // Act
        var squared = Territory.Square(bbox);

        // Assert
        await Assert.That(squared.North).IsEqualTo(0);
        await Assert.That(squared.South).IsEqualTo(-20);

        // Width should be expanded to match height (20)
        var width = squared.East - squared.West;
        await Assert.That(width).IsEqualTo(20);

        // Center should be preserved
        var centerX = (squared.West + squared.East) / 2;
        await Assert.That(centerX).IsEqualTo(-15);
    }

    [Test]
    public async Task TestSquare_WithParameterOverload_ReturnsCorrectSquare()
    {
        // Arrange: directly provide west, south, east, north

        // Act
        var squared = Territory.Square(-20, -10, 20, 10);

        // Assert
        await Assert.That(squared.West).IsEqualTo(-20);
        await Assert.That(squared.East).IsEqualTo(20);
        await Assert.That(squared.South).IsEqualTo(-20);
        await Assert.That(squared.North).IsEqualTo(20);
    }
}
