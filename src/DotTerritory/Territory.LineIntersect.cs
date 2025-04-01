using NetTopologySuite.Geometries;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes two lines and finds their intersection point(s).
    /// </summary>
    /// <param name="line1">First line</param>
    /// <param name="line2">Second line</param>
    /// <returns>A GeometryCollection of Point(s) that represent the intersection(s) of the lines</returns>
    /// <example>
    /// <code>
    /// var line1 = geometryFactory.CreateLineString([
    ///     new Coordinate(0, 0),
    ///     new Coordinate(2, 2)
    /// ]);
    /// var line2 = geometryFactory.CreateLineString([
    ///     new Coordinate(0, 2),
    ///     new Coordinate(2, 0)
    /// ]);
    /// var intersections = Territory.LineIntersect(line1, line2);
    /// // => GeometryCollection containing a single point (1, 1)
    /// </code>
    /// </example>
    public static GeometryCollection LineIntersect(LineString line1, LineString line2)
    {
        if (line1 == null || line2 == null || line1.IsEmpty || line2.IsEmpty)
        {
            var geomFactory = line1?.Factory ?? line2?.Factory ?? new GeometryFactory();
            return geomFactory.CreateGeometryCollection();
        }

        var intersection = line1.Intersection(line2);

        // The result could be a Point, MultiPoint, LineString, or empty geometry
        // Convert to GeometryCollection of points
        return intersection switch
        {
            Point point => line1.Factory.CreateGeometryCollection([point]),
            MultiPoint multiPoint
                => line1.Factory.CreateGeometryCollection(
                    Enumerable
                        .Range(0, multiPoint.NumGeometries)
                        .Select(i => multiPoint.GetGeometryN(i))
                        .ToArray()
                ),
            LineString lineString => ExtractPointsFromLineString(lineString),
            GeometryCollection collection => ExtractPointsFromCollection(collection),
            _ => line1.Factory.CreateGeometryCollection()
        };
    }

    /// <summary>
    /// Takes any number of lines and finds their intersection point(s).
    /// </summary>
    /// <param name="lines">Array of LineStrings</param>
    /// <returns>A GeometryCollection of Point(s) that represent the intersection(s) of the lines</returns>
    /// <example>
    /// <code>
    /// var line1 = geometryFactory.CreateLineString([
    ///     new Coordinate(0, 0),
    ///     new Coordinate(2, 2)
    /// ]);
    /// var line2 = geometryFactory.CreateLineString([
    ///     new Coordinate(0, 2),
    ///     new Coordinate(2, 0)
    /// ]);
    /// var line3 = geometryFactory.CreateLineString([
    ///     new Coordinate(0, 1),
    ///     new Coordinate(2, 1)
    /// ]);
    /// var intersections = Territory.LineIntersect([line1, line2, line3]);
    /// // => GeometryCollection containing points (1, 1), (0.5, 0.5), (1.5, 1.5)
    /// </code>
    /// </example>
    public static GeometryCollection LineIntersect(params LineString[] lines)
    {
        if (lines == null || lines.Length < 2)
        {
            var geomFactory = lines?.FirstOrDefault()?.Factory ?? new GeometryFactory();
            return geomFactory.CreateGeometryCollection();
        }

        // Calculate intersections between all pairs of lines
        var factory = lines[0].Factory;
        var points = new List<Point>();

        for (int i = 0; i < lines.Length - 1; i++)
        {
            if (lines[i] == null || lines[i].IsEmpty)
                continue;

            for (int j = i + 1; j < lines.Length; j++)
            {
                if (lines[j] == null || lines[j].IsEmpty)
                    continue;

                var intersection = LineIntersect(lines[i], lines[j]);

                // Add all points from the intersection result
                for (int k = 0; k < intersection.NumGeometries; k++)
                {
                    var point = (Point)intersection.GetGeometryN(k);
                    // Check if point is already in the list
                    if (!points.Any(p => p.Coordinate.Equals2D(point.Coordinate)))
                    {
                        points.Add(point);
                    }
                }
            }
        }

        return factory.CreateGeometryCollection([.. points]);
    }

    // Helper method to extract points from a LineString
    private static GeometryCollection ExtractPointsFromLineString(LineString lineString)
    {
        if (lineString == null || lineString.IsEmpty)
        {
            var geomFactory = lineString?.Factory ?? new GeometryFactory();
            return geomFactory.CreateGeometryCollection();
        }

        var factory = lineString.Factory;
        var coordinates = lineString.Coordinates;
        var points = coordinates.Select(c => factory.CreatePoint(c)).ToArray();

        return factory.CreateGeometryCollection(points);
    }

    // Helper method to extract points from a GeometryCollection
    private static GeometryCollection ExtractPointsFromCollection(GeometryCollection collection)
    {
        if (collection == null || collection.IsEmpty)
        {
            var geomFactory = collection?.Factory ?? new GeometryFactory();
            return geomFactory.CreateGeometryCollection();
        }

        var factory = collection.Factory;
        var points = new List<Point>();

        // Iterate through each geometry in the collection
        for (int i = 0; i < collection.NumGeometries; i++)
        {
            var geometry = collection.GetGeometryN(i);

            switch (geometry)
            {
                case Point point:
                    // If the geometry is a point, add it directly
                    points.Add(point);
                    break;

                case LineString lineString:
                    // If the geometry is a line, extract points from it
                    var linePoints = ExtractPointsFromLineString(lineString);
                    for (int j = 0; j < linePoints.NumGeometries; j++)
                    {
                        points.Add((Point)linePoints.GetGeometryN(j));
                    }
                    break;

                case MultiPoint multiPoint:
                    // If the geometry is a multipoint, add all its points
                    for (int j = 0; j < multiPoint.NumGeometries; j++)
                    {
                        points.Add((Point)multiPoint.GetGeometryN(j));
                    }
                    break;

                case GeometryCollection nestedCollection:
                    // Handle nested collections recursively
                    if (nestedCollection != collection) // Avoid infinite recursion
                    {
                        var nestedPoints = ExtractPointsFromCollection(nestedCollection);
                        for (int j = 0; j < nestedPoints.NumGeometries; j++)
                        {
                            points.Add((Point)nestedPoints.GetGeometryN(j));
                        }
                    }
                    break;
            }
        }

        // Remove duplicate points by comparing coordinates
        var distinctPoints = new List<Point>();
        foreach (var point in points)
        {
            if (!distinctPoints.Any(p => p.Coordinate.Equals2D(point.Coordinate)))
            {
                distinctPoints.Add(point);
            }
        }

        return factory.CreateGeometryCollection(distinctPoints.ToArray());
    }
}
