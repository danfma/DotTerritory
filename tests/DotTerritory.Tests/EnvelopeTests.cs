using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class EnvelopeTests
{
    [Test]
    public async Task TestEnvelope_FromBbox_CreatesCorrectPolygon()
    {
        // Arrange
        var bbox = new BBox(-75.343, 39.984, -70.534, 42.123);

        // Act
        var polygon = Territory.Envelope(bbox);

        // Assert
        await Assert.That(polygon).IsTypeOf<Polygon>();

        // Check that the polygon has the correct number of points
        await Assert.That(polygon.ExteriorRing.NumPoints).IsEqualTo(5); // 4 corners + 1 to close the ring

        // Check that the coordinates match the bbox corners
        var coords = polygon.ExteriorRing.Coordinates;

        // Coordinates should be in counter-clockwise order, starting from the northwest
        await Assert.That(coords[0].X).IsEqualTo(bbox.West);
        await Assert.That(coords[0].Y).IsEqualTo(bbox.North);

        await Assert.That(coords[1].X).IsEqualTo(bbox.East);
        await Assert.That(coords[1].Y).IsEqualTo(bbox.North);

        await Assert.That(coords[2].X).IsEqualTo(bbox.East);
        await Assert.That(coords[2].Y).IsEqualTo(bbox.South);

        await Assert.That(coords[3].X).IsEqualTo(bbox.West);
        await Assert.That(coords[3].Y).IsEqualTo(bbox.South);

        // Last point is the same as first to close the ring
        await Assert.That(coords[4].X).IsEqualTo(coords[0].X);
        await Assert.That(coords[4].Y).IsEqualTo(coords[0].Y);
    }

    [Test]
    public async Task TestEnvelope_FromCoordinates_CreatesCorrectPolygon()
    {
        // Arrange
        var west = -75.343;
        var south = 39.984;
        var east = -70.534;
        var north = 42.123;

        // Act
        var polygon = Territory.Envelope(west, south, east, north);

        // Assert
        await Assert.That(polygon).IsTypeOf<Polygon>();

        // Check that the polygon's envelope matches the bbox
        var envelope = polygon.EnvelopeInternal;
        await Assert.That(envelope.MinX).IsEqualTo(west);
        await Assert.That(envelope.MinY).IsEqualTo(south);
        await Assert.That(envelope.MaxX).IsEqualTo(east);
        await Assert.That(envelope.MaxY).IsEqualTo(north);
    }

    [Test]
    public async Task TestEnvelope_FromGeometry_CreatesCorrectPolygon()
    {
        // Arrange
        var geometryFactory = new GeometryFactory();
        var point = geometryFactory.CreatePoint(new Coordinate(-75.343, 39.984));

        // Act
        var polygon = Territory.Envelope(point);

        // Assert
        await Assert.That(polygon).IsTypeOf<Polygon>();

        // The envelope of a point should be a polygon with the point's coordinates
        var coords = polygon.ExteriorRing.Coordinates;

        // All coordinates should have the point's x,y values
        foreach (var coord in coords)
        {
            await Assert.That(coord.X).IsEqualTo(point.X);
            await Assert.That(coord.Y).IsEqualTo(point.Y);
        }
    }

    [Test]
    public async Task TestEnvelope_FromComplexGeometry_CreatesBoundingRectangle()
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
        await Assert.That(polygon).IsTypeOf<Polygon>();

        // Check that the polygon encompasses all points of the line
        var envelope = lineString.EnvelopeInternal;
        var polygonEnvelope = polygon.EnvelopeInternal;

        await Assert.That(polygonEnvelope.MinX).IsEqualTo(envelope.MinX);
        await Assert.That(polygonEnvelope.MinY).IsEqualTo(envelope.MinY);
        await Assert.That(polygonEnvelope.MaxX).IsEqualTo(envelope.MaxX);
        await Assert.That(polygonEnvelope.MaxY).IsEqualTo(envelope.MaxY);
    }
}
