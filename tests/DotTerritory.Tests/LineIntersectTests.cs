namespace DotTerritory.Tests;

public class LineIntersectTests
{
    [Test]
    public async Task TestLineIntersect_SingleIntersection()
    {
        var factory = new GeometryFactory();

        // Create two lines that intersect at (1, 1)
        var line1 = factory.CreateLineString([new Coordinate(0, 0), new Coordinate(2, 2)]);

        var line2 = factory.CreateLineString([new Coordinate(0, 2), new Coordinate(2, 0)]);

        var result = Territory.LineIntersect(line1, line2);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.NumGeometries).IsEqualTo(1);

        var point = (Point)result.GetGeometryN(0);
        await Assert.That(point.X).IsEqualTo(1).Within(1e-5);
        await Assert.That(point.Y).IsEqualTo(1).Within(1e-5);
    }

    [Test]
    public async Task TestLineIntersect_NoIntersection()
    {
        var factory = new GeometryFactory();

        // Create two parallel lines with no intersection
        var line1 = factory.CreateLineString([new Coordinate(0, 0), new Coordinate(2, 0)]);

        var line2 = factory.CreateLineString([new Coordinate(0, 1), new Coordinate(2, 1)]);

        var result = Territory.LineIntersect(line1, line2);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.NumGeometries).IsEqualTo(0);
    }

    [Test]
    public async Task TestLineIntersect_NullInputs()
    {
        var factory = new GeometryFactory();

        var line = factory.CreateLineString([new Coordinate(0, 0), new Coordinate(2, 2)]);

        // Test with null inputs
        var result1 = Territory.LineIntersect(line, null!);
        var result2 = Territory.LineIntersect(null!, line);
        var result3 = Territory.LineIntersect(null!, null!);

        await Assert.That(result1).IsNotNull();
        await Assert.That(result1.NumGeometries).IsEqualTo(0);

        await Assert.That(result2).IsNotNull();
        await Assert.That(result2.NumGeometries).IsEqualTo(0);

        await Assert.That(result3).IsNotNull();
        await Assert.That(result3.NumGeometries).IsEqualTo(0);
    }
}
