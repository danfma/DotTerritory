using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class BooleanEqualTest
{
    [Fact]
    public void IdenticalPolygonsShouldBeEqual()
    {
        // Arrange
        var polygon1 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 5),
                    new Coordinate(5, 5),
                    new Coordinate(5, 0),
                    new Coordinate(0, 0),
                ]
            )
        );
        
        var polygon2 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 5),
                    new Coordinate(5, 5),
                    new Coordinate(5, 0),
                    new Coordinate(0, 0),
                ]
            )
        );
        
        // Act
        var result = Territory.BooleanEqual(polygon1, polygon2);
        
        // Assert
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void DifferentPolygonsShouldNotBeEqual()
    {
        // Arrange
        var polygon1 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 5),
                    new Coordinate(5, 5),
                    new Coordinate(5, 0),
                    new Coordinate(0, 0),
                ]
            )
        );
        
        var polygon2 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(1, 1),
                    new Coordinate(1, 6),
                    new Coordinate(6, 6),
                    new Coordinate(6, 1),
                    new Coordinate(1, 1),
                ]
            )
        );
        
        // Act
        var result = Territory.BooleanEqual(polygon1, polygon2);
        
        // Assert
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void SameGeometryWithSamePointOrderShouldBeEqual()
    {
        // Arrange
        var polygon1 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 5),
                    new Coordinate(5, 5),
                    new Coordinate(5, 0),
                    new Coordinate(0, 0),
                ]
            )
        );
        
        var polygon2 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 5),
                    new Coordinate(5, 5),
                    new Coordinate(5, 0),
                    new Coordinate(0, 0),
                ]
            )
        );
        
        // Act
        var result = Territory.BooleanEqual(polygon1, polygon2);
        
        // Assert
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void SamePolygonWithDifferentPointOrderShouldNotBeEqual()
    {
        // Arrange
        var polygon1 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 5),
                    new Coordinate(5, 5),
                    new Coordinate(5, 0),
                    new Coordinate(0, 0),
                ]
            )
        );
        
        var polygon2 = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(5, 0),
                    new Coordinate(5, 5),
                    new Coordinate(0, 5),
                    new Coordinate(0, 0),
                ]
            )
        );
        
        // Act
        var result = Territory.BooleanEqual(polygon1, polygon2);
        
        // Assert
        // BooleanEqual uses EqualsExact, which requires the exact same coordinate order
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void IdenticalPointsShouldBeEqual()
    {
        // Arrange
        var point1 = new Point(5, 5);
        var point2 = new Point(5, 5);
        
        // Act
        var result = Territory.BooleanEqual(point1, point2);
        
        // Assert
        result.ShouldBeTrue();
    }
    
    [Fact]
    public void DifferentPointsShouldNotBeEqual()
    {
        // Arrange
        var point1 = new Point(5, 5);
        var point2 = new Point(6, 6);
        
        // Act
        var result = Territory.BooleanEqual(point1, point2);
        
        // Assert
        result.ShouldBeFalse();
    }
}