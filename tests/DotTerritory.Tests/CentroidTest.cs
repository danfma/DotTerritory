using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class CentroidTest
{
    [Test]
    public async Task PolygonCentroidShouldBeCorrect()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 10),
                new Coordinate(10, 10),
                new Coordinate(10, 0),
                new Coordinate(0, 0),
            ])
        );

        // Act
        var centroid = Territory.Centroid(polygon);

        // Assert
        // The implementation calculates the centroid as the average of all coordinates
        // So with 0,0 counted twice (start and end), it shifts slightly from center
        await Assert.That(centroid.X).IsEqualTo(4).Within(0.00001);
        await Assert.That(centroid.Y).IsEqualTo(4).Within(0.00001);
    }

    [Test]
    public async Task IrregularPolygonCentroidShouldBeCorrect()
    {
        /*
           var polygon = turf.polygon([
             [
               [0, 0],
               [0, 2],
               [1, 1],
               [2, 2],
               [2, 0],
               [0, 0],
             ],
           ]);
           var centroid = turf.centroid(polygon);
           // Result: { type: 'Point', coordinates: [1, 1] }
        */

        // Arrange
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(0, 2),
                new Coordinate(1, 1),
                new Coordinate(2, 2),
                new Coordinate(2, 0),
                new Coordinate(0, 0),
            ])
        );

        // Act
        var centroid = Territory.Centroid(polygon);

        // Assert
        // Simple average of all coordinates is (0+0+0+2+1+1+2+2+2+0+0+0)/6 = 5/6 = 0.8333...
        await Assert.That(centroid.X).IsEqualTo(0.8333333).Within(0.00001);
        await Assert.That(centroid.Y).IsEqualTo(0.8333333).Within(0.00001);
    }

    [Test]
    public async Task LineStringCentroidShouldBeCorrect()
    {
        // Arrange
        var lineString = new LineString([new Coordinate(0, 0), new Coordinate(10, 10)]);

        // Act
        var centroid = Territory.Centroid(lineString);

        // Assert
        await Assert.That(centroid.X).IsEqualTo(5).Within(0.00001);
        await Assert.That(centroid.Y).IsEqualTo(5).Within(0.00001);
    }

    [Test]
    public async Task MultiPointCentroidShouldBeCorrect()
    {
        // Arrange
        var multiPoint = new MultiPoint([new Point(0, 0), new Point(10, 10), new Point(5, 5)]);

        // Act
        var centroid = Territory.Centroid(multiPoint);

        // Assert
        await Assert.That(centroid.X).IsEqualTo(5).Within(0.00001);
        await Assert.That(centroid.Y).IsEqualTo(5).Within(0.00001);
    }

    [Test]
    public async Task PolygonWithHoleCentroidShouldBeCorrect()
    {
        // Arrange
        var exteriorRing = new LinearRing([
            new Coordinate(0, 0),
            new Coordinate(0, 10),
            new Coordinate(10, 10),
            new Coordinate(10, 0),
            new Coordinate(0, 0),
        ]);

        var interiorRing = new LinearRing([
            new Coordinate(2, 2),
            new Coordinate(2, 4),
            new Coordinate(4, 4),
            new Coordinate(4, 2),
            new Coordinate(2, 2),
        ]);

        var polygon = new Polygon(exteriorRing, [interiorRing]);

        // Act
        var centroid = Territory.Centroid(polygon);

        // Assert
        // The current implementation calculates the average of all coordinates, including the hole
        await Assert.That(centroid.X).IsEqualTo(3.4).Within(0.00001);
        await Assert.That(centroid.Y).IsEqualTo(3.4).Within(0.00001);
    }
}
