using NetTopologySuite.Geometries;
using Shouldly;
using Xunit;

namespace DotTerritory.Tests;

public class SquareTests
{
    [Fact]
    public void TestSquare_WhenWidthEqualsHeight_ReturnsOriginalBBox()
    {
        // Arrange
        var bbox = new BBox(-20, -20, 20, 20);

        // Act
        var squared = Territory.Square(bbox);

        // Assert
        squared.West.ShouldBe(-20);
        squared.South.ShouldBe(-20);
        squared.East.ShouldBe(20);
        squared.North.ShouldBe(20);
    }

    [Fact]
    public void TestSquare_WhenWidthGreaterThanHeight_IncreasesHeight()
    {
        // Arrange
        var bbox = new BBox(-20, -10, 20, 10);

        // Act
        var squared = Territory.Square(bbox);

        // Assert
        squared.West.ShouldBe(-20);
        squared.East.ShouldBe(20);

        // Height should be expanded equally on both sides
        squared.South.ShouldBe(-20);
        squared.North.ShouldBe(20);
    }

    [Fact]
    public void TestSquare_WhenHeightGreaterThanWidth_IncreasesWidth()
    {
        // Arrange
        var bbox = new BBox(-10, -20, 10, 20);

        // Act
        var squared = Territory.Square(bbox);

        // Assert
        squared.South.ShouldBe(-20);
        squared.North.ShouldBe(20);

        // Width should be expanded equally on both sides
        squared.West.ShouldBe(-20);
        squared.East.ShouldBe(20);
    }

    [Fact]
    public void TestSquare_WithNegativeCoordinates_ReturnsCorrectSquare()
    {
        // Arrange
        var bbox = new BBox(-20, -20, -10, 0);

        // Act
        var squared = Territory.Square(bbox);

        // Assert
        squared.North.ShouldBe(0);
        squared.South.ShouldBe(-20);

        // Width should be expanded to match height (20)
        var width = squared.East - squared.West;
        width.ShouldBe(20);

        // Center should be preserved
        var centerX = (squared.West + squared.East) / 2;
        centerX.ShouldBe(-15);
    }

    [Fact]
    public void TestSquare_WithParameterOverload_ReturnsCorrectSquare()
    {
        // Arrange: directly provide west, south, east, north

        // Act
        var squared = Territory.Square(-20, -10, 20, 10);

        // Assert
        squared.West.ShouldBe(-20);
        squared.East.ShouldBe(20);
        squared.South.ShouldBe(-20);
        squared.North.ShouldBe(20);
    }
}
