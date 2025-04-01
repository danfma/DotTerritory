using NetTopologySuite.Geometries;
using Shouldly;
using Xunit;

namespace DotTerritory.Tests;

public class EnvelopeTests
{
    [Fact]
    public void TestEnvelope_FromBbox_CreatesCorrectPolygon()
    {
        // Arrange
        var bbox = new BBox(-75.343, 39.984, -70.534, 42.123);

        // Act
        var polygon = Territory.Envelope(bbox);

        // Assert
        polygon.ShouldBeOfType<Polygon>();

        // Check that the polygon has the correct number of points
        polygon.ExteriorRing.NumPoints.ShouldBe(5); // 4 corners + 1 to close the ring

        // Check that the coordinates match the bbox corners
        var coords = polygon.ExteriorRing.Coordinates;

        // Coordinates should be in counter-clockwise order, starting from the northwest
        coords[0].X.ShouldBe(bbox.West);
        coords[0].Y.ShouldBe(bbox.North);

        coords[1].X.ShouldBe(bbox.East);
        coords[1].Y.ShouldBe(bbox.North);

        coords[2].X.ShouldBe(bbox.East);
        coords[2].Y.ShouldBe(bbox.South);

        coords[3].X.ShouldBe(bbox.West);
        coords[3].Y.ShouldBe(bbox.South);

        // Last point is the same as first to close the ring
        coords[4].X.ShouldBe(coords[0].X);
        coords[4].Y.ShouldBe(coords[0].Y);
    }

    [Fact]
    public void TestEnvelope_FromCoordinates_CreatesCorrectPolygon()
    {
        // Arrange
        var west = -75.343;
        var south = 39.984;
        var east = -70.534;
        var north = 42.123;

        // Act
        var polygon = Territory.Envelope(west, south, east, north);

        // Assert
        polygon.ShouldBeOfType<Polygon>();

        // Check that the polygon's envelope matches the bbox
        var envelope = polygon.EnvelopeInternal;
        envelope.MinX.ShouldBe(west);
        envelope.MinY.ShouldBe(south);
        envelope.MaxX.ShouldBe(east);
        envelope.MaxY.ShouldBe(north);
    }

    [Fact]
    public void TestEnvelope_FromGeometry_CreatesCorrectPolygon()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(-75.343, 39.984));

        // Act
        var polygon = Territory.Envelope(point);

        // Assert
        polygon.ShouldBeOfType<Polygon>();

        // The envelope of a point should be a polygon with the point's coordinates
        var coords = polygon.ExteriorRing.Coordinates;

        // All coordinates should have the point's x,y values
        foreach (var coord in coords)
        {
            coord.X.ShouldBe(point.X);
            coord.Y.ShouldBe(point.Y);
        }
    }

    [Fact]
    public void TestEnvelope_FromComplexGeometry_CreatesBoundingRectangle()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var lineString = geometryFactory.CreateLineString(
            new[]
            {
                new Coordinate(-75.343, 39.984),
                new Coordinate(-70.534, 42.123),
                new Coordinate(-72.534, 40.123),
            }
        );

        // Act
        var polygon = Territory.Envelope(lineString);

        // Assert
        polygon.ShouldBeOfType<Polygon>();

        // Check that the polygon encompasses all points of the line
        var envelope = lineString.EnvelopeInternal;
        var polygonEnvelope = polygon.EnvelopeInternal;

        polygonEnvelope.MinX.ShouldBe(envelope.MinX);
        polygonEnvelope.MinY.ShouldBe(envelope.MinY);
        polygonEnvelope.MaxX.ShouldBe(envelope.MaxX);
        polygonEnvelope.MaxY.ShouldBe(envelope.MaxY);
    }
}
