namespace DotTerritory.Tests;

public class SimplifyTests
{
    [Test]
    public async Task TestSimplify_LineString_ReducesPoints()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line = geometryFactory.CreateLineString(
            new[]
            {
                new Coordinate(0, 0),
                new Coordinate(0.01, 0.01), // Should be removed with sufficient tolerance
                new Coordinate(0.02, 0.01), // Should be removed with sufficient tolerance
                new Coordinate(0.03, 0), // Should be removed with sufficient tolerance
                new Coordinate(1, 0),
            }
        );

        // Act
        var simplified = Territory.Simplify(line, 0.05, highQuality: false);

        // Assert
        await Assert.That(simplified.NumPoints).IsEqualTo(2); // Only the start and end points should remain
        await Assert.That(simplified.Coordinates[0]).IsEqualTo(new Coordinate(0, 0));
        await Assert.That(simplified.Coordinates[1]).IsEqualTo(new Coordinate(1, 0));
    }

    [Test]
    public async Task TestSimplify_Polygon_PreservesTopology()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var polygon = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0.9, 0.9), // Should be removed with sufficient tolerance
                new Coordinate(0.8, 1.1), // Should be removed with sufficient tolerance
                new Coordinate(0, 1),
                new Coordinate(0, 0),
            }
        );

        // Act
        var simplified = Territory.Simplify(polygon, 0.3, highQuality: false);

        // Assert
        await Assert.That(simplified.NumPoints).IsEqualTo(5); // Simplified to a simple rectangle (5 points including the closing point)
        await Assert.That(simplified.IsValid).IsTrue(); // The topology should be preserved
    }

    [Test]
    public async Task TestSimplify_EmptyGeometry_ReturnsEmptyGeometry()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var emptyLine = geometryFactory.CreateLineString(Array.Empty<Coordinate>());

        // Act
        var simplified = Territory.Simplify(emptyLine, 0.1, highQuality: false);

        // Assert
        await Assert.That(simplified.IsEmpty).IsTrue();
    }

    [Test]
    public async Task TestSimplify_ZeroTolerance_ReturnsOriginalGeometry()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(0.5, 0.5), new Coordinate(1, 0) }
        );

        // Act
        var simplified = Territory.Simplify(line, 0, highQuality: false);

        // Assert
        await Assert.That(simplified.NumPoints).IsEqualTo(line.NumPoints);
        await Assert.That(simplified.Coordinates).IsEquivalentTo(line.Coordinates);
    }

    [Test]
    public async Task TestSimplify_WithHighQuality_ReturnsValidGeometry()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var polygon = geometryFactory.CreatePolygon(
            new[]
            {
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0.9, 0.9),
                new Coordinate(0.8, 1.1),
                new Coordinate(0, 1),
                new Coordinate(0, 0),
            }
        );

        // Act
        var simplified = Territory.Simplify(polygon, 0.3, highQuality: true);

        // Assert
        await Assert.That(simplified.IsValid).IsTrue();
    }

    [Test]
    public async Task TestSimplify_WithOptions_UsesCorrectQualitySetting()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line = geometryFactory.CreateLineString(
            new[]
            {
                new Coordinate(0, 0),
                new Coordinate(0.01, 0.01),
                new Coordinate(0.02, 0.01),
                new Coordinate(0.03, 0),
                new Coordinate(1, 0),
            }
        );

        // Act - using options overload with high quality
        var simplifiedHQ = Territory.Simplify(
            line,
            0.05,
            options => options with { HighQuality = true }
        );

        // Also test the direct boolean parameter overload
        var simplifiedHQDirect = Territory.Simplify(line, 0.05, true);

        // Assert
        await Assert.That(simplifiedHQ.NumPoints).IsEqualTo(simplifiedHQDirect.NumPoints);
        await Assert.That(simplifiedHQ.Coordinates[0]).IsEqualTo(simplifiedHQDirect.Coordinates[0]);
        await Assert
            .That(simplifiedHQ.Coordinates[simplifiedHQ.NumPoints - 1])
            .IsEqualTo(simplifiedHQDirect.Coordinates[simplifiedHQDirect.NumPoints - 1]);
    }

    [Test]
    public async Task TestSimplify_ComplexGeometry_ReducesSize()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();

        // Create a "noisy" circle with 100 points
        var circlePoints = new List<Coordinate>();
        for (int i = 0; i < 100; i++)
        {
            var angle = i * (Math.PI * 2) / 100;
            // Add some noise
            var radius = 1.0 + (i % 2 == 0 ? 0.05 : -0.05);
            circlePoints.Add(new Coordinate(Math.Cos(angle) * radius, Math.Sin(angle) * radius));
        }
        // Close the circle
        circlePoints.Add(circlePoints[0]);

        var complexPolygon = geometryFactory.CreatePolygon(circlePoints.ToArray());

        // Act
        var simplified = Territory.Simplify(complexPolygon, 0.2, highQuality: false);

        // Assert
        await Assert.That(simplified.NumPoints).IsLessThan(complexPolygon.NumPoints);
        await Assert.That(simplified.IsValid).IsTrue();
    }
}
