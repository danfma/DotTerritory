using FluentAssertions;
using NetTopologySuite.Geometries;
using UnitsNet;

namespace DotTerritory.Tests;

public class WalkAlongTest
{
    private static readonly double QuarterSide = 0.01;
    private static readonly double HalfQuarterSide = QuarterSide / 2;

    [Fact]
    public void WalkAlong_ToSecondSegment()
    {
        var circuit = new LinearRing(
            [
                new Coordinate(0, 0),
                new Coordinate(0, QuarterSide),
                new Coordinate(QuarterSide, QuarterSide),
                new Coordinate(QuarterSide, 0),
                new Coordinate(0, 0)
            ]
        );

        var segments = Territory.GetSegments(circuit).ToArray();
        var point = Territory.WalkAlong(circuit, Length.FromKilometers(2));

        point.X.Should().BeGreaterThan(0).And.BeLessOrEqualTo(QuarterSide);
        point.Y.Should().BeApproximately(QuarterSide, 0.00001);
        segments[1]
            .SegmentFraction(point.Coordinate)
            .Should()
            .BeGreaterThan(0)
            .And.BeLessOrEqualTo(1);
    }

    [Fact]
    public void WalkAlong_Loop()
    {
        var circuit = new LinearRing(
            [
                new Coordinate(0, 0),
                new Coordinate(0, QuarterSide),
                new Coordinate(QuarterSide, QuarterSide),
                new Coordinate(QuarterSide, 0),
                new Coordinate(0, 0)
            ]
        );

        var length = Territory.GetLength(circuit);
        var point = Territory.WalkAlong(circuit, length);
        var precision = 0.00001;

        point.X.Should().BeApproximately(0, precision);
        point.Y.Should().BeApproximately(0, precision);
    }

    [Fact]
    public void WalkAlong_OneAndHalfLoop()
    {
        var circuit = new LinearRing(
            [
                new Coordinate(-HalfQuarterSide, 0),
                new Coordinate(-HalfQuarterSide, HalfQuarterSide),
                new Coordinate(HalfQuarterSide, HalfQuarterSide),
                new Coordinate(HalfQuarterSide, -HalfQuarterSide),
                new Coordinate(-HalfQuarterSide, -HalfQuarterSide),
                new Coordinate(-HalfQuarterSide, 0),
            ]
        );

        var length = Territory.GetLength(circuit) * 1.5;
        var point = Territory.WalkAlong(circuit, length);
        var precision = 1e-6;

        point.X.Should().BeApproximately(HalfQuarterSide, precision);
        point.Y.Should().BeApproximately(0, precision);
    }
}
