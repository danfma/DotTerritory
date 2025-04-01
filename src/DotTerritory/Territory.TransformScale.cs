// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.TransformScale.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Scales a geometry from a given point by the specified factor.
    /// </summary>
    /// <param name="geometry">The geometry to scale</param>
    /// <param name="factor">The scaling factor (1 = no change, &lt;1 = smaller, &gt;1 = larger)</param>
    /// <param name="options">Optional parameters including origin point</param>
    /// <returns>The scaled geometry</returns>
    public static Geometry TransformScale(
        Geometry geometry,
        double factor,
        TransformScaleOptions options = default
    )
    {
        options = TransformScaleOptions.OrDefault(options);

        // Get the origin point (center of scaling)
        Coordinate origin;
        if (options.Origin != null)
        {
            origin = options.Origin.Coordinate;
        }
        else
        {
            // Use the center of the geometry's bounding box if no origin is specified
            var bbox = Bbox(geometry);
            origin = new Coordinate((bbox.MinX + bbox.MaxX) / 2, (bbox.MinY + bbox.MaxY) / 2);
        }

        // Get original coordinates
        var coords = geometry.Coordinates;

        // Create new coordinates with scaling applied
        var scaledCoords = new Coordinate[coords.Length];
        for (int i = 0; i < coords.Length; i++)
        {
            var original = coords[i];

            // Calculate the vector from origin to point
            var dx = original.X - origin.X;
            var dy = original.Y - origin.Y;

            // Scale the vector
            var scaledX = dx * factor;
            var scaledY = dy * options.FactorY ?? factor;

            // Translate the scaled vector back from the origin
            scaledCoords[i] = new Coordinate(origin.X + scaledX, origin.Y + scaledY);

            // Handle Z coordinate if present and options.mutateZ is set
            if (options.MutateZ && !double.IsNaN(original.Z))
            {
                var dz = original.Z - (options.OriginZ ?? 0);
                var scaledZ = dz * (options.FactorZ ?? factor);
                scaledCoords[i].Z = (options.OriginZ ?? 0) + scaledZ;
            }
        }

        // Create a new geometry of the same type
        return RecreateGeometry(geometry, scaledCoords);
    }

    /// <summary>
    /// Scales a point from a given origin by the specified factor.
    /// </summary>
    /// <param name="point">The point to scale</param>
    /// <param name="factor">The scaling factor (1 = no change, &lt;1 = smaller, &gt;1 = larger)</param>
    /// <param name="options">Optional parameters including origin point</param>
    /// <returns>The scaled point</returns>
    public static Point TransformScale(
        Point point,
        double factor,
        TransformScaleOptions options = default
    )
    {
        return (Point)TransformScale((Geometry)point, factor, options);
    }

    /// <summary>
    /// Scales a line from a given origin by the specified factor.
    /// </summary>
    /// <param name="line">The line to scale</param>
    /// <param name="factor">The scaling factor (1 = no change, &lt;1 = smaller, &gt;1 = larger)</param>
    /// <param name="options">Optional parameters including origin point</param>
    /// <returns>The scaled line</returns>
    public static LineString TransformScale(
        LineString line,
        double factor,
        TransformScaleOptions options = default
    )
    {
        return (LineString)TransformScale((Geometry)line, factor, options);
    }

    /// <summary>
    /// Scales a polygon from a given origin by the specified factor.
    /// </summary>
    /// <param name="polygon">The polygon to scale</param>
    /// <param name="factor">The scaling factor (1 = no change, &lt;1 = smaller, &gt;1 = larger)</param>
    /// <param name="options">Optional parameters including origin point</param>
    /// <returns>The scaled polygon</returns>
    public static Polygon TransformScale(
        Polygon polygon,
        double factor,
        TransformScaleOptions options = default
    )
    {
        return (Polygon)TransformScale((Geometry)polygon, factor, options);
    }
}

/// <summary>
/// Options for transform scale operations
/// </summary>
public readonly record struct TransformScaleOptions
{
    public static readonly TransformScaleOptions Empty = new();
    public static readonly TransformScaleOptions Default = new()
    {
        Origin = null,
        FactorY = null,
        FactorZ = null,
        OriginZ = null,
        MutateZ = false,
    };

    /// <summary>
    /// Point from which to scale the geometry (defaults to the center of the geometry's bounding box)
    /// </summary>
    public Point? Origin { get; init; }

    /// <summary>
    /// Scaling factor for y-coordinates, defaults to the same as the main factor if not specified
    /// </summary>
    public double? FactorY { get; init; }

    /// <summary>
    /// Scaling factor for z-coordinates, defaults to the same as the main factor if not specified
    /// </summary>
    public double? FactorZ { get; init; }

    /// <summary>
    /// Z-coordinate of the origin, defaults to 0
    /// </summary>
    public double? OriginZ { get; init; }

    /// <summary>
    /// If true, z-values will be scaled
    /// </summary>
    public bool MutateZ { get; init; }

    public static TransformScaleOptions OrDefault(TransformScaleOptions options) =>
        options == Empty ? Default : options;
}
