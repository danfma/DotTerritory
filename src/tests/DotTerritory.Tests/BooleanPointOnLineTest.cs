using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanPointOnLineTest
{
    [Fact]
    public void PointOnLineShouldReturnTrue()
    {
        // Arrange
        var line = new LineString(
            [new Coordinate(0, 0), new Coordinate(5, 5), new Coordinate(10, 10),]
        );

        var point = new Point(5, 5);

        // Act
        var result = Territory.BooleanPointOnLine(point, line);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void PointNotOnLineShouldReturnFalse()
    {
        // Arrange
        var line = new LineString(
            [new Coordinate(0, 0), new Coordinate(5, 5), new Coordinate(10, 10),]
        );

        var point = new Point(5, 6);

        // Act
        var result = Territory.BooleanPointOnLine(point, line);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void PointBetweenLineSegmentsShouldReturnTrue()
    {
        // Arrange
        var line = new LineString(
            [new Coordinate(0, 0), new Coordinate(5, 5), new Coordinate(10, 10),]
        );

        var point = new Point(2.5, 2.5);

        // Act
        var result = Territory.BooleanPointOnLine(point, line);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void PointOnLineEndpointShouldReturnTrue()
    {
        // Arrange
        var line = new LineString(
            [new Coordinate(0, 0), new Coordinate(5, 5), new Coordinate(10, 10),]
        );

        var point = new Point(0, 0);

        // Act
        var result = Territory.BooleanPointOnLine(point, line);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void PointOnIndividualLineInMultiLineShouldBeDetected()
    {
        // Arrange
        var line1 = new LineString([new Coordinate(0, 0), new Coordinate(5, 5),]);

        var line2 = new LineString([new Coordinate(10, 10), new Coordinate(15, 15),]);

        var point = new Point(12.5, 12.5);

        // Act
        var result = Territory.BooleanPointOnLine(point, line2);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void PointNotOnIndividualLinesInMultiLineTest()
    {
        // Arrange
        var line1 = new LineString([new Coordinate(0, 0), new Coordinate(5, 5),]);

        var line2 = new LineString([new Coordinate(10, 10), new Coordinate(15, 15),]);

        var point = new Point(7, 7);

        // Act
        var onLine1 = Territory.BooleanPointOnLine(point, line1);
        var onLine2 = Territory.BooleanPointOnLine(point, line2);

        // Assert
        onLine1.ShouldBeFalse();
        onLine2.ShouldBeFalse();
    }
}
