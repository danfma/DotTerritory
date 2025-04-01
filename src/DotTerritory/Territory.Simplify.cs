using NetTopologySuite.Geometries;
using NetTopologySuite.Simplify;
using UnitsNet;
using UnitsNet.Units;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Simplifies a feature's coordinates using the Douglas-Peucker algorithm.
    /// Takes a LineString or Polygon and returns a simplified version.
    /// </summary>
    /// <param name="geom">Feature to be simplified</param>
    /// <param name="tolerance">Simplification tolerance - higher means more simplification</param>
    /// <param name="highQuality">Whether or not to use high quality simplification</param>
    /// <returns>A simplified feature</returns>
    /// <example>
    /// <code>
    /// var line = geometryFactory.CreateLineString(new[] {
    ///     new Coordinate(0, 0),
    ///     new Coordinate(0.01, 0.01),
    ///     new Coordinate(0.02, 0.01),
    ///     new Coordinate(0.03, 0),
    ///     new Coordinate(1, 0)
    /// });
    /// var simplified = Territory.Simplify(line, 0.05);
    /// // simplified now has only two points: (0,0) and (1,0)
    /// </code>
    /// </example>
    public static Geometry Simplify(Geometry geom, double tolerance, bool highQuality = false)
    {
        if (geom == null || geom.IsEmpty)
            return geom;

        if (tolerance <= 0)
            return geom.Copy();

        // Use NTS's DouglasPeuckerSimplifier for high quality simplification
        if (highQuality)
        {
            var simplifier = new DouglasPeuckerSimplifier(geom) { DistanceTolerance = tolerance };
            return simplifier.GetResultGeometry();
        }

        // Use NTS's TopologyPreservingSimplifier for normal simplification
        var tpSimplifier = new TopologyPreservingSimplifier(geom) { DistanceTolerance = tolerance };
        return tpSimplifier.GetResultGeometry();
    }

    /// <summary>
    /// Simplifies a feature's coordinates using the Douglas-Peucker algorithm.
    /// Takes a LineString or Polygon and returns a simplified version.
    /// </summary>
    /// <param name="geom">Feature to be simplified</param>
    /// <param name="tolerance">Simplification tolerance - higher means more simplification</param>
    /// <param name="options">Additional options</param>
    /// <returns>A simplified feature</returns>
    public static Geometry Simplify(
        Geometry geom,
        double tolerance,
        Func<SimplifyOptions, SimplifyOptions>? configure = null
    )
    {
        var options = configure?.Invoke(SimplifyOptions.Empty) ?? SimplifyOptions.Empty;
        return Simplify(geom, tolerance, options.HighQuality);
    }

    /// <summary>
    /// Options for simplification
    /// </summary>
    /// <param name="HighQuality">Whether to use high quality simplification (slower but better)</param>
    public record struct SimplifyOptions(bool HighQuality = false)
    {
        public static readonly SimplifyOptions Empty = new();
    }
}
