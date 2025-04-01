namespace DotTerritory.Tests;

public class SimplifyTests
{
    [Fact]
    public void TestSimplify_LineString_ReducesPoints()
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
                new Coordinate(1, 0)
            }
        );

        // Act
        var simplified = Territory.Simplify(line, 0.05);

        // Assert
        simplified.NumPoints.ShouldBe(2); // Only the start and end points should remain
        simplified.GetCoordinateN(0).ShouldBe(new Coordinate(0, 0));
        simplified.GetCoordinateN(1).ShouldBe(new Coordinate(1, 0));
    }

    [Fact]
    public void TestSimplify_Polygon_PreservesTopology()
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
                new Coordinate(0, 0)
            }
        );

        // Act
        var simplified = Territory.Simplify(polygon, 0.3);

        // Assert
        simplified.NumPoints.ShouldBe(5); // Simplified to a simple rectangle (5 points including the closing point)
        simplified.IsValid.ShouldBeTrue(); // The topology should be preserved
    }

    [Fact]
    public void TestSimplify_EmptyGeometry_ReturnsEmptyGeometry()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var emptyLine = geometryFactory.CreateLineString(Array.Empty<Coordinate>());

        // Act
        var simplified = Territory.Simplify(emptyLine, 0.1);

        // Assert
        simplified.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void TestSimplify_ZeroTolerance_ReturnsOriginalGeometry()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var line = geometryFactory.CreateLineString(
            new[] { new Coordinate(0, 0), new Coordinate(0.5, 0.5), new Coordinate(1, 0) }
        );

        // Act
        var simplified = Territory.Simplify(line, 0);

        // Assert
        simplified.NumPoints.ShouldBe(line.NumPoints);
        simplified.Coordinates.ShouldBe(line.Coordinates);
    }

    [Fact]
    public void TestSimplify_WithHighQuality_ReturnsValidGeometry()
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
                new Coordinate(0, 0)
            }
        );

        // Act
        var simplified = Territory.Simplify(polygon, 0.3, highQuality: true);

        // Assert
        simplified.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void TestSimplify_WithOptions_UsesCorrectQualitySetting()
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
                new Coordinate(1, 0)
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
        simplifiedHQ.NumPoints.ShouldBe(simplifiedHQDirect.NumPoints);
        simplifiedHQ.GetCoordinateN(0).ShouldBe(simplifiedHQDirect.GetCoordinateN(0));
        simplifiedHQ
            .GetCoordinateN(simplifiedHQ.NumPoints - 1)
            .ShouldBe(simplifiedHQDirect.GetCoordinateN(simplifiedHQDirect.NumPoints - 1));
    }

    [Fact]
    public void TestSimplify_ComplexGeometry_ReducesSize()
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
        var simplified = Territory.Simplify(complexPolygon, 0.2);

        // Assert
        simplified.NumPoints.ShouldBeLessThan(complexPolygon.NumPoints);
        simplified.IsValid.ShouldBeTrue();
    }
}
