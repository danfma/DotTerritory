using NetTopologySuite.Geometries;
using NetTopologySuite.Features;

namespace DotTerritory.Tests;

public class BooleanPointInPolygonTest
{
    [Fact]
    public void PointInPolygonShouldReturnTrue()
    {
        // Arrange
        var polygon = new Polygon(
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
        
        var point = new Point(2.5, 2.5);
        
        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon);
        
        // Assert
        result.ShouldBeTrue();
        
        // Verify with NTS directly for consistency
        polygon.Contains(point).ShouldBeTrue();
    }
    
    [Fact]
    public void PointOutsidePolygonShouldReturnFalse()
    {
        // Arrange
        var polygon = new Polygon(
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
        
        var point = new Point(10, 10);
        
        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon);
        
        // Assert
        result.ShouldBeFalse();
        
        // Verify with NTS directly for consistency
        polygon.Contains(point).ShouldBeFalse();
    }
    
    [Fact]
    public void PointOnBoundaryShouldReturnTrue()
    {
        // Arrange
        var polygon = new Polygon(
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
        
        var point = new Point(0, 2.5);
        
        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon);
        
        // Assert
        result.ShouldBeTrue();
        
        // Verify boundary position - point should be on boundary
        polygon.Boundary.Distance(point).ShouldBeLessThan(double.Epsilon);
    }
    
    [Fact]
    public void PointOnBoundaryWithIgnoreBoundaryShouldReturnFalse()
    {
        // Arrange
        var polygon = new Polygon(
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
        
        var point = new Point(0, 2.5);
        
        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon, ignoreBoundary: true);
        
        // Assert
        result.ShouldBeFalse();
        
        // Verify boundary position - point should be on boundary
        polygon.Boundary.Distance(point).ShouldBeLessThan(double.Epsilon);
    }
    
    [Fact]
    public void PointInHoleShouldReturnFalse()
    {
        // Arrange
        var exteriorRing = new LinearRing(
            [
                new Coordinate(0, 0),
                new Coordinate(0, 10),
                new Coordinate(10, 10),
                new Coordinate(10, 0),
                new Coordinate(0, 0),
            ]
        );
        
        var interiorRing = new LinearRing(
            [
                new Coordinate(2, 2),
                new Coordinate(2, 4),
                new Coordinate(4, 4),
                new Coordinate(4, 2),
                new Coordinate(2, 2),
            ]
        );
        
        var polygon = new Polygon(exteriorRing, [interiorRing]);
        var point = new Point(3, 3);
        
        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon);
        
        // Assert
        result.ShouldBeFalse();
        
        // Verify with NTS directly for consistency
        polygon.Contains(point).ShouldBeFalse();
        
        // Additional verification that point is inside exterior but also inside hole
        new Polygon(exteriorRing).Contains(point).ShouldBeTrue();
        new Polygon(interiorRing).Contains(point).ShouldBeTrue();
    }
    
    [Fact]
    public void PointOnHoleBoundaryShouldReturnTrue()
    {
        // Arrange
        var exteriorRing = new LinearRing(
            [
                new Coordinate(0, 0),
                new Coordinate(0, 10),
                new Coordinate(10, 10),
                new Coordinate(10, 0),
                new Coordinate(0, 0),
            ]
        );
        
        var interiorRing = new LinearRing(
            [
                new Coordinate(2, 2),
                new Coordinate(2, 4),
                new Coordinate(4, 4),
                new Coordinate(4, 2),
                new Coordinate(2, 2),
            ]
        );
        
        var polygon = new Polygon(exteriorRing, [interiorRing]);
        var point = new Point(2, 3); // On the hole boundary
        
        // Act
        var result = Territory.BooleanPointInPolygon(point, polygon);
        
        // Assert
        result.ShouldBeTrue();
        
        // Verify boundary position - point should be on boundary
        polygon.Boundary.Distance(point).ShouldBeLessThan(double.Epsilon);
    }
    
    [Fact]
    public void PointInMultiPolygonShouldReturnTrue()
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
                    new Coordinate(10, 10),
                    new Coordinate(10, 15),
                    new Coordinate(15, 15),
                    new Coordinate(15, 10),
                    new Coordinate(10, 10),
                ]
            )
        );
        
        var multiPolygon = new MultiPolygon([polygon1, polygon2]);
        var point = new Point(12.5, 12.5);
        
        // Act
        var result = Territory.BooleanPointInPolygon(point, multiPolygon);
        
        // Assert
        result.ShouldBeTrue();
        
        // Verify that point is only inside second polygon
        polygon1.Contains(point).ShouldBeFalse();
        polygon2.Contains(point).ShouldBeTrue();
    }
    
    [Fact]
    public void ComplexConcavePolygonShouldHandlePointsCorrectly()
    {
        // Arrange - Create a concave polygon (C-shape)
        var polygon = new Polygon(
            new LinearRing(
                [
                    new Coordinate(0, 0),
                    new Coordinate(0, 10),
                    new Coordinate(10, 10),
                    new Coordinate(10, 8),
                    new Coordinate(2, 8),
                    new Coordinate(2, 2),
                    new Coordinate(10, 2),
                    new Coordinate(10, 0),
                    new Coordinate(0, 0),
                ]
            )
        );
        
        // Points to test
        var insidePoint = new Point(1, 1);
        var insideConcavityPoint = new Point(5, 5);
        var outsidePoint = new Point(15, 15);
        
        // Act & Assert
        Territory.BooleanPointInPolygon(insidePoint, polygon).ShouldBeTrue();
        Territory.BooleanPointInPolygon(insideConcavityPoint, polygon).ShouldBeFalse();
        Territory.BooleanPointInPolygon(outsidePoint, polygon).ShouldBeFalse();
        
        // Verify with NTS directly for consistency
        polygon.Contains(insidePoint).ShouldBeTrue();
        polygon.Contains(insideConcavityPoint).ShouldBeFalse();
        polygon.Contains(outsidePoint).ShouldBeFalse();
    }
    
    [Fact]
    public void NullGeometriesShouldThrowArgumentNullException()
    {
        // Arrange
        var point = new Point(1, 1);
        Polygon? nullPolygon = null;
        
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            Territory.BooleanPointInPolygon(point, nullPolygon!));
            
        Should.Throw<ArgumentNullException>(() => 
            Territory.BooleanPointInPolygon(null!, new Polygon(new LinearRing([]))));
    }
    
    [Fact]
    public void FeatureInterfaceShouldWorkCorrectly()
    {
        // Arrange
        var polygon = new Polygon(
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
        
        var pointFeature = new Feature(new Point(2.5, 2.5), new AttributesTable());
        var polygonFeature = new Feature(polygon, new AttributesTable());
        
        // Act
        var result = Territory.BooleanPointInPolygon(pointFeature, polygonFeature);
        
        // Assert
        result.ShouldBeTrue();
    }
}