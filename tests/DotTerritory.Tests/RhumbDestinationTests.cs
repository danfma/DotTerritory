using NetTopologySuite.Geometries;
using Shouldly;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace DotTerritory.Tests;

public class RhumbDestinationTests
{
    [Fact]
    public void TestRhumbDestination_EastwardJourney_CorrectDestination()
    {
        // Arrange
        var origin = new Coordinate(-75.343, 39.984);
        var distance = Length.FromKilometers(100);
        var bearing = Angle.FromDegrees(90); // Due east

        // Act
        var destination = Territory.RhumbDestination(origin, distance, bearing);

        // Assert
        // Should move eastward (increase longitude) along same latitude
        destination.Y.ShouldBeInRange(39.95, 40.02); // Latitude should be approximately the same
        destination.X.ShouldBeGreaterThan(origin.X); // Longitude should increase

        // Validate distance between points is approximately what we specified
        var calculatedDistance = Territory.RhumbDistance(origin, destination, LengthUnit.Kilometer);
        calculatedDistance.Kilometers.ShouldBeInRange(99, 101);
    }

    [Fact]
    public void TestRhumbDestination_NorthwardJourney_CorrectDestination()
    {
        // Arrange
        var origin = new Coordinate(-75.343, 39.984);
        var distance = Length.FromKilometers(100);
        var bearing = Angle.FromDegrees(0); // Due north

        // Act
        var destination = Territory.RhumbDestination(origin, distance, bearing);

        // Assert
        // Should move northward (increase latitude) along same longitude
        destination.X.ShouldBeInRange(-75.37, -75.31); // Longitude should be approximately the same
        destination.Y.ShouldBeGreaterThan(origin.Y); // Latitude should increase

        // Validate distance between points is approximately what we specified
        var calculatedDistance = Territory.RhumbDistance(origin, destination, LengthUnit.Kilometer);
        calculatedDistance.Kilometers.ShouldBeInRange(99, 101);
    }

    [Fact]
    public void TestRhumbDestination_DiagonalJourney_CorrectDestination()
    {
        // Arrange
        var origin = new Coordinate(-75.343, 39.984);
        var distance = Length.FromKilometers(100);
        var bearing = Angle.FromDegrees(45); // Northeast

        // Act
        var destination = Territory.RhumbDestination(origin, distance, bearing);

        // Assert
        // Should move northeast (increase both latitude and longitude)
        destination.X.ShouldBeGreaterThan(origin.X); // Longitude should increase
        destination.Y.ShouldBeGreaterThan(origin.Y); // Latitude should increase

        // Validate distance between points is approximately what we specified
        var calculatedDistance = Territory.RhumbDistance(origin, destination, LengthUnit.Kilometer);
        calculatedDistance.Kilometers.ShouldBeInRange(99, 101);

        // Validate bearing is approximately what we specified
        var calculatedBearing = Territory.RhumbBearing(origin, destination);
        calculatedBearing.Degrees.ShouldBeInRange(44, 46);
    }

    [Fact]
    public void TestRhumbDestination_NegativeBearing_CorrectDestination()
    {
        // Arrange
        var origin = new Coordinate(-75.343, 39.984);
        var distance = Length.FromKilometers(100);
        var bearing = Angle.FromDegrees(-45); // Northwest

        // Act
        var destination = Territory.RhumbDestination(origin, distance, bearing);

        // Assert
        // Should move northwest (increase latitude, decrease longitude)
        destination.X.ShouldBeLessThan(origin.X); // Longitude should decrease
        destination.Y.ShouldBeGreaterThan(origin.Y); // Latitude should increase

        // Validate distance between points is approximately what we specified
        var calculatedDistance = Territory.RhumbDistance(origin, destination, LengthUnit.Kilometer);
        calculatedDistance.Kilometers.ShouldBeInRange(99, 101);
    }

    [Fact]
    public void TestRhumbDestination_CrossingEquator_CorrectDestination()
    {
        // Arrange
        var origin = new Coordinate(0, 5); // 5° N of equator
        var distance = Length.FromKilometers(1200);
        var bearing = Angle.FromDegrees(180); // Due south

        // Act
        var destination = Territory.RhumbDestination(origin, distance, bearing);

        // Assert
        // Should move southward, crossing the equator
        destination.X.ShouldBeInRange(-0.1, 0.1); // Longitude should be approximately the same
        destination.Y.ShouldBeLessThan(0); // Latitude should be negative (south of equator)

        // Validate distance between points is approximately what we specified
        var calculatedDistance = Territory.RhumbDistance(origin, destination, LengthUnit.Kilometer);
        calculatedDistance.Kilometers.ShouldBeInRange(1190, 1210);
    }

    [Fact]
    public void TestRhumbDestination_ParameterOverload_CorrectDestination()
    {
        // Arrange: Using direct coordinate inputs instead of Coordinate objects

        // Act
        var destination = Territory.RhumbDestination(
            -75.343,
            39.984,
            Length.FromKilometers(100),
            Angle.FromDegrees(90)
        );

        // Assert
        destination.Y.ShouldBeInRange(39.95, 40.02); // Latitude should be approximately the same
        destination.X.ShouldBeGreaterThan(-75.343); // Longitude should increase
    }
}
