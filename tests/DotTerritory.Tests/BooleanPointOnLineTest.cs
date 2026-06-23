using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanPointOnLineTest
{
    [Test]
    public async Task PointOnLineShouldReturnTrue()
    {
        // Arrange
        var line = new LineString([
            new Coordinate(0, 0),
            new Coordinate(5, 5),
            new Coordinate(10, 10),
        ]);

        var point = new Point(5, 5);

        // Act
        var result = Territory.BooleanPointOnLine(point, line);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task PointNotOnLineShouldReturnFalse()
    {
        // Arrange
        var line = new LineString([
            new Coordinate(0, 0),
            new Coordinate(5, 5),
            new Coordinate(10, 10),
        ]);

        var point = new Point(5, 6);

        // Act
        var result = Territory.BooleanPointOnLine(point, line);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task PointBetweenLineSegmentsShouldReturnTrue()
    {
        // Arrange
        var line = new LineString([
            new Coordinate(0, 0),
            new Coordinate(5, 5),
            new Coordinate(10, 10),
        ]);

        var point = new Point(2.5, 2.5);

        // Act
        var result = Territory.BooleanPointOnLine(point, line);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task PointOnLineEndpointShouldReturnTrue()
    {
        // Arrange
        var line = new LineString([
            new Coordinate(0, 0),
            new Coordinate(5, 5),
            new Coordinate(10, 10),
        ]);

        var point = new Point(0, 0);

        // Act
        var result = Territory.BooleanPointOnLine(point, line);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task PointOnIndividualLineInMultiLineShouldBeDetected()
    {
        // Arrange
        var line1 = new LineString([new Coordinate(0, 0), new Coordinate(5, 5)]);

        var line2 = new LineString([new Coordinate(10, 10), new Coordinate(15, 15)]);

        var point = new Point(12.5, 12.5);

        // Act
        var result = Territory.BooleanPointOnLine(point, line2);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task PointNotOnIndividualLinesInMultiLineTest()
    {
        // Arrange
        var line1 = new LineString([new Coordinate(0, 0), new Coordinate(5, 5)]);

        var line2 = new LineString([new Coordinate(10, 10), new Coordinate(15, 15)]);

        var point = new Point(7, 7);

        // Act
        var onLine1 = Territory.BooleanPointOnLine(point, line1);
        var onLine2 = Territory.BooleanPointOnLine(point, line2);

        // Assert
        await Assert.That(onLine1).IsFalse();
        await Assert.That(onLine2).IsFalse();
    }
}
