using NetTopologySuite.Geometries;
using Xunit;

namespace DotTerritory.Tests;

public class LineIntersectTests
{
    [Fact]
    public void TestLineIntersect_SingleIntersection()
    {
        var factory = new GeometryFactory();

        // Create two lines that intersect at (1, 1)
        var line1 = factory.CreateLineString([new Coordinate(0, 0), new Coordinate(2, 2)]);

        var line2 = factory.CreateLineString([new Coordinate(0, 2), new Coordinate(2, 0)]);

        var result = Territory.LineIntersect(line1, line2);

        Assert.NotNull(result);
        Assert.Equal(1, result.NumGeometries);

        var point = (Point)result.GetGeometryN(0);
        Assert.Equal(1, point.X, 5);
        Assert.Equal(1, point.Y, 5);
    }

    [Fact]
    public void TestLineIntersect_MultipleLines()
    {
        var factory = new GeometryFactory();

        // Create three lines with various intersections
        var line1 = factory.CreateLineString([new Coordinate(0, 0), new Coordinate(4, 4)]);

        var line2 = factory.CreateLineString([new Coordinate(0, 4), new Coordinate(4, 0)]);

        var line3 = factory.CreateLineString([new Coordinate(0, 2), new Coordinate(4, 2)]);

        var result = Territory.LineIntersect(line1, line2, line3);

        Assert.NotNull(result);
        Assert.Equal(3, result.NumGeometries);

        // Should have intersection points at (2, 2), (1, 3), and (3, 1)
        var points = new List<Coordinate>();
        for (int i = 0; i < result.NumGeometries; i++)
        {
            points.Add(((Point)result.GetGeometryN(i)).Coordinate);
        }

        // Check if the expected intersection points are present
        Assert.Contains(points, p => Math.Abs(p.X - 2) < 0.001 && Math.Abs(p.Y - 2) < 0.001);
        Assert.Contains(points, p => Math.Abs(p.X - 2) < 0.001 && Math.Abs(p.Y - 2) < 0.001);
        Assert.Contains(points, p => Math.Abs(p.X - 3) < 0.001 && Math.Abs(p.Y - 1) < 0.001);
    }

    [Fact]
    public void TestLineIntersect_NoIntersection()
    {
        var factory = new GeometryFactory();

        // Create two parallel lines with no intersection
        var line1 = factory.CreateLineString([new Coordinate(0, 0), new Coordinate(2, 0)]);

        var line2 = factory.CreateLineString([new Coordinate(0, 1), new Coordinate(2, 1)]);

        var result = Territory.LineIntersect(line1, line2);

        Assert.NotNull(result);
        Assert.Equal(0, result.NumGeometries);
    }

    [Fact]
    public void TestLineIntersect_NullInputs()
    {
        var factory = new GeometryFactory();

        var line = factory.CreateLineString([new Coordinate(0, 0), new Coordinate(2, 2)]);

        // Test with null inputs
        var result1 = Territory.LineIntersect(line, null);
        var result2 = Territory.LineIntersect(null, line);
        var result3 = Territory.LineIntersect(null, null);

        Assert.NotNull(result1);
        Assert.Equal(0, result1.NumGeometries);

        Assert.NotNull(result2);
        Assert.Equal(0, result2.NumGeometries);

        Assert.NotNull(result3);
        Assert.Equal(0, result3.NumGeometries);
    }
}
