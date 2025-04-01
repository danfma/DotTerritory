namespace DotTerritory;

public static partial class Territory
{
    public static Geometry BBoxClip(Geometry geometry, BBox bbox)
    {
        return geometry switch
        {
            LineString lineString => BBoxClipLineString(lineString, bbox),
            MultiLineString multiLineString => BBoxClipMultiLineString(multiLineString, bbox),
            Polygon polygon => BBoxClipPolygon(polygon, bbox),
            MultiPolygon multiPolygon => BBoxClipMultiPolygon(multiPolygon, bbox),
            _ => RaiseNotSupported(geometry)
        };
    }

    private static Geometry RaiseNotSupported(Geometry geometry)
    {
        throw new NotSupportedException($"Geometry type {geometry.GeometryType} is not supported");
    }

    public static LineString BBoxClipLineString(LineString lineString, BBox bbox)
    {
        var points = lineString.Coordinates;
        var length = points.Length;

        if (length < 2)
            return lineString;

        var codeA = BitCode(points[0], bbox);
        var part = new List<Coordinate>();
        var result = new List<List<Coordinate>>();

        for (var i = 1; i < length; i++)
        {
            var a = points[i - 1];
            var b = points[i];
            var codeB = BitCode(b, bbox);
            var lastCode = codeB;

            while (true)
            {
                if ((codeA | codeB) == 0)
                {
                    // both points are inside the bbox
                    part.Add(a);

                    if (codeB != lastCode)
                    {
                        part.Add(b);

                        if (i < length - 1)
                        {
                            result.Add(part);
                            part = new List<Coordinate>();
                        }
                    }
                    else if (i == length - 1)
                    {
                        part.Add(b);
                    }

                    break;
                }

                if ((codeA & codeB) != 0)
                {
                    // both points are outside the bbox
                    break;
                }

                byte codeOut;
                if (codeA != 0)
                {
                    codeOut = codeA;
                    var intersection = Intersect(a, b, codeOut, bbox);
                    if (intersection != null)
                    {
                        a = intersection;
                        codeA = BitCode(a, bbox);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    codeOut = codeB;
                    var intersection = Intersect(a, b, codeOut, bbox);
                    if (intersection != null)
                    {
                        b = intersection;
                        codeB = BitCode(b, bbox);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            codeA = lastCode;
        }

        if (part.Count > 0)
        {
            result.Add(part);
        }

        if (result.Count == 0)
            return new LineString(Array.Empty<Coordinate>());

        if (result.Count == 1)
            return new LineString(result[0].ToArray());

        return new LineString(result.SelectMany(x => x).ToArray());
    }

    public static MultiLineString BBoxClipMultiLineString(
        MultiLineString multiLineString,
        BBox bbox
    )
    {
        var lines = new List<LineString>();

        foreach (var geom in multiLineString.Geometries)
        {
            var line = (LineString)geom;
            var clipped = BBoxClipLineString(line, bbox);

            if (!clipped.IsEmpty)
            {
                lines.Add(clipped);
            }
        }

        return new MultiLineString(lines.ToArray());
    }

    public static Polygon BBoxClipPolygon(Polygon polygon, BBox bbox)
    {
        // Apply Sutherland-Hodgman algorithm to clip the polygon
        var rings = new List<LinearRing>();

        // Clip the exterior ring
        var exteriorRing = polygon.ExteriorRing;
        var clippedExteriorRing = PolygonClip(exteriorRing.CoordinateSequence, bbox);

        if (clippedExteriorRing.Count < 4) // Not enough points for a valid polygon
        {
            return Polygon.Empty;
        }

        // Check if ring is closed
        var first = clippedExteriorRing[0];
        var last = clippedExteriorRing[^1];

        if (first.X != last.X || first.Y != last.Y)
        {
            clippedExteriorRing.Add(clippedExteriorRing[0]); // Close the ring
        }

        var exteriorLinearRing = new LinearRing(clippedExteriorRing.ToArray());

        // Process holes (interior rings)
        var interiorRings = new List<LinearRing>();
        for (int i = 0; i < polygon.NumInteriorRings; i++)
        {
            var interiorRing = polygon.GetInteriorRingN(i);
            var clippedInteriorRing = PolygonClip(interiorRing.CoordinateSequence, bbox);

            if (clippedInteriorRing.Count >= 4) // Need at least 4 points for a valid ring
            {
                // Check if ring is closed
                first = clippedInteriorRing[0];
                last = clippedInteriorRing[^1];

                if (first.X != last.X || first.Y != last.Y)
                {
                    clippedInteriorRing.Add(clippedInteriorRing[0]); // Close the ring
                }

                interiorRings.Add(new LinearRing(clippedInteriorRing.ToArray()));
            }
        }

        return new Polygon(exteriorLinearRing, interiorRings.ToArray());
    }

    public static MultiPolygon BBoxClipMultiPolygon(MultiPolygon multiPolygon, BBox bbox)
    {
        var polygons = new List<Polygon>();

        foreach (var geom in multiPolygon.Geometries)
        {
            var polygon = (Polygon)geom;
            var clipped = BBoxClipPolygon(polygon, bbox);

            if (!clipped.IsEmpty)
            {
                polygons.Add(clipped);
            }
        }

        return new MultiPolygon(polygons.ToArray());
    }

    private static List<Coordinate> PolygonClip(CoordinateSequence ring, BBox bbox)
    {
        var points = ring.ToCoordinateArray();
        List<Coordinate> result = new List<Coordinate>(points);

        // Clip against each edge of the bbox
        for (byte edge = 1; edge <= 8; edge *= 2)
        {
            if (result.Count == 0)
                break;

            var inputList = result;
            result = new List<Coordinate>();

            var prev = inputList[^1];
            var prevInside = (BitCode(prev, bbox) & edge) == 0;

            foreach (var point in inputList)
            {
                var inside = (BitCode(point, bbox) & edge) == 0;

                // If edge transitioned from outside to inside or vice versa, add intersection point
                if (inside != prevInside)
                {
                    var intersection = Intersect(prev, point, edge, bbox);
                    if (intersection != null)
                    {
                        result.Add(intersection);
                    }
                }

                // Add the current point if it's inside
                if (inside)
                {
                    result.Add(point);
                }

                prev = point;
                prevInside = inside;
            }
        }

        return result;
    }

    private static Coordinate? Intersect(Coordinate a, Coordinate b, byte edge, BBox bbox)
    {
        if ((edge & 8) != 0) // Top edge
        {
            return new Coordinate(a.X + (b.X - a.X) * (bbox.North - a.Y) / (b.Y - a.Y), bbox.North);
        }

        if ((edge & 4) != 0) // Bottom edge
        {
            return new Coordinate(a.X + (b.X - a.X) * (bbox.South - a.Y) / (b.Y - a.Y), bbox.South);
        }

        if ((edge & 2) != 0) // Right edge
        {
            return new Coordinate(bbox.East, a.Y + (b.Y - a.Y) * (bbox.East - a.X) / (b.X - a.X));
        }

        if ((edge & 1) != 0) // Left edge
        {
            return new Coordinate(bbox.West, a.Y + (b.Y - a.Y) * (bbox.West - a.X) / (b.X - a.X));
        }

        return null;
    }

    private static byte BitCode(Coordinate coordinate, BBox bbox)
    {
        var code = (byte)0;

        if (coordinate.X < bbox.West)
        {
            code |= 1;
        }
        else if (coordinate.X > bbox.East)
        {
            code |= 2;
        }

        if (coordinate.Y < bbox.South)
        {
            code |= 4;
        }
        else if (coordinate.Y > bbox.North)
        {
            code |= 8;
        }

        return code;
    }
}
