// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.LineOffset.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a line and returns a line at offset distance from the input line.
    /// Positive values offset to the right, negative to the left along the line.
    /// </summary>
    /// <param name="line">Input line</param>
    /// <param name="distance">Distance to offset the line</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>Offset line</returns>
    public static LineString LineOffset(
        LineString line,
        Length distance,
        LineOffsetOptions options = default
    )
    {
        if (options == LineOffsetOptions.Empty)
            options = LineOffsetOptions.Default;

        // Convert to NTS geometry classes for offsetting
        var lineGeom = line;

        // Apply offset using NetTopologySuite buffer and extract the result
        var offsetCurve = lineGeom.Offset(distance.Meters, options.Segments);

        // Return the offset line (could be a LineString or MultiLineString)
        if (offsetCurve is LineString lineString)
        {
            return lineString;
        }
        else if (offsetCurve is MultiLineString multiLineString)
        {
            // Combine all strings from the multilinestring result
            var coordinates = new List<Coordinate>();

            for (var i = 0; i < multiLineString.NumGeometries; i++)
            {
                var geom = multiLineString.GetGeometryN(i);
                if (geom is LineString ls)
                {
                    coordinates.AddRange(ls.Coordinates);
                }
            }

            return new LineString(coordinates.ToArray());
        }

        throw new InvalidOperationException(
            "Unexpected geometry type returned from offset operation"
        );
    }
}

/// <summary>
/// Options for LineOffset operations
/// </summary>
public readonly record struct LineOffsetOptions
{
    public static readonly LineOffsetOptions Empty = new();
    public static readonly LineOffsetOptions Default = new() { Segments = 8 };

    /// <summary>
    /// Number of segments used to approximate a quarter circle (default: 8)
    /// </summary>
    public int Segments { get; init; }
}
