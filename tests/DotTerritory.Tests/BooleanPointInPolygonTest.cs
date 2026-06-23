using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanPointInPolygonTest
{
    [Test]
    public async Task PointInPolygonShouldReturnTrue()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        var point = new Point(2.5, 2.5);

        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon);

        // Assert
        await Assert.That(result).IsTrue();

        // Verify with NTS directly for consistency
        await Assert.That(polygon.Contains(point)).IsTrue();
    }

    [Test]
    public async Task PointOutsidePolygonShouldReturnFalse()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        var point = new Point(10, 10);

        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon);

        // Assert
        await Assert.That(result).IsFalse();

        // Verify with NTS directly for consistency
        await Assert.That(polygon.Contains(point)).IsFalse();
    }

    [Test]
    public async Task PointOnBoundaryShouldReturnTrue()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        var point = new Point(0, 2.5);

        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon);

        // Assert
        await Assert.That(result).IsTrue();

        // Verify boundary position - point should be on boundary
        await Assert.That(polygon.Boundary.Distance(point)).IsLessThan(double.Epsilon);
    }

    [Test]
    public async Task PointOnBoundaryWithIgnoreBoundaryShouldReturnFalse()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        var point = new Point(0, 2.5);

        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon, ignoreBoundary: true);

        // Assert
        await Assert.That(result).IsFalse();

        // Verify boundary position - point should be on boundary
        await Assert.That(polygon.Boundary.Distance(point)).IsLessThan(double.Epsilon);
    }

    [Test]
    public async Task PointInHoleShouldReturnFalse()
    {
        // Arrange
        var exteriorRing = new LinearRing([
            new Coordinate(0, 0),
            new Coordinate(0, 10),
            new Coordinate(10, 10),
            new Coordinate(10, 0),
            new Coordinate(0, 0),
        ]);

        var interiorRing = new LinearRing([
            new Coordinate(2, 2),
            new Coordinate(2, 4),
            new Coordinate(4, 4),
            new Coordinate(4, 2),
            new Coordinate(2, 2),
        ]);

        var polygon = new Polygon(exteriorRing, [interiorRing]);
        var point = new Point(3, 3);

        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon);

        // Assert
        await Assert.That(result).IsFalse();

        // Verify with NTS directly for consistency
        await Assert.That(polygon.Contains(point)).IsFalse();

        // Additional verification that point is inside exterior but also inside hole
        await Assert.That(new Polygon(exteriorRing).Contains(point)).IsTrue();
        await Assert.That(new Polygon(interiorRing).Contains(point)).IsTrue();
    }

    [Test]
    public async Task PointOnHoleBoundaryShouldReturnTrue()
    {
        // Arrange
        var exteriorRing = new LinearRing([
            new Coordinate(0, 0),
            new Coordinate(0, 10),
            new Coordinate(10, 10),
            new Coordinate(10, 0),
            new Coordinate(0, 0),
        ]);

        var interiorRing = new LinearRing([
            new Coordinate(2, 2),
            new Coordinate(2, 4),
            new Coordinate(4, 4),
            new Coordinate(4, 2),
            new Coordinate(2, 2),
        ]);

        var polygon = new Polygon(exteriorRing, [interiorRing]);
        var point = new Point(2, 3); // On the hole boundary

        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon);

        // Assert
        await Assert.That(result).IsTrue();

        // Verify boundary position - point should be on boundary
        await Assert.That(polygon.Boundary.Distance(point)).IsLessThan(double.Epsilon);
    }

    [Test]
    public async Task PointInMultiPolygonShouldReturnTrue()
    {
        // Arrange
        var polygon1 = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        var polygon2 = new Polygon(
            new LinearRing([
                new Coordinate(10, 10),
                new Coordinate(10, 15),
                new Coordinate(15, 15),
                new Coordinate(15, 10),
                new Coordinate(10, 10),
            ])
        );

        var multiPolygon = new MultiPolygon([polygon1, polygon2]);
        var point = new Point(12.5, 12.5);

        // Act
        var result = Territory.BooleanPointInPolygon(point, multiPolygon);

        // Assert
        await Assert.That(result).IsTrue();

        // Verify that point is only inside second polygon
        await Assert.That(polygon1.Contains(point)).IsFalse();
        await Assert.That(polygon2.Contains(point)).IsTrue();
    }

    [Test]
    public async Task ComplexConcavePolygonShouldHandlePointsCorrectly()
    {
        // Arrange - Create a concave polygon (C-shape)
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 10),
                new Coordinate(10, 10),
                new Coordinate(10, 8),
                new Coordinate(2, 8),
                new Coordinate(2, 2),
                new Coordinate(10, 2),
                new Coordinate(10, 0),
                new Coordinate(0, 0),
            ])
        );

        // Points to test
        var insidePoint = new Point(1, 1);
        var insideConcavityPoint = new Point(5, 5);
        var outsidePoint = new Point(15, 15);

        // Act & Assert
        await Assert.That(Territory.BooleanPointInPolygon(insidePoint, polygon)).IsTrue();
        await Assert.That(Territory.BooleanPointInPolygon(insideConcavityPoint, polygon)).IsFalse();
        await Assert.That(Territory.BooleanPointInPolygon(outsidePoint, polygon)).IsFalse();

        // Verify with NTS directly for consistency
        await Assert.That(polygon.Contains(insidePoint)).IsTrue();
        await Assert.That(polygon.Contains(insideConcavityPoint)).IsFalse();
        await Assert.That(polygon.Contains(outsidePoint)).IsFalse();
    }

    [Test]
    public async Task NullGeometriesShouldThrowArgumentNullException()
    {
        // Arrange
        var point = new Point(1, 1);
        Polygon? nullPolygon = null;

        // Act & Assert
        await Assert
            .That(() => Territory.BooleanPointInPolygon(point, nullPolygon!))
            .Throws<ArgumentNullException>();

        await Assert
            .That(() => Territory.BooleanPointInPolygon(null!, new Polygon(new LinearRing([]))))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task FeatureInterfaceShouldWorkCorrectly()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        var pointFeature = new Feature(new Point(2.5, 2.5), new AttributesTable());
        var polygonFeature = new Feature(polygon, new AttributesTable());

        // Act
        var result = Territory.BooleanPointInPolygon(pointFeature, polygonFeature);

        // Assert
        await Assert.That(result).IsTrue();
    }
}
