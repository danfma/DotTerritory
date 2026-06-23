using NetTopologySuite.Geometries;
using UnitsNet;

namespace DotTerritory.Tests;

public class WalkAlongTest
{
    private static readonly double QuarterSide = 0.01;
    private static readonly double HalfQuarterSide = QuarterSide / 2;

    [Test]
    public async Task WalkAlong_ToSecondSegment()
    {
        var circuit = new LinearRing([
            new Coordinate(0, 0),
            new Coordinate(0, QuarterSide),
            new Coordinate(QuarterSide, QuarterSide),
            new Coordinate(QuarterSide, 0),
            new Coordinate(0, 0),
        ]);

        var segments = Territory.GetSegments(circuit).ToArray();

        // Calculate the total length and segment lengths for precise position verification
        var totalLength = Territory.GetLength(circuit);
        var firstSegmentLength = Territory.Distance(circuit.Coordinates[0], circuit.Coordinates[1]);
        var distanceToWalk = Length.FromKilometers(2);

        // Calculate expected position (should be on second segment)
        var expectedSegmentIndex = 1;
        var expectedSegmentProgress =
            (distanceToWalk.Kilometers - firstSegmentLength.Kilometers)
            / Territory.Distance(circuit.Coordinates[1], circuit.Coordinates[2]).Kilometers;
        var expectedX = expectedSegmentProgress * QuarterSide;
        var expectedY = QuarterSide;

        var point = Territory.WalkAlong(circuit, distanceToWalk);

        // Assert exact position with reasonable tolerance
        var tolerance = 1e-10;
        await Assert.That(point.X).IsEqualTo(expectedX).Within(tolerance);
        await Assert.That(point.Y).IsEqualTo(expectedY).Within(tolerance);

        // Verify which segment it's on
        var segmentFraction = segments[expectedSegmentIndex].SegmentFraction(point.Coordinate);
        await Assert.That(segmentFraction).IsEqualTo(expectedSegmentProgress).Within(tolerance);

        // Also verify by checking coordinates are on the expected line
        await Assert.That(point.Coordinate.Y).IsEqualTo(QuarterSide).Within(tolerance);
        await Assert.That(point.Coordinate.X).IsGreaterThan(0);
        await Assert.That(point.Coordinate.X).IsLessThan(QuarterSide);
    }

    [Test]
    public async Task WalkAlong_Loop()
    {
        var circuit = new LinearRing([
            new Coordinate(0, 0),
            new Coordinate(0, QuarterSide),
            new Coordinate(QuarterSide, QuarterSide),
            new Coordinate(QuarterSide, 0),
            new Coordinate(0, 0),
        ]);

        var length = Territory.GetLength(circuit);
        var point = Territory.WalkAlong(circuit, length);
        var tolerance = 1e-10;

        // Walking exactly one loop should return to the starting point
        await Assert.That(point.X).IsEqualTo(0).Within(tolerance);
        await Assert.That(point.Y).IsEqualTo(0).Within(tolerance);

        // Verify we get the same point from the first coordinate
        await Assert.That(point.Coordinate.Equals2D(circuit.Coordinates[0])).IsTrue();

        // Additional test for partial loop
        var halfLength = Length.FromMeters(length.Meters / 2);
        var halfPoint = Territory.WalkAlong(circuit, halfLength);

        // After half the length, we should be approximately at the opposite corner
        await Assert.That(halfPoint.X).IsEqualTo(QuarterSide).Within(tolerance);
        await Assert.That(halfPoint.Y).IsEqualTo(QuarterSide).Within(tolerance);

        // Verify coordinates are approximately equal - direct comparison may fail due to floating point precision
        await Assert.That(halfPoint.X).IsEqualTo(circuit.Coordinates[2].X).Within(tolerance);
        await Assert.That(halfPoint.Y).IsEqualTo(circuit.Coordinates[2].Y).Within(tolerance);
    }

    [Test]
    public async Task WalkAlong_OneAndHalfLoop()
    {
        var circuit = new LinearRing([
            new Coordinate(-HalfQuarterSide, 0),
            new Coordinate(-HalfQuarterSide, HalfQuarterSide),
            new Coordinate(HalfQuarterSide, HalfQuarterSide),
            new Coordinate(HalfQuarterSide, -HalfQuarterSide),
            new Coordinate(-HalfQuarterSide, -HalfQuarterSide),
            new Coordinate(-HalfQuarterSide, 0),
        ]);

        var circuitLength = Territory.GetLength(circuit);
        var length = circuitLength * 1.5;
        var point = Territory.WalkAlong(circuit, length);
        var tolerance = 1e-10;

        // Calculate expected position (exactly half-way through the circuit)
        // After one full loop, we're back at (-HalfQuarterSide, 0)
        // Then going 0.5 * circuitLength should put us halfway around the circuit
        // Verify this is the expected coordinate

        await Assert.That(point.X).IsEqualTo(HalfQuarterSide).Within(tolerance);
        await Assert.That(point.Y).IsEqualTo(0).Within(tolerance); // Interpolated between coordinates 2-3

        // Calculate specific expected position
        var halfwayDistance = circuitLength.Meters * 0.5;
        var distFromStartToSegment = Territory
            .Distance(circuit.Coordinates[2], circuit.Coordinates[3])
            .Meters;
        var progressOnSegment =
            (halfwayDistance - distFromStartToSegment)
            / Territory.Distance(circuit.Coordinates[3], circuit.Coordinates[4]).Meters;

        // Make sure we're properly tracking multiple loops
        var oneLoopPoint = Territory.WalkAlong(circuit, circuitLength);
        await Assert.That(oneLoopPoint.X).IsEqualTo(-HalfQuarterSide).Within(tolerance);
        await Assert.That(oneLoopPoint.Y).IsEqualTo(0).Within(tolerance);
    }

    [Test]
    public async Task WalkAlong_NegativeDistance()
    {
        var circuit = new LinearRing([
            new Coordinate(0, 0),
            new Coordinate(0, QuarterSide),
            new Coordinate(QuarterSide, QuarterSide),
            new Coordinate(QuarterSide, 0),
            new Coordinate(0, 0),
        ]);

        var negativeDistance = Length.FromKilometers(-1);
        var point = Territory.WalkAlong(circuit, negativeDistance);
        var tolerance = 1e-10;

        // Negative distance should walk in reverse direction
        // Given the circuit, this should place us near (QuarterSide, 0)
        await Assert.That(point.Y).IsEqualTo(0).Within(tolerance);
        await Assert.That(point.X).IsBetween(0, QuarterSide);

        // Full negative loop should return to start
        var fullLength = Territory.GetLength(circuit);
        var fullNegativePoint = Territory.WalkAlong(circuit, fullLength * -1);
        await Assert.That(fullNegativePoint.X).IsEqualTo(0).Within(tolerance);
        await Assert.That(fullNegativePoint.Y).IsEqualTo(0).Within(tolerance);
    }
}
