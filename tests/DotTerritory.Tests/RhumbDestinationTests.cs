using NetTopologySuite.Geometries;
using UnitsNet;
using UnitsNet.Units;

namespace DotTerritory.Tests;

public class RhumbDestinationTests
{
    [Test]
    public async Task TestRhumbDestination_EastwardJourney_CorrectDestination()
    {
        // Arrange
        var origin = new Coordinate(-75.343, 39.984);
        var distance = Length.FromKilometers(100);
        var bearing = Angle.FromDegrees(90); // Due east

        // Act
        var destination = Territory.RhumbDestination(origin, distance, bearing);

        // Assert
        // Should move eastward (increase longitude) along same latitude
        await Assert.That(destination.Y).IsBetween(39.95, 40.02); // Latitude should be approximately the same
        await Assert.That(destination.X).IsGreaterThan(origin.X); // Longitude should increase

        // Validate distance between points is approximately what we specified
        var calculatedDistance = Territory.RhumbDistance(origin, destination, LengthUnit.Kilometer);
        await Assert.That(calculatedDistance.Kilometers).IsBetween(99, 101);
    }

    [Test]
    public async Task TestRhumbDestination_NorthwardJourney_CorrectDestination()
    {
        // Arrange
        var origin = new Coordinate(-75.343, 39.984);
        var distance = Length.FromKilometers(100);
        var bearing = Angle.FromDegrees(0); // Due north

        // Act
        var destination = Territory.RhumbDestination(origin, distance, bearing);

        // Assert
        // Should move northward (increase latitude) along same longitude
        await Assert.That(destination.X).IsBetween(-75.37, -75.31); // Longitude should be approximately the same
        await Assert.That(destination.Y).IsGreaterThan(origin.Y); // Latitude should increase

        // Validate distance between points is approximately what we specified
        var calculatedDistance = Territory.RhumbDistance(origin, destination, LengthUnit.Kilometer);
        await Assert.That(calculatedDistance.Kilometers).IsBetween(99, 101);
    }

    [Test]
    public async Task TestRhumbDestination_CrossingEquator_CorrectDestination()
    {
        // Arrange
        var origin = new Coordinate(0, 5); // 5° N of equator
        var distance = Length.FromKilometers(1200);
        var bearing = Angle.FromDegrees(180); // Due south

        // Act
        var destination = Territory.RhumbDestination(origin, distance, bearing);

        // Assert
        // Should move southward, crossing the equator
        await Assert.That(destination.X).IsBetween(-0.1, 0.1); // Longitude should be approximately the same
        await Assert.That(destination.Y).IsLessThan(0); // Latitude should be negative (south of equator)

        // Validate distance between points is approximately what we specified
        var calculatedDistance = Territory.RhumbDistance(origin, destination, LengthUnit.Kilometer);
        await Assert.That(calculatedDistance.Kilometers).IsBetween(1190, 1210);
    }

    [Test]
    public async Task TestRhumbDestination_ParameterOverload_CorrectDestination()
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
        await Assert.That(destination.Y).IsBetween(39.95, 40.02); // Latitude should be approximately the same
        await Assert.That(destination.X).IsGreaterThan(-75.343); // Longitude should increase
    }
}
