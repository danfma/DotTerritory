// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.TransformTranslate.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Translates (moves) a geometry by the given distance along the given angle.
    /// </summary>
    /// <param name="geometry">The geometry to translate</param>
    /// <param name="distance">The distance to translate the geometry</param>
    /// <param name="angle">The angle of translation in degrees (clockwise from north)</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>The translated geometry</returns>
    public static Geometry TransformTranslate(
        Geometry geometry,
        Length distance,
        Angle angle,
        TransformTranslateOptions options = default
    )
    {
        options = TransformTranslateOptions.OrDefault(options);

        // Calculate translation offsets
        var angleInRadians = angle.Radians;
        var dx = Math.Sin(angleInRadians) * distance.Meters;
        var dy = Math.Cos(angleInRadians) * distance.Meters;

        // Get original coordinates
        var coords = geometry.Coordinates;

        // Create new coordinates with the calculated offset
        var translatedCoords = new Coordinate[coords.Length];
        for (int i = 0; i < coords.Length; i++)
        {
            var original = coords[i];
            translatedCoords[i] = new Coordinate(original.X + dx, original.Y + dy);

            // Handle Z coordinate if present and options.mutateZ is set
            if (options.MutateZ && !double.IsNaN(original.Z))
            {
                translatedCoords[i].Z = original.Z;
            }
        }

        // Create a new geometry of the same type
        return RecreateGeometry(geometry, translatedCoords);
    }

    /// <summary>
    /// Translates (moves) a point by the given distance along the given angle.
    /// </summary>
    /// <param name="point">The point to translate</param>
    /// <param name="distance">The distance to translate the point</param>
    /// <param name="angle">The angle of translation in degrees (clockwise from north)</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>The translated point</returns>
    public static Point TransformTranslate(
        Point point,
        Length distance,
        Angle angle,
        TransformTranslateOptions options = default
    )
    {
        return (Point)TransformTranslate((Geometry)point, distance, angle, options);
    }

    /// <summary>
    /// Translates (moves) a line by the given distance along the given angle.
    /// </summary>
    /// <param name="line">The line to translate</param>
    /// <param name="distance">The distance to translate the line</param>
    /// <param name="angle">The angle of translation in degrees (clockwise from north)</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>The translated line</returns>
    public static LineString TransformTranslate(
        LineString line,
        Length distance,
        Angle angle,
        TransformTranslateOptions options = default
    )
    {
        return (LineString)TransformTranslate((Geometry)line, distance, angle, options);
    }

    /// <summary>
    /// Translates (moves) a polygon by the given distance along the given angle.
    /// </summary>
    /// <param name="polygon">The polygon to translate</param>
    /// <param name="distance">The distance to translate the polygon</param>
    /// <param name="angle">The angle of translation in degrees (clockwise from north)</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>The translated polygon</returns>
    public static Polygon TransformTranslate(
        Polygon polygon,
        Length distance,
        Angle angle,
        TransformTranslateOptions options = default
    )
    {
        return (Polygon)TransformTranslate((Geometry)polygon, distance, angle, options);
    }

    // Helper method to recreate a geometry of the same type with new coordinates
    private static Geometry RecreateGeometry(Geometry originalGeometry, Coordinate[] newCoordinates)
    {
        if (originalGeometry is Point)
        {
            return new Point(newCoordinates[0]);
        }
        else if (originalGeometry is LineString)
        {
            return new LineString(newCoordinates);
        }
        else if (originalGeometry is Polygon polygon)
        {
            // For polygons, we need to handle the exterior and interior rings
            int numInteriorRings = polygon.NumInteriorRings;

            // Get the coordinates for the exterior ring
            var shellCoordCount = polygon.ExteriorRing.NumPoints;
            var shellCoordinates = new Coordinate[shellCoordCount];

            for (int i = 0; i < shellCoordCount; i++)
            {
                int index = i;
                shellCoordinates[i] = newCoordinates[index];
            }

            // Create a new shell
            var shell = new LinearRing(shellCoordinates);

            // If there are no interior rings, create a simple polygon
            if (numInteriorRings == 0)
            {
                return new Polygon(shell);
            }

            // Otherwise, handle interior rings
            var holes = new LinearRing[numInteriorRings];
            int coordIndex = shellCoordCount;

            for (int ringIndex = 0; ringIndex < numInteriorRings; ringIndex++)
            {
                var ring = polygon.GetInteriorRingN(ringIndex);
                var ringCoordCount = ring.NumPoints;
                var ringCoordinates = new Coordinate[ringCoordCount];

                for (int i = 0; i < ringCoordCount; i++)
                {
                    ringCoordinates[i] = newCoordinates[coordIndex++];
                }

                holes[ringIndex] = new LinearRing(ringCoordinates);
            }

            return new Polygon(shell, holes);
        }
        else if (originalGeometry is MultiPoint)
        {
            var points = new Point[newCoordinates.Length];
            for (int i = 0; i < newCoordinates.Length; i++)
            {
                points[i] = new Point(newCoordinates[i]);
            }
            return new MultiPoint(points);
        }
        // Add other geometry types as needed

        throw new ArgumentException(
            $"Unsupported geometry type: {originalGeometry.GetType().Name}"
        );
    }
}

/// <summary>
/// Options for transform translate operations
/// </summary>
public readonly record struct TransformTranslateOptions
{
    public static readonly TransformTranslateOptions Empty = new();
    public static readonly TransformTranslateOptions Default = new() { MutateZ = false };

    /// <summary>
    /// If true, z-values will be translated along with x and y
    /// </summary>
    public bool MutateZ { get; init; }

    public static TransformTranslateOptions OrDefault(TransformTranslateOptions options) =>
        options == Empty ? Default : options;
}
