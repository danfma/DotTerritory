using NetTopologySuite.Geometries;
using UnitsNet;
using UnitsNet.Units;

namespace DotTerritory.Tests;

public class RhumbDistanceTests
{
    [Test]
    public async Task TestRhumbDistance_SamePoint_ReturnsZero()
    {
        // Arrange
        var point = new Coordinate(-75.343, 39.984);

        // Act
        var distance = Territory.RhumbDistance(point, point, LengthUnit.Kilometer);

        // Assert
        await Assert.That(distance.Kilometers).IsLessThan(0.0001);
    }

    [Test]
    public async Task TestRhumbDistance_PointsAlongSameLongitude_CorrectDistance()
    {
        // Arrange
        var point1 = new Coordinate(-75.343, 39.984);
        var point2 = new Coordinate(-75.343, 35.984); // Same longitude, different latitude

        // Act
        var distance = Territory.RhumbDistance(point1, point2, LengthUnit.Kilometer);

        // Assert - Distance should be close to expected value
        // Approx 445 km for 4° of latitude
        await Assert.That(distance.Kilometers).IsBetween(440, 450);
    }

    [Test]
    public async Task TestRhumbDistance_PointsAcrossEquator_CorrectDistance()
    {
        // Arrange
        var point1 = new Coordinate(0, 10);
        var point2 = new Coordinate(0, -10);

        // Act
        var distance = Territory.RhumbDistance(point1, point2, LengthUnit.Kilometer);

        // Assert - Distance should be close to expected value
        // Approx 2220 km for 20° of latitude along the prime meridian
        await Assert.That(distance.Kilometers).IsBetween(2210, 2230);
    }

    [Test]
    public async Task TestRhumbDistance_PointsAcrossAntimeridian_CorrectDistance()
    {
        // Arrange
        var point1 = new Coordinate(179, 0);
        var point2 = new Coordinate(-179, 0);

        // Act
        var distance = Territory.RhumbDistance(point1, point2, LengthUnit.Kilometer);

        // Assert - Should be a short distance (2° of longitude at equator)
        // Not the long way around the world
        await Assert.That(distance.Kilometers).IsBetween(220, 230);
    }

    [Test]
    public async Task TestRhumbDistance_DefaultUnits_ReturnsMeters()
    {
        // Arrange
        var point1 = new Coordinate(-75.343, 39.984);
        var point2 = new Coordinate(-75.343, 40.984);

        // Act
        var distance = Territory.RhumbDistance(point1, point2);

        // Assert
        // Should be around 111 km or 111,000 meters
        await Assert.That(distance.Meters).IsBetween(110000, 112000);
        await Assert.That(distance.Unit).IsEqualTo(LengthUnit.Meter);
    }
}
