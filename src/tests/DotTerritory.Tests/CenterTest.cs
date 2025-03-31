using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class CenterTest
{
    [Fact]
    public void PolygonCenterShouldBeCorrect()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 10),
                    new Coordinate(10, 10),
                    new Coordinate(10, 0),
                    new Coordinate(0, 0),
                ]
            )
        );
        
        // Act
        var center = Territory.Center(polygon);
        
        // Assert
        center.X.ShouldBe(5);
        center.Y.ShouldBe(5);
    }
    
    [Fact]
    public void LineStringCenterShouldBeCorrect()
    {
        // Arrange
        var lineString = new LineString(
            [
                new Coordinate(0, 0),
                new Coordinate(10, 10),
            ]
        );
        
        // Act
        var center = Territory.Center(lineString);
        
        // Assert
        center.X.ShouldBe(5);
        center.Y.ShouldBe(5);
    }
    
    [Fact]
    public void PointCenterShouldBeTheSamePoint()
    {
        // Arrange
        var point = new Point(5, 5);
        
        // Act
        var center = Territory.Center(point);
        
        // Assert
        center.X.ShouldBe(5);
        center.Y.ShouldBe(5);
    }
    
    [Fact]
    public void MultiPointCenterShouldBeCorrect()
    {
        // Arrange
        var points = new MultiPoint(
            [
                new Point(0, 0),
                new Point(10, 10),
            ]
        );
        
        // Act
        var center = Territory.Center(points);
        
        // Assert
        center.X.ShouldBe(5);
        center.Y.ShouldBe(5);
    }
    
    [Fact]
    public void IrregularPolygonCenterShouldBeCorrect()
    {
        // Arrange
        var polygon = new Polygon(
            new LinearRing(
                [
                    new Coordinate(-10, -10),
                    new Coordinate(-10, 10),
                    new Coordinate(10, 10),
                    new Coordinate(10, -10),
                    new Coordinate(-10, -10),
                ]
            )
        );
        
        // Act
        var center = Territory.Center(polygon);
        
        // Assert
        center.X.ShouldBe(0);
        center.Y.ShouldBe(0);
    }
}