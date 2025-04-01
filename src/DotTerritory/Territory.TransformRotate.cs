// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.TransformRotate.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Rotates a geometry around a center point by the given angle.
    /// </summary>
    /// <param name="geometry">The geometry to rotate</param>
    /// <param name="angle">The angle of rotation in degrees (positive is clockwise)</param>
    /// <param name="options">Optional parameters including pivot point</param>
    /// <returns>The rotated geometry</returns>
    public static Geometry TransformRotate(
        Geometry geometry,
        Angle angle,
        TransformRotateOptions options = default
    )
    {
        options = TransformRotateOptions.OrDefault(options);

        // Get the pivot point (center of rotation)
        Coordinate pivot;
        if (options.Pivot != null)
        {
            pivot = options.Pivot.Coordinate;
        }
        else
        {
            // Use the center of the geometry's bounding box if no pivot is specified
            var bbox = Bbox(geometry);
            pivot = new Coordinate((bbox.MinX + bbox.MaxX) / 2, (bbox.MinY + bbox.MaxY) / 2);
        }

        // Convert angle to radians (negative for clockwise rotation in mathematical terms)
        var angleInRadians = -angle.Radians;

        // Get original coordinates
        var coords = geometry.Coordinates;

        // Create new coordinates with rotation applied
        var rotatedCoords = new Coordinate[coords.Length];
        for (int i = 0; i < coords.Length; i++)
        {
            var original = coords[i];

            // Translate point to origin
            var x = original.X - pivot.X;
            var y = original.Y - pivot.Y;

            // Rotate point
            var xRotated = x * Math.Cos(angleInRadians) - y * Math.Sin(angleInRadians);
            var yRotated = x * Math.Sin(angleInRadians) + y * Math.Cos(angleInRadians);

            // Translate point back
            rotatedCoords[i] = new Coordinate(xRotated + pivot.X, yRotated + pivot.Y);

            // Handle Z coordinate if present and options.mutateZ is set
            if (options.MutateZ && !double.IsNaN(original.Z))
            {
                rotatedCoords[i].Z = original.Z;
            }
        }

        // Create a new geometry of the same type
        return RecreateGeometry(geometry, rotatedCoords);
    }

    /// <summary>
    /// Rotates a point around a center point by the given angle.
    /// </summary>
    /// <param name="point">The point to rotate</param>
    /// <param name="angle">The angle of rotation in degrees (positive is clockwise)</param>
    /// <param name="options">Optional parameters including pivot point</param>
    /// <returns>The rotated point</returns>
    public static Point TransformRotate(
        Point point,
        Angle angle,
        TransformRotateOptions options = default
    )
    {
        return (Point)TransformRotate((Geometry)point, angle, options);
    }

    /// <summary>
    /// Rotates a line around a center point by the given angle.
    /// </summary>
    /// <param name="line">The line to rotate</param>
    /// <param name="angle">The angle of rotation in degrees (positive is clockwise)</param>
    /// <param name="options">Optional parameters including pivot point</param>
    /// <returns>The rotated line</returns>
    public static LineString TransformRotate(
        LineString line,
        Angle angle,
        TransformRotateOptions options = default
    )
    {
        return (LineString)TransformRotate((Geometry)line, angle, options);
    }

    /// <summary>
    /// Rotates a polygon around a center point by the given angle.
    /// </summary>
    /// <param name="polygon">The polygon to rotate</param>
    /// <param name="angle">The angle of rotation in degrees (positive is clockwise)</param>
    /// <param name="options">Optional parameters including pivot point</param>
    /// <returns>The rotated polygon</returns>
    public static Polygon TransformRotate(
        Polygon polygon,
        Angle angle,
        TransformRotateOptions options = default
    )
    {
        return (Polygon)TransformRotate((Geometry)polygon, angle, options);
    }
}

/// <summary>
/// Options for transform rotate operations
/// </summary>
public readonly record struct TransformRotateOptions
{
    public static readonly TransformRotateOptions Empty = new();
    public static readonly TransformRotateOptions Default = new() { Pivot = null, MutateZ = false };

    /// <summary>
    /// Point around which to rotate the geometry (defaults to the center of the geometry's bounding box)
    /// </summary>
    public Point? Pivot { get; init; }

    /// <summary>
    /// If true, z-values will be maintained during rotation
    /// </summary>
    public bool MutateZ { get; init; }

    public static TransformRotateOptions OrDefault(TransformRotateOptions options) =>
        options == Empty ? Default : options;
}
