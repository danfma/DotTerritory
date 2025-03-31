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
        
        // Calculate the total length and segment lengths for precise position verification
        var totalLength = Territory.GetLength(circuit);
        var firstSegmentLength = Territory.Distance(circuit.Coordinates[0], circuit.Coordinates[1]);
        var distanceToWalk = Length.FromKilometers(2);
        
        // Calculate expected position (should be on second segment)
        var expectedSegmentIndex = 1;
        var expectedSegmentProgress = (distanceToWalk.Kilometers - firstSegmentLength.Kilometers) / 
                                      Territory.Distance(circuit.Coordinates[1], circuit.Coordinates[2]).Kilometers;
        var expectedX = expectedSegmentProgress * QuarterSide;
        var expectedY = QuarterSide;
        
        var point = Territory.WalkAlong(circuit, distanceToWalk);

        // Assert exact position with reasonable tolerance
        var tolerance = 1e-10;
        point.X.ShouldBe(expectedX, tolerance);
        point.Y.ShouldBe(expectedY, tolerance);

        // Verify which segment it's on
        var segmentFraction = segments[expectedSegmentIndex].SegmentFraction(point.Coordinate);
        segmentFraction.ShouldBe(expectedSegmentProgress, tolerance);
        
        // Also verify by checking coordinates are on the expected line
        point.Coordinate.ShouldSatisfyAllConditions(
            coordinate => coordinate.Y.ShouldBe(QuarterSide, tolerance),
            coordinate => coordinate.X.ShouldBeGreaterThan(0),
            coordinate => coordinate.X.ShouldBeLessThan(QuarterSide)
        );
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
        var tolerance = 1e-10;

        // Walking exactly one loop should return to the starting point
        point.X.ShouldBe(0, tolerance);
        point.Y.ShouldBe(0, tolerance);
        
        // Verify we get the same point from the first coordinate
        point.Coordinate.Equals2D(circuit.Coordinates[0]).ShouldBeTrue();
        
        // Additional test for partial loop
        var halfLength = Length.FromMeters(length.Meters / 2);
        var halfPoint = Territory.WalkAlong(circuit, halfLength);
        
        // After half the length, we should be approximately at the opposite corner
        halfPoint.X.ShouldBe(QuarterSide, tolerance);
        halfPoint.Y.ShouldBe(QuarterSide, tolerance);
        
        // Verify coordinates are approximately equal - direct comparison may fail due to floating point precision
        halfPoint.X.ShouldBe(circuit.Coordinates[2].X, tolerance);
        halfPoint.Y.ShouldBe(circuit.Coordinates[2].Y, tolerance);
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

        var circuitLength = Territory.GetLength(circuit);
        var length = circuitLength * 1.5;
        var point = Territory.WalkAlong(circuit, length);
        var tolerance = 1e-10;

        // Calculate expected position (exactly half-way through the circuit)
        // After one full loop, we're back at (-HalfQuarterSide, 0)
        // Then going 0.5 * circuitLength should put us halfway around the circuit
        // Verify this is the expected coordinate
        
        point.X.ShouldBe(HalfQuarterSide, tolerance);
        point.Y.ShouldBe(0, tolerance); // Interpolated between coordinates 2-3
        
        // Calculate specific expected position
        var halfwayDistance = circuitLength.Meters * 0.5;
        var distFromStartToSegment = Territory.Distance(circuit.Coordinates[2], circuit.Coordinates[3]).Meters;
        var progressOnSegment = (halfwayDistance - distFromStartToSegment) / 
                               Territory.Distance(circuit.Coordinates[3], circuit.Coordinates[4]).Meters;
        
        // Make sure we're properly tracking multiple loops
        var oneLoopPoint = Territory.WalkAlong(circuit, circuitLength);
        oneLoopPoint.X.ShouldBe(-HalfQuarterSide, tolerance);
        oneLoopPoint.Y.ShouldBe(0, tolerance);
    }
    
    [Fact]
    public void WalkAlong_NegativeDistance()
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
        
        var negativeDistance = Length.FromKilometers(-1);
        var point = Territory.WalkAlong(circuit, negativeDistance);
        var tolerance = 1e-10;
        
        // Negative distance should walk in reverse direction
        // Given the circuit, this should place us near (QuarterSide, 0)
        point.Y.ShouldBe(0, tolerance);
        point.X.ShouldBeInRange(0, QuarterSide);
        
        // Full negative loop should return to start
        var fullLength = Territory.GetLength(circuit);
        var fullNegativePoint = Territory.WalkAlong(circuit, fullLength * -1);
        fullNegativePoint.X.ShouldBe(0, tolerance);
        fullNegativePoint.Y.ShouldBe(0, tolerance);
    }
}
