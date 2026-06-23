using BenchmarkDotNet.Attributes;
using DotTerritory;
using NetTopologySuite.Geometries;
using UnitsNet;

namespace TimeAndMemoryUsage;

[MemoryDiagnoser]
public class DotTerritoryAngleBench
{
    private readonly Coordinate _start = new(5, 5);
    private readonly Coordinate _mid = new(5, 6);
    private readonly Coordinate _end = new(3, 4);
    private readonly Territory.GetAngleOptions _options = new(Mercator: true);

    [Benchmark, BenchmarkCategory("Angle")]
    public Angle GetAngle()
    {
        return Territory.GetAngle(_start, _mid, _end);
    }

    [Benchmark, BenchmarkCategory("Angle")]
    public Angle GetAngleWithAllocation()
    {
        return Territory.GetAngle(new Coordinate(5, 5), new Coordinate(5, 6), new Coordinate(3, 4));
    }

    [Benchmark, BenchmarkCategory("Angle")]
    public Angle GetAngleUsingMercator()
    {
        return Territory.GetAngle(_start, _mid, _end, options => options with { Mercator = true });
    }

    [Benchmark, BenchmarkCategory("Angle")]
    public Angle GetAngleUsingMercatorWithAllocation()
    {
        return Territory.GetAngle(
            new Coordinate(5, 5),
            new Coordinate(5, 6),
            new Coordinate(3, 4),
            options => options with { Mercator = true }
        );
    }
}
