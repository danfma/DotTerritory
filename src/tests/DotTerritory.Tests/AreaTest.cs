using FluentAssertions;
using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class AreaTest
{
    [Fact]
    public void AreaCalculationShouldMatchTurfArea()
    {
        /*
         const p = turf.polygon(
               [
                   [
                       [-5, 52],
                       [-4, 56],
                       [-2, 51],
                       [-7, 54],
                       [-5, 52],
                   ],
               ],
               {name: "poly1"},
           )
           
           console.log('area', turf.area(p)) // 32819945055.137398 m²
         */

        var polygon = new Polygon(
            new LinearRing(
                [
                    new Coordinate(-5, 52),
                    new Coordinate(-4, 56),
                    new Coordinate(-2, 51),
                    new Coordinate(-7, 54),
                    new Coordinate(-5, 52),
                ]
            )
        );

        var area = Territory.Area(polygon);

        area.SquareMeters.Should().BeApproximately(32819945055.137398, 0.001);
    }
}
