using NetTopologySuite.Geometries;
using UnitsNet;
using UnitsNet.Units;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Returns the minimum distance between a point and a polygon or multi-polygon.
    /// </summary>
    /// <param name="point">The point</param>
    /// <param name="polygon">The polygon or multi-polygon</param>
    /// <param name="units">The units of the returned distance</param>
    /// <returns>The minimum distance between the point and the polygon</returns>
    /// <example>
    /// <code>
    /// var pt = new Point(new Coordinate(0, 0));
    /// var polygon = GeometryFactory.CreatePolygon(new[] {
    ///     new Coordinate(1, 1),
    ///     new Coordinate(1, 2),
    ///     new Coordinate(2, 2),
    ///     new Coordinate(2, 1),
    ///     new Coordinate(1, 1)
    /// });
    /// var distance = Territory.PointToPolygonDistance(pt, polygon, LengthUnit.Kilometer);
    /// </code>
    /// </example>
    public static Length PointToPolygonDistance(
        Point point,
        Polygon polygon,
        LengthUnit units = LengthUnit.Meter
    )
    {
        // If the point is inside the polygon, the distance is 0
        if (polygon.Contains(point))
        {
            return Length.Zero;
        }

        // Calculate distance to the boundary
        return PointToLineDistance(point, polygon.ExteriorRing, units);
    }

    /// <summary>
    /// Returns the minimum distance between a point and a polygon or multi-polygon.
    /// </summary>
    /// <param name="point">The point coordinates</param>
    /// <param name="polygon">The polygon or multi-polygon</param>
    /// <param name="units">The units of the returned distance</param>
    /// <returns>The minimum distance between the point and the polygon</returns>
    public static Length PointToPolygonDistance(
        Coordinate point,
        Polygon polygon,
        LengthUnit units = LengthUnit.Meter
    )
    {
        return PointToPolygonDistance(new Point(point), polygon, units);
    }

    /// <summary>
    /// Returns the minimum distance between a point and a multi-polygon.
    /// </summary>
    /// <param name="point">The point</param>
    /// <param name="multiPolygon">The multi-polygon</param>
    /// <param name="units">The units of the returned distance</param>
    /// <returns>The minimum distance between the point and the multi-polygon</returns>
    public static Length PointToPolygonDistance(
        Point point,
        MultiPolygon multiPolygon,
        LengthUnit units = LengthUnit.Meter
    )
    {
        // If the point is inside any polygon, the distance is 0
        if (multiPolygon.Contains(point))
        {
            return Length.Zero;
        }

        var minDistance = double.MaxValue;

        // Calculate minimum distance to any polygon in the multi-polygon
        for (int i = 0; i < multiPolygon.NumGeometries; i++)
        {
            var polygon = (Polygon)multiPolygon.GetGeometryN(i);
            var distance = PointToPolygonDistance(point, polygon, units);

            if (distance.Value < minDistance)
            {
                minDistance = distance.Value;
            }
        }

        return Length.From(minDistance, units);
    }

    /// <summary>
    /// Returns the minimum distance between a point and a multi-polygon.
    /// </summary>
    /// <param name="point">The point coordinates</param>
    /// <param name="multiPolygon">The multi-polygon</param>
    /// <param name="units">The units of the returned distance</param>
    /// <returns>The minimum distance between the point and the multi-polygon</returns>
    public static Length PointToPolygonDistance(
        Coordinate point,
        MultiPolygon multiPolygon,
        LengthUnit units = LengthUnit.Meter
    )
    {
        return PointToPolygonDistance(new Point(point), multiPolygon, units);
    }
}
