namespace DotTerritory.Tests;

public class PointToLineDistanceTests
{
    [Test]
    public async Task TestPointToLineDistance_PointOnLine_ReturnsZero()
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
        await Assert.That(distance.Meters).IsEqualTo(0).Within(1e-10);
    }

    [Test]
    public async Task TestPointToLineDistance_EmptyLine_ReturnsZero()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(0, 0));
        var line = geometryFactory.CreateLineString(Array.Empty<Coordinate>());

        // Act
        var distance = Territory.PointToLineDistance(point, line);

        // Assert
        await Assert.That(distance.Meters).IsEqualTo(0);
    }

    [Test]
    public async Task TestPointToSegmentDistance_PointOnSegment_ReturnsZero()
    {
        // Arrange
        var point = new Coordinate(1, 1);
        var start = new Coordinate(0, 0);
        var end = new Coordinate(2, 2);

        // Act
        var distance = Territory.PointToSegmentDistance(point, start, end);

        // Assert
        await Assert.That(distance.Meters).IsEqualTo(0).Within(1e-10);
    }
}
