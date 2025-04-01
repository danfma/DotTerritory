// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.LineChunk.cs
namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Divides a LineString into chunks of a specified length.
    /// If the line is shorter than the segment length, the original line is returned.
    /// </summary>
    /// <param name="line">The LineString to divide</param>
    /// <param name="segmentLength">The length of each segment</param>
    /// <param name="options">Optional parameters</param>
    /// <returns>Array of LineString segments</returns>
    public static LineString[] LineChunk(
        LineString line,
        Length segmentLength,
        LineChunkOptions options = default
    )
    {
        if (options == LineChunkOptions.Empty)
            options = LineChunkOptions.Default;

        // If the segmentLength is less than or equal to 0, return the original line
        if (segmentLength.Meters <= 0)
        {
            throw new ArgumentException(
                "Segment length must be greater than 0",
                nameof(segmentLength)
            );
        }

        // Calculate the total length of the line
        var lineLength = MeasureLineLength(line);

        // If the line is shorter than the segment length, return the original line
        if (lineLength.Meters <= segmentLength.Meters)
        {
            return new LineString[] { line };
        }

        var chunks = new List<LineString>();
        var currentChunkCoords = new List<Coordinate>();
        var accumulatedLength = Length.FromMeters(0);
        var coords = line.Coordinates;

        // Add the first coordinate to the current chunk
        currentChunkCoords.Add(coords[0]);

        // Iterate through each segment in the line
        for (var i = 0; i < coords.Length - 1; i++)
        {
            var segmentStart = coords[i];
            var segmentEnd = coords[i + 1];
            var segmentDistance = Distance(segmentStart, segmentEnd);

            // If adding this segment would exceed the segment length, split it
            if (accumulatedLength + segmentDistance > segmentLength)
            {
                var remainingLength = segmentLength - accumulatedLength;
                var ratio = remainingLength.Meters / segmentDistance.Meters;

                // Calculate the intermediate coordinate using linear interpolation
                var intermediateX = segmentStart.X + ratio * (segmentEnd.X - segmentStart.X);
                var intermediateY = segmentStart.Y + ratio * (segmentEnd.Y - segmentStart.Y);
                var intermediateCoord = new Coordinate(intermediateX, intermediateY);

                // Add the intermediate coordinate to the current chunk
                currentChunkCoords.Add(intermediateCoord);

                // Create a LineString for the current chunk and add it to the list
                chunks.Add(new LineString(currentChunkCoords.ToArray()));

                // Start a new chunk with the intermediate coordinate
                currentChunkCoords = new List<Coordinate> { intermediateCoord };
                accumulatedLength = Length.FromMeters(0);

                // Reconsider the current segment with a new segmentStart
                i--; // Step back one iteration
                coords[i] = intermediateCoord; // Update the segment start

                continue;
            }

            // Add the end coordinate to the current chunk
            currentChunkCoords.Add(segmentEnd);
            accumulatedLength += segmentDistance;

            // If we've accumulated exactly the segment length, complete the chunk
            if (Math.Abs(accumulatedLength.Meters - segmentLength.Meters) < 1e-10)
            {
                chunks.Add(new LineString(currentChunkCoords.ToArray()));

                // Start a new chunk with the last coordinate of the previous chunk
                currentChunkCoords = new List<Coordinate> { segmentEnd };
                accumulatedLength = Length.FromMeters(0);
            }
        }

        // Add any remaining coordinates as the final chunk
        if (currentChunkCoords.Count >= 2)
        {
            chunks.Add(new LineString(currentChunkCoords.ToArray()));
        }

        return chunks.ToArray();
    }

    /// <summary>
    /// Helper method to measure the length of a line to avoid ambiguity with the Length property/method
    /// </summary>
    private static Length MeasureLineLength(LineString line)
    {
        double totalLengthMeters = 0;
        var coords = line.Coordinates;

        for (int i = 0; i < coords.Length - 1; i++)
        {
            totalLengthMeters += Distance(coords[i], coords[i + 1]).Meters;
        }

        return Length.FromMeters(totalLengthMeters);
    }
}

/// <summary>
/// Options for LineChunk
/// </summary>
public readonly record struct LineChunkOptions
{
    public static readonly LineChunkOptions Empty = new();
    public static readonly LineChunkOptions Default = new();

    // Currently no options used, but keeping the record struct for future extensions
    // and to maintain consistency with other Territory functions
}
