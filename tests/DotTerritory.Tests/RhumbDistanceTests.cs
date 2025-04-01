using NetTopologySuite.Geometries;
using Shouldly;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace DotTerritory.Tests;

public class RhumbDistanceTests
{
    [Fact]
    public void TestRhumbDistance_SamePoint_ReturnsZero()
    {
        // Arrange
        var point = new Coordinate(-75.343, 39.984);

        // Act
        var distance = Territory.RhumbDistance(point, point, LengthUnit.Kilometer);

        // Assert
        distance.Kilometers.ShouldBeLessThan(0.0001);
    }

    [Fact]
    public void TestRhumbDistance_PointsAlongSameLatitude_CorrectDistance()
    {
        // Arrange
        var point1 = new Coordinate(-75.343, 39.984);
        var point2 = new Coordinate(-80.343, 39.984); // Same latitude, different longitude

        // Act
        var distance = Territory.RhumbDistance(point1, point2, LengthUnit.Kilometer);

        // Assert - Distance should be close to expected value
        // Approx 390 km along the same latitude at ~40°N
        distance.Kilometers.ShouldBeInRange(385, 395);
    }

    [Fact]
    public void TestRhumbDistance_PointsAlongSameLongitude_CorrectDistance()
    {
        // Arrange
        var point1 = new Coordinate(-75.343, 39.984);
        var point2 = new Coordinate(-75.343, 35.984); // Same longitude, different latitude

        // Act
        var distance = Territory.RhumbDistance(point1, point2, LengthUnit.Kilometer);

        // Assert - Distance should be close to expected value
        // Approx 445 km for 4° of latitude
        distance.Kilometers.ShouldBeInRange(440, 450);
    }

    [Fact]
    public void TestRhumbDistance_PointsAcrossEquator_CorrectDistance()
    {
        // Arrange
        var point1 = new Coordinate(0, 10);
        var point2 = new Coordinate(0, -10);

        // Act
        var distance = Territory.RhumbDistance(point1, point2, LengthUnit.Kilometer);

        // Assert - Distance should be close to expected value
        // Approx 2220 km for 20° of latitude along the prime meridian
        distance.Kilometers.ShouldBeInRange(2210, 2230);
    }

    [Fact]
    public void TestRhumbDistance_PointsAcrossAntimeridian_CorrectDistance()
    {
        // Arrange
        var point1 = new Coordinate(179, 0);
        var point2 = new Coordinate(-179, 0);

        // Act
        var distance = Territory.RhumbDistance(point1, point2, LengthUnit.Kilometer);

        // Assert - Should be a short distance (2° of longitude at equator)
        // Not the long way around the world
        distance.Kilometers.ShouldBeInRange(220, 230);
    }

    [Fact]
    public void TestRhumbDistance_ParameterOverload_CorrectDistance()
    {
        // Arrange: Using direct coordinate inputs instead of Coordinate objects

        // Act
        var distance = Territory.RhumbDistance(
            -75.343,
            39.984,
            -80.343,
            39.984,
            LengthUnit.Kilometer
        );

        // Assert
        distance.Kilometers.ShouldBeInRange(385, 395);
    }

    [Fact]
    public void TestRhumbDistance_DefaultUnits_ReturnsMeters()
    {
        // Arrange
        var point1 = new Coordinate(-75.343, 39.984);
        var point2 = new Coordinate(-75.343, 40.984);

        // Act
        var distance = Territory.RhumbDistance(point1, point2);

        // Assert
        // Should be around 111 km or 111,000 meters
        distance.Meters.ShouldBeInRange(110000, 112000);
        distance.Unit.ShouldBe(LengthUnit.Meter);
    }
}
