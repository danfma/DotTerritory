using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class CleanCoordsTest
{
    [Test]
    public async Task RemoveDuplicatePointsFromLineString()
    {
        // Arrange
        var lineString = new LineString([
            new Coordinate(0, 0),
            new Coordinate(0, 0), // Duplicate
            new Coordinate(1, 1),
            new Coordinate(2, 2),
            new Coordinate(2, 2), // Duplicate
        ]);

        // Act
        var cleaned = Territory.CleanCoords(lineString);

        // Assert
        // The implementation removes both duplicates and collinear points
        // Since 0,0 - 1,1 - 2,2 are collinear, only the endpoints remain
        await Assert.That(cleaned.Coordinates.Length).IsEqualTo(2);
        await Assert.That(cleaned.Coordinates[0].X).IsEqualTo(0);
        await Assert.That(cleaned.Coordinates[0].Y).IsEqualTo(0);
        await Assert.That(cleaned.Coordinates[1].X).IsEqualTo(2);
        await Assert.That(cleaned.Coordinates[1].Y).IsEqualTo(2);
    }

    [Test]
    public async Task RemoveCollinearPointsFromLineString()
    {
        // Arrange
        var lineString = new LineString([
            new Coordinate(0, 0),
            new Coordinate(1, 1), // Collinear with adjacent points
            new Coordinate(2, 2),
            new Coordinate(3, 3), // Collinear with adjacent points
            new Coordinate(4, 4),
        ]);

        // Act
        var cleaned = Territory.CleanCoords(lineString);

        // Assert
        await Assert.That(cleaned.Coordinates.Length).IsEqualTo(2);
        await Assert.That(cleaned.Coordinates[0].X).IsEqualTo(0);
        await Assert.That(cleaned.Coordinates[0].Y).IsEqualTo(0);
        await Assert.That(cleaned.Coordinates[1].X).IsEqualTo(4);
        await Assert.That(cleaned.Coordinates[1].Y).IsEqualTo(4);
    }

    [Test]
    public async Task RemoveDuplicatePointsFromPolygon()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 0), // Duplicate
                new Coordinate(0, 5),
                new Coordinate(5, 5),
                new Coordinate(5, 5), // Duplicate
                new Coordinate(5, 0),
                new Coordinate(0, 0),
            ])
        );

        // Act
        var cleaned = (Polygon)Territory.CleanCoords(polygon);

        // Assert
        await Assert.That(cleaned.ExteriorRing.Coordinates.Length).IsEqualTo(5);
        await Assert.That(cleaned.ExteriorRing.Coordinates[0].X).IsEqualTo(0);
        await Assert.That(cleaned.ExteriorRing.Coordinates[0].Y).IsEqualTo(0);
        await Assert.That(cleaned.ExteriorRing.Coordinates[1].X).IsEqualTo(0);
        await Assert.That(cleaned.ExteriorRing.Coordinates[1].Y).IsEqualTo(5);
        await Assert.That(cleaned.ExteriorRing.Coordinates[2].X).IsEqualTo(5);
        await Assert.That(cleaned.ExteriorRing.Coordinates[2].Y).IsEqualTo(5);
        await Assert.That(cleaned.ExteriorRing.Coordinates[3].X).IsEqualTo(5);
        await Assert.That(cleaned.ExteriorRing.Coordinates[3].Y).IsEqualTo(0);
        await Assert.That(cleaned.ExteriorRing.Coordinates[4].X).IsEqualTo(0);
        await Assert.That(cleaned.ExteriorRing.Coordinates[4].Y).IsEqualTo(0);
    }

    [Test]
    public async Task RemoveCollinearPointsFromPolygon()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 5),
                new Coordinate(2.5, 5), // Collinear
                new Coordinate(5, 5),
                new Coordinate(5, 2.5), // Collinear
                new Coordinate(5, 0),
                new Coordinate(2.5, 0), // Collinear
                new Coordinate(0, 0),
            ])
        );

        // Act
        var cleaned = (Polygon)Territory.CleanCoords(polygon);

        // Assert
        await Assert.That(cleaned.ExteriorRing.Coordinates.Length).IsEqualTo(5);
        await Assert.That(cleaned.ExteriorRing.Coordinates[0].X).IsEqualTo(0);
        await Assert.That(cleaned.ExteriorRing.Coordinates[0].Y).IsEqualTo(0);
        await Assert.That(cleaned.ExteriorRing.Coordinates[1].X).IsEqualTo(0);
        await Assert.That(cleaned.ExteriorRing.Coordinates[1].Y).IsEqualTo(5);
        await Assert.That(cleaned.ExteriorRing.Coordinates[2].X).IsEqualTo(5);
        await Assert.That(cleaned.ExteriorRing.Coordinates[2].Y).IsEqualTo(5);
        await Assert.That(cleaned.ExteriorRing.Coordinates[3].X).IsEqualTo(5);
        await Assert.That(cleaned.ExteriorRing.Coordinates[3].Y).IsEqualTo(0);
        await Assert.That(cleaned.ExteriorRing.Coordinates[4].X).IsEqualTo(0);
        await Assert.That(cleaned.ExteriorRing.Coordinates[4].Y).IsEqualTo(0);
    }

    [Test]
    public async Task CleanPolygonWithHole()
    {
        // Arrange
        var exteriorRing = new LinearRing([
            new Coordinate(0, 0),
            new Coordinate(0, 10),
            new Coordinate(5, 10), // Collinear
            new Coordinate(10, 10),
            new Coordinate(10, 0),
            new Coordinate(0, 0),
        ]);

        var interiorRing = new LinearRing([
            new Coordinate(2, 2),
            new Coordinate(2, 4),
            new Coordinate(3, 4), // Collinear
            new Coordinate(4, 4),
            new Coordinate(4, 2),
            new Coordinate(2, 2),
        ]);

        var polygon = new Polygon(exteriorRing, [interiorRing]);

        // Act
        var cleaned = (Polygon)Territory.CleanCoords(polygon);

        // Assert
        await Assert.That(cleaned.ExteriorRing.Coordinates.Length).IsEqualTo(5);
        await Assert.That(cleaned.NumInteriorRings).IsEqualTo(1);
        await Assert.That(cleaned.GetInteriorRingN(0).Coordinates.Length).IsEqualTo(5);
    }

    [Test]
    public async Task CleanMultiPoint()
    {
        // Arrange
        var multiPoint = new MultiPoint([
            new Point(0, 0),
            new Point(0, 0), // Duplicate
            new Point(1, 1),
            new Point(2, 2),
            new Point(2, 2), // Duplicate
        ]);

        // Act
        var cleaned = (MultiPoint)Territory.CleanCoords(multiPoint);

        // Assert
        await Assert.That(cleaned.NumGeometries).IsEqualTo(3);
        await Assert.That(((Point)cleaned.GetGeometryN(0)).X).IsEqualTo(0);
        await Assert.That(((Point)cleaned.GetGeometryN(0)).Y).IsEqualTo(0);
        await Assert.That(((Point)cleaned.GetGeometryN(1)).X).IsEqualTo(1);
        await Assert.That(((Point)cleaned.GetGeometryN(1)).Y).IsEqualTo(1);
        await Assert.That(((Point)cleaned.GetGeometryN(2)).X).IsEqualTo(2);
        await Assert.That(((Point)cleaned.GetGeometryN(2)).Y).IsEqualTo(2);
    }
}
