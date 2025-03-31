using NetTopologySuite.Geometries;

namespace DotTerritory.Tests;

public class ProjectionTest
{
    [Fact]
    public void ConvertToAndFromMercatorShouldMatchTurfJs()
    {
        var point = new Point(-71, 41);
        var mercatorPoint = Territory.ToMercator(point);
        var wgs84Point = Territory.ToWgs84(mercatorPoint);

        wgs84Point.X.ShouldBe(point.X, tolerance: 0.0001);
        wgs84Point.Y.ShouldBe(point.Y, tolerance: 0.0001);

        mercatorPoint.X.ShouldBe(-7903683.846322424, tolerance: 0.001);
        mercatorPoint.Y.ShouldBe(5012341.663847514, tolerance: 0.001);
    }

    [Fact]
    public void ConvertLineStringToAndFromMercator()
    {
        double[][] coordinates =
        [
            [-122.402565479, 37.800018586],
            [-122.40309119224547, 37.80270111774213],
            [-122.403531075, 37.804935338],
            [-122.403230667, 37.804918385],
            [-122.401835918, 37.803782474],
            [-122.394883633, 37.795830611],
            [-122.394475937, 37.795152374],
            [-122.39389658, 37.794643693],
            [-122.393059731, 37.794135007],
            [-122.392566204, 37.793677188],
            [-122.39409606903791, 37.792539514596676],
            [-122.395892143, 37.791201521],
            [-122.397587299, 37.792558061],
            [-122.39928849041462, 37.79237021335458],
            [-122.400956154, 37.792185015],
            [-122.401750088, 37.795966258],
            [-122.402565479, 37.800018586]
        ];

        double[][] mercatorCoordinates =
        [
            [-13625791.260912606, 4551213.538145592],
            [-13625849.783043394, 4551591.4685018705],
            [-13625898.75056764, 4551906.248659134],
            [-13625865.309302049, 4551903.860108546],
            [-13625710.046553584, 4551743.820014667],
            [-13624936.121727534, 4550623.53980465],
            [-13624890.737216415, 4550527.993502388],
            [-13624826.243490187, 4550456.333893487],
            [-13624733.085885637, 4550384.674073626],
            [-13624678.146711305, 4550320.180432205],
            [-13624848.450508308, 4550159.916423304],
            [-13625048.388547193, 4549971.436474123],
            [-13625237.092449928, 4550162.529034647],
            [-13625426.468211947, 4550136.067174733],
            [-13625612.111673085, 4550109.978582927],
            [-13625700.492001688, 4550642.649114012],
            [-13625791.260912606, 4551213.538145592]
        ];

        var line = new LineString(coordinates.Select(c => new Coordinate(c[0], c[1])).ToArray());
        var mercatorLine = Territory.ToMercator(line);
        var wgs84Line = Territory.ToWgs84(mercatorLine);

        for (var i = 0; i < coordinates.Length; i++)
        {
            wgs84Line.Coordinates[i].X.ShouldBe(coordinates[i][0], tolerance: 0.0001);
            wgs84Line.Coordinates[i].Y.ShouldBe(coordinates[i][1], tolerance: 0.0001);
            mercatorLine.Coordinates[i].X.ShouldBe(mercatorCoordinates[i][0], tolerance: 0.001);
            mercatorLine.Coordinates[i].Y.ShouldBe(mercatorCoordinates[i][1], tolerance: 0.001);
        }
    }
}
