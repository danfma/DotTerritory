namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a line, a specified start distance, and a specified stop distance and returns a subsection of the line
    /// between those points. The distances are measured in the same units as the coordinate system.
    /// </summary>
    /// <param name="line">Input LineString geometry</param>
    /// <param name="startDistance">Distance along the line to the starting point</param>
    /// <param name="stopDistance">Distance along the line to the ending point</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>Sliced LineString</returns>
    /// <example>
    /// <code>
    /// var line = GeometryFactory.CreateLineString(new[] {
    ///     new Coordinate(-77.031669, 38.878605),
    ///     new Coordinate(-77.029609, 38.881946),
    ///     new Coordinate(-77.020339, 38.884084),
    ///     new Coordinate(-77.025661, 38.885821),
    ///     new Coordinate(-77.021884, 38.889563)
    /// });
    /// var sliced = Territory.LineSliceAlong(line, Length.FromKilometers(1), Length.FromKilometers(2.5));
    /// </code>
    /// </example>
    public static LineString LineSliceAlong(
        LineString line,
        Length startDistance,
        Length stopDistance,
        Func<LineSliceAlongOptions, LineSliceAlongOptions>? configure = null
    )
    {
        var options = configure?.Invoke(LineSliceAlongOptions.Empty) ?? LineSliceAlongOptions.Empty;
        var lineLength = GetLength(line);

        // Normalize start and stop distances to be within the line's length
        if (startDistance > lineLength)
            startDistance = lineLength;

        if (stopDistance > lineLength)
            stopDistance = lineLength;

        if (startDistance < Length.Zero)
            startDistance = Length.Zero;

        if (stopDistance < Length.Zero)
            stopDistance = Length.Zero;

        // Swap if start is greater than stop
        if (startDistance > stopDistance)
            (startDistance, stopDistance) = (stopDistance, startDistance);

        // If start and stop are the same, return a point as a LineString with identical points
        if (startDistance.Equals(stopDistance, Length.FromMicrometers(1)))
        {
            var point = WalkAlong(line, startDistance);
            return new LineString(new[] { point.Coordinate, point.Coordinate });
        }

        // Get coordinates for start and stop points
        var startPoint = WalkAlong(line, startDistance).Coordinate;
        var stopPoint = WalkAlong(line, stopDistance).Coordinate;

        // Special case: if we're taking the first segment and starting exactly at the beginning
        if (Math.Abs(startDistance.Meters) < 1e-10 && line.Coordinates.Length > 0)
        {
            startPoint = line.Coordinates[0];
        }

        // Special case: if we're taking the last segment and stopping exactly at the end
        if (
            Math.Abs(stopDistance.Meters - lineLength.Meters) < 1e-10
            && line.Coordinates.Length > 0
        )
        {
            stopPoint = line.Coordinates[line.Coordinates.Length - 1];
        }

        // Generate coordinates for the sliced line
        var coords = new List<Coordinate> { startPoint };

        // Accumulate length as we move along the line
        var accumulatedLength = Length.Zero;

        // For each segment in the original line
        for (int i = 0; i < line.Coordinates.Length - 1; i++)
        {
            var currentCoord = line.Coordinates[i];
            var nextCoord = line.Coordinates[i + 1];

            // Calculate the length of this segment
            var segmentLength = Distance(currentCoord, nextCoord);

            // If accumulated length + segment length is less than start distance, skip this segment
            if (accumulatedLength + segmentLength < startDistance)
            {
                accumulatedLength += segmentLength;
                continue;
            }

            // If accumulated length is greater than stop distance, we're done
            if (accumulatedLength > stopDistance)
                break;

            // If this segment contains the stop point, add it
            if (
                accumulatedLength <= stopDistance
                && accumulatedLength + segmentLength >= stopDistance
            )
            {
                coords.Add(stopPoint);
                break;
            }

            // If this segment is entirely within our slice, add the endpoint of the segment
            if (
                accumulatedLength >= startDistance
                && accumulatedLength + segmentLength <= stopDistance
            )
            {
                coords.Add(nextCoord);
            }

            accumulatedLength += segmentLength;
        }

        // If we didn't add the stop point yet, add it now
        if (coords.Count == 1 || coords[coords.Count - 1].Distance(stopPoint) > 1e-10)
        {
            coords.Add(stopPoint);
        }

        // Create and return the new LineString
        return new LineString(coords.ToArray());
    }

    /// <summary>
    /// LineSliceAlong options
    /// </summary>
    /// <param name="Units">Length units for the input distances</param>
    public record struct LineSliceAlongOptions(LengthUnit Units = LengthUnit.Meter)
    {
        public static readonly LineSliceAlongOptions Empty = new();
    }
}
