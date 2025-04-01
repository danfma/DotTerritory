using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class CleanCoordsTest
{
    [Fact]
    public void RemoveDuplicatePointsFromLineString()
    {
        // Arrange
        var lineString = new LineString(
            [
                new Coordinate(0, 0),
                new Coordinate(0, 0), // Duplicate
                new Coordinate(1, 1),
                new Coordinate(2, 2),
                new Coordinate(2, 2), // Duplicate
            ]
        );

        // Act
        var cleaned = Territory.CleanCoords(lineString);

        // Assert
        // The implementation removes both duplicates and collinear points
        // Since 0,0 - 1,1 - 2,2 are collinear, only the endpoints remain
        cleaned.Coordinates.Length.ShouldBe(2);
        cleaned.Coordinates[0].X.ShouldBe(0);
        cleaned.Coordinates[0].Y.ShouldBe(0);
        cleaned.Coordinates[1].X.ShouldBe(2);
        cleaned.Coordinates[1].Y.ShouldBe(2);
    }

    [Fact]
    public void RemoveCollinearPointsFromLineString()
    {
        // Arrange
        var lineString = new LineString(
            [
                new Coordinate(0, 0),
                new Coordinate(1, 1), // Collinear with adjacent points
                new Coordinate(2, 2),
                new Coordinate(3, 3), // Collinear with adjacent points
                new Coordinate(4, 4),
            ]
        );

        // Act
        var cleaned = Territory.CleanCoords(lineString);

        // Assert
        cleaned.Coordinates.Length.ShouldBe(2);
        cleaned.Coordinates[0].X.ShouldBe(0);
        cleaned.Coordinates[0].Y.ShouldBe(0);
        cleaned.Coordinates[1].X.ShouldBe(4);
        cleaned.Coordinates[1].Y.ShouldBe(4);
    }

    [Fact]
    public void RemoveDuplicatePointsFromPolygon()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 0), // Duplicate
                    new Coordinate(0, 5),
                    new Coordinate(5, 5),
                    new Coordinate(5, 5), // Duplicate
                    new Coordinate(5, 0),
                    new Coordinate(0, 0),
                ]
            )
        );

        // Act
        var cleaned = (Polygon)Territory.CleanCoords(polygon);

        // Assert
        cleaned.ExteriorRing.Coordinates.Length.ShouldBe(5);
        cleaned.ExteriorRing.Coordinates[0].X.ShouldBe(0);
        cleaned.ExteriorRing.Coordinates[0].Y.ShouldBe(0);
        cleaned.ExteriorRing.Coordinates[1].X.ShouldBe(0);
        cleaned.ExteriorRing.Coordinates[1].Y.ShouldBe(5);
        cleaned.ExteriorRing.Coordinates[2].X.ShouldBe(5);
        cleaned.ExteriorRing.Coordinates[2].Y.ShouldBe(5);
        cleaned.ExteriorRing.Coordinates[3].X.ShouldBe(5);
        cleaned.ExteriorRing.Coordinates[3].Y.ShouldBe(0);
        cleaned.ExteriorRing.Coordinates[4].X.ShouldBe(0);
        cleaned.ExteriorRing.Coordinates[4].Y.ShouldBe(0);
    }

    [Fact]
    public void RemoveCollinearPointsFromPolygon()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 5),
                    new Coordinate(2.5, 5), // Collinear
                    new Coordinate(5, 5),
                    new Coordinate(5, 2.5), // Collinear
                    new Coordinate(5, 0),
                    new Coordinate(2.5, 0), // Collinear
                    new Coordinate(0, 0),
                ]
            )
        );

        // Act
        var cleaned = (Polygon)Territory.CleanCoords(polygon);

        // Assert
        cleaned.ExteriorRing.Coordinates.Length.ShouldBe(5);
        cleaned.ExteriorRing.Coordinates[0].X.ShouldBe(0);
        cleaned.ExteriorRing.Coordinates[0].Y.ShouldBe(0);
        cleaned.ExteriorRing.Coordinates[1].X.ShouldBe(0);
        cleaned.ExteriorRing.Coordinates[1].Y.ShouldBe(5);
        cleaned.ExteriorRing.Coordinates[2].X.ShouldBe(5);
        cleaned.ExteriorRing.Coordinates[2].Y.ShouldBe(5);
        cleaned.ExteriorRing.Coordinates[3].X.ShouldBe(5);
        cleaned.ExteriorRing.Coordinates[3].Y.ShouldBe(0);
        cleaned.ExteriorRing.Coordinates[4].X.ShouldBe(0);
        cleaned.ExteriorRing.Coordinates[4].Y.ShouldBe(0);
    }

    [Fact]
    public void CleanPolygonWithHole()
    {
        // Arrange
        var exteriorRing = new LinearRing(
            [
                new Coordinate(0, 0),
                new Coordinate(0, 10),
                new Coordinate(5, 10), // Collinear
                new Coordinate(10, 10),
                new Coordinate(10, 0),
                new Coordinate(0, 0),
            ]
        );

        var interiorRing = new LinearRing(
            [
                new Coordinate(2, 2),
                new Coordinate(2, 4),
                new Coordinate(3, 4), // Collinear
                new Coordinate(4, 4),
                new Coordinate(4, 2),
                new Coordinate(2, 2),
            ]
        );

        var polygon = new Polygon(exteriorRing, [interiorRing]);

        // Act
        var cleaned = (Polygon)Territory.CleanCoords(polygon);

        // Assert
        cleaned.ExteriorRing.Coordinates.Length.ShouldBe(5);
        cleaned.NumInteriorRings.ShouldBe(1);
        cleaned.GetInteriorRingN(0).Coordinates.Length.ShouldBe(5);
    }

    [Fact]
    public void CleanMultiPoint()
    {
        // Arrange
        var multiPoint = new MultiPoint(
            [
                new Point(0, 0),
                new Point(0, 0), // Duplicate
                new Point(1, 1),
                new Point(2, 2),
                new Point(2, 2), // Duplicate
            ]
        );

        // Act
        var cleaned = (MultiPoint)Territory.CleanCoords(multiPoint);

        // Assert
        cleaned.NumGeometries.ShouldBe(3);
        ((Point)cleaned.GetGeometryN(0)).X.ShouldBe(0);
        ((Point)cleaned.GetGeometryN(0)).Y.ShouldBe(0);
        ((Point)cleaned.GetGeometryN(1)).X.ShouldBe(1);
        ((Point)cleaned.GetGeometryN(1)).Y.ShouldBe(1);
        ((Point)cleaned.GetGeometryN(2)).X.ShouldBe(2);
        ((Point)cleaned.GetGeometryN(2)).Y.ShouldBe(2);
    }
}
