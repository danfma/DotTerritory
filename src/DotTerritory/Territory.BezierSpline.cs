// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.BezierSpline.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a line and returns a curved version by applying a Bezier spline algorithm.
    /// The bezier spline implementation is based on @lecho/leaflet-path-drag and
    /// @mourner/simplify-js.
    /// </summary>
    /// <param name="line">Input LineString</param>
    /// <param name="options">Optional parameters: resolution (default: 10000), sharpness (default: 0.85)</param>
    /// <returns>Curved line as LineString</returns>
    public static LineString BezierSpline(LineString line, BezierSplineOptions options = default)
    {
        options = BezierSplineOptions.OrDefault(options);

        var coords = line.Coordinates;

        // Not enough points to create a curve
        if (coords.Length < 3)
        {
            return line;
        }

        var splinePoints = new List<Coordinate>();
        var points = coords.ToList();

        // Clone the original points array
        var controlPoints = points.Select(p => new Coordinate(p.X, p.Y)).ToList();

        // Add duplicate start and end points to the control points list
        controlPoints.Insert(0, controlPoints[0]);
        controlPoints.Add(controlPoints[^1]);

        // Create the spline points
        for (var i = 0; i < points.Count - 1; i++)
        {
            var p0 = controlPoints[i];
            var p1 = controlPoints[i + 1];
            var p2 = controlPoints[i + 2];
            var p3 = controlPoints[i + 3];

            // Create segments based on resolution
            var maxSegments = Math.Max(
                Math.Ceiling(Distance(p1, p2).Meters / options.Resolution),
                1
            );

            for (var j = 0; j <= maxSegments; j++)
            {
                var t = (double)j / maxSegments;

                // Calculate the position of the spline point using the cubic Bezier formula
                var position = GetCubicBezierXY(t, p0, p1, p2, p3, options.Sharpness);

                // Only add a new point if it's not a duplicate of the last point
                if (splinePoints.Count == 0 || !splinePoints[^1].Equals(position))
                {
                    splinePoints.Add(position);
                }
            }
        }

        // Ensure the end point is added
        if (!splinePoints[^1].Equals(points[^1]))
        {
            splinePoints.Add(points[^1]);
        }

        return new LineString(splinePoints.ToArray());
    }

    /// <summary>
    /// Calculates cubic Bezier curve point at a specific t value
    /// </summary>
    private static Coordinate GetCubicBezierXY(
        double t,
        Coordinate p0,
        Coordinate p1,
        Coordinate p2,
        Coordinate p3,
        double sharpness
    )
    {
        // Adjust control points based on sharpness
        var controlPoint1 = new Coordinate(
            p1.X + (p2.X - p0.X) / 6 * sharpness,
            p1.Y + (p2.Y - p0.Y) / 6 * sharpness
        );

        var controlPoint2 = new Coordinate(
            p2.X - (p3.X - p1.X) / 6 * sharpness,
            p2.Y - (p3.Y - p1.Y) / 6 * sharpness
        );

        // Apply cubic Bezier formula
        var x = CubicBezier(t, p1.X, controlPoint1.X, controlPoint2.X, p2.X);
        var y = CubicBezier(t, p1.Y, controlPoint1.Y, controlPoint2.Y, p2.Y);

        return new Coordinate(x, y);
    }

    /// <summary>
    /// Calculate cubic Bezier value at a specific t
    /// </summary>
    private static double CubicBezier(double t, double p0, double p1, double p2, double p3)
    {
        var oneMinusT = 1 - t;
        var oneMinusTSquared = oneMinusT * oneMinusT;
        var oneMinusTCubed = oneMinusTSquared * oneMinusT;
        var tSquared = t * t;
        var tCubed = tSquared * t;

        return oneMinusTCubed * p0
            + 3 * oneMinusTSquared * t * p1
            + 3 * oneMinusT * tSquared * p2
            + tCubed * p3;
    }
}

/// <summary>
/// Options for bezier spline generation
/// </summary>
public readonly record struct BezierSplineOptions
{
    public static readonly BezierSplineOptions Empty = new();
    public static readonly BezierSplineOptions Default = new()
    {
        Resolution = 10_000,
        Sharpness = 0.85,
    };

    /// <summary>
    /// Time in milliseconds between points; determines the smoothness of the curve (default: 10000)
    /// </summary>
    public double Resolution { get; init; }

    /// <summary>
    /// Bezier curve sharpness factor between 0 and 1 (default: 0.85)
    /// </summary>
    public double Sharpness { get; init; }

    public static BezierSplineOptions OrDefault(BezierSplineOptions options) =>
        options == Empty ? Default : options;
}
