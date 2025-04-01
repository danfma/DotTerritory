namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes any GeoJSON object and returns a rectangular Polygon that encompasses all vertices of the given object.
    /// For Point and MultiPoint features, the envelope is created using the single point or the collection of points.
    /// For LineString and MultiLineString features, the envelope is created using the vertices of the lines.
    /// For Polygon and MultiPolygon features, the envelope is created using the vertices of the polygons.
    /// </summary>
    /// <param name="bbox">A BBox object representing the bounding box</param>
    /// <returns>A rectangular Polygon that encompasses all vertices of the given object</returns>
    /// <example>
    /// <code>
    /// var bbox = new BBox(-75.343, 39.984, -75.534, 39.123);
    /// var polygon = Territory.Envelope(bbox);
    /// </code>
    /// </example>
    public static Polygon Envelope(BBox bbox)
    {
        var west = bbox.West;
        var south = bbox.South;
        var east = bbox.East;
        var north = bbox.North;

        var geometryFactory = new GeometryFactory();

        // Create the corners of the rectangle (5 points to close the ring)
        var coordinates = new[]
        {
            new Coordinate(west, north),
            new Coordinate(east, north),
            new Coordinate(east, south),
            new Coordinate(west, south),
            new Coordinate(west, north), // Close the ring
        };

        // Create the linear ring for the polygon shell
        var shell = geometryFactory.CreateLinearRing(coordinates);

        // Create the polygon
        return geometryFactory.CreatePolygon(shell);
    }

    /// <summary>
    /// Takes any GeoJSON object and returns a rectangular Polygon that encompasses all vertices of the given object.
    /// </summary>
    /// <param name="west">The westernmost longitude</param>
    /// <param name="south">The southernmost latitude</param>
    /// <param name="east">The easternmost longitude</param>
    /// <param name="north">The northernmost latitude</param>
    /// <returns>A rectangular Polygon that encompasses all vertices of the given object</returns>
    public static Polygon Envelope(double west, double south, double east, double north)
    {
        return Envelope(new BBox(west, south, east, north));
    }

    /// <summary>
    /// Takes any GeoJSON object and returns a rectangular Polygon that encompasses all vertices of the given object.
    /// </summary>
    /// <param name="geometry">A Geometry object</param>
    /// <returns>A rectangular Polygon that encompasses all vertices of the given object</returns>
    public static Polygon Envelope(Geometry geometry)
    {
        var envelope = geometry.EnvelopeInternal;
        return Envelope(new BBox(envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY));
    }
}
