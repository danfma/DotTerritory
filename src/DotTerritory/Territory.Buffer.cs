// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.Buffer.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Creates a buffer around a geometry for a given radius.
    /// Units of the radius are determined by the units parameter.
    /// </summary>
    /// <param name="geometry">The input geometry</param>
    /// <param name="radius">The buffer radius</param>
    /// <param name="options">Optional parameters: steps, units</param>
    /// <returns>Buffered polygon or multi-polygon</returns>
    public static Geometry Buffer(Geometry geometry, Length radius, BufferOptions options = default)
    {
        if (options == BufferOptions.Empty)
            options = BufferOptions.Default;

        // Convert length to meters (default unit for NetTopologySuite)
        var distanceInMeters = radius.Meters;

        // Use NetTopologySuite's buffer operation
        var buffered = geometry.Buffer(distanceInMeters, options.Steps);

        return buffered;
    }

    /// <summary>
    /// Creates a buffer around a point for a given radius.
    /// Units of the radius are determined by the units parameter.
    /// </summary>
    /// <param name="point">The input point</param>
    /// <param name="radius">The buffer radius</param>
    /// <param name="options">Optional parameters: steps, units</param>
    /// <returns>Buffered polygon</returns>
    public static Polygon Buffer(Point point, Length radius, BufferOptions options = default)
    {
        return (Polygon)Buffer((Geometry)point, radius, options);
    }

    /// <summary>
    /// Creates a buffer around a line for a given radius.
    /// Units of the radius are determined by the units parameter.
    /// </summary>
    /// <param name="line">The input line</param>
    /// <param name="radius">The buffer radius</param>
    /// <param name="options">Optional parameters: steps, units</param>
    /// <returns>Buffered polygon</returns>
    public static Geometry Buffer(LineString line, Length radius, BufferOptions options = default)
    {
        return Buffer((Geometry)line, radius, options);
    }

    /// <summary>
    /// Creates a buffer around a polygon for a given radius.
    /// Units of the radius are determined by the units parameter.
    /// </summary>
    /// <param name="polygon">The input polygon</param>
    /// <param name="radius">The buffer radius</param>
    /// <param name="options">Optional parameters: steps, units</param>
    /// <returns>Buffered polygon</returns>
    public static Geometry Buffer(Polygon polygon, Length radius, BufferOptions options = default)
    {
        return Buffer((Geometry)polygon, radius, options);
    }
}

/// <summary>
/// Options for buffer generation
/// </summary>
public readonly record struct BufferOptions
{
    public static readonly BufferOptions Empty = new();
    public static readonly BufferOptions Default = new() { Steps = 8 };

    /// <summary>
    /// Number of segments used to approximate a quarter circle (default: 8)
    /// </summary>
    public int Steps { get; init; }
}
