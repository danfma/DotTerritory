namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a geometry and returns a GeometryCollection of all points contained in the geometry.
    /// </summary>
    /// <param name="geom">Any geometry type</param>
    /// <returns>A GeometryCollection of points</returns>
    /// <example>
    /// <code>
    /// var line = geometryFactory.CreateLineString([
    ///     new Coordinate(0, 0),
    ///     new Coordinate(1, 1),
    ///     new Coordinate(2, 2)
    /// ]);
    /// var points = Territory.Explode(line);
    /// // points is a GeometryCollection containing 3 Point geometries
    /// </code>
    /// </example>
    public static GeometryCollection Explode(Geometry geom)
    {
        var geometryFactory = geom.Factory;
        var coords = GetCoordinates(geom);
        var points = coords.Select(c => geometryFactory.CreatePoint(c)).ToArray();
        return geometryFactory.CreateGeometryCollection(points);
    }

    // Helper method to get all coordinates from any geometry type
    private static Coordinate[] GetCoordinates(Geometry geom)
    {
        if (geom == null || geom.IsEmpty)
            return [];

        return geom switch
        {
            Point point => [point.Coordinate],
            LineString line => line.Coordinates,
            Polygon polygon => polygon.Coordinates,
            MultiPoint multiPoint => multiPoint.Coordinates,
            MultiLineString multiLine => multiLine.Coordinates,
            MultiPolygon multiPolygon => multiPolygon.Coordinates,
            GeometryCollection collection => GetCoordinatesFromCollection(collection),
            _ => [],
        };
    }

    private static Coordinate[] GetCoordinatesFromCollection(GeometryCollection collection)
    {
        if (collection == null || collection.IsEmpty)
            return [];

        var coordinates = new List<Coordinate>();

        for (var i = 0; i < collection.NumGeometries; i++)
        {
            var coords = GetCoordinates(collection.GetGeometryN(i));
            coordinates.AddRange(coords);
        }

        return [.. coordinates];
    }
}
