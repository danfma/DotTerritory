namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Returns true if a point is on a line. Accepts an optional parameter to ignore the start or end vertices of the line.
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <param name="line">LineString to check against</param>
    /// <param name="ignoreEndVertices">Whether to ignore the line's endpoints (false by default)</param>
    /// <param name="epsilon">Maximum tolerance distance for a point to be considered on a line segment</param>
    /// <returns>True if the point is on the line</returns>
    public static bool BooleanPointOnLine(
        Point point,
        LineString line,
        bool ignoreEndVertices = false,
        double? epsilon = null
    )
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point), "Point is required");

        if (line == null)
            throw new ArgumentNullException(nameof(line), "LineString is required");

        var pointCoord = point.Coordinate;
        var lineCoords = line.Coordinates;

        for (int i = 0; i < lineCoords.Length - 1; i++)
        {
            string? ignoreBoundary = null;

            if (ignoreEndVertices)
            {
                // Determine which ends to ignore based on the segment's position in the line
                if (i == 0)
                {
                    ignoreBoundary = "start";
                }

                if (i == lineCoords.Length - 2)
                {
                    ignoreBoundary = "end";
                }

                if (i == 0 && i + 1 == lineCoords.Length - 2)
                {
                    ignoreBoundary = "both";
                }
            }

            if (
                IsPointOnLineSegment(
                    lineCoords[i],
                    lineCoords[i + 1],
                    pointCoord,
                    ignoreBoundary,
                    epsilon
                )
            )
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns true if a point is on a line. Accepts an optional parameter to ignore the start or end vertices of the line.
    /// </summary>
    /// <param name="pointFeature">Point feature to check</param>
    /// <param name="line">LineString to check against</param>
    /// <param name="ignoreEndVertices">Whether to ignore the line's endpoints (false by default)</param>
    /// <param name="epsilon">Maximum tolerance distance for a point to be considered on a line segment</param>
    /// <returns>True if the point is on the line</returns>
    public static bool BooleanPointOnLine(
        IFeature pointFeature,
        LineString line,
        bool ignoreEndVertices = false,
        double? epsilon = null
    )
    {
        if (pointFeature == null)
            throw new ArgumentNullException(nameof(pointFeature), "Point feature is required");

        if (!(pointFeature.Geometry is Point point))
            throw new ArgumentException("Feature geometry must be a Point", nameof(pointFeature));

        return BooleanPointOnLine(point, line, ignoreEndVertices, epsilon);
    }

    /// <summary>
    /// Returns true if a point is on a line. Accepts an optional parameter to ignore the start or end vertices of the line.
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <param name="lineFeature">LineString feature to check against</param>
    /// <param name="ignoreEndVertices">Whether to ignore the line's endpoints (false by default)</param>
    /// <param name="epsilon">Maximum tolerance distance for a point to be considered on a line segment</param>
    /// <returns>True if the point is on the line</returns>
    public static bool BooleanPointOnLine(
        Point point,
        IFeature lineFeature,
        bool ignoreEndVertices = false,
        double? epsilon = null
    )
    {
        if (lineFeature == null)
            throw new ArgumentNullException(nameof(lineFeature), "LineString feature is required");

        if (!(lineFeature.Geometry is LineString line))
            throw new ArgumentException(
                "Feature geometry must be a LineString",
                nameof(lineFeature)
            );

        return BooleanPointOnLine(point, line, ignoreEndVertices, epsilon);
    }

    /// <summary>
    /// Returns true if a point is on a line. Accepts an optional parameter to ignore the start or end vertices of the line.
    /// </summary>
    /// <param name="pointFeature">Point feature to check</param>
    /// <param name="lineFeature">LineString feature to check against</param>
    /// <param name="ignoreEndVertices">Whether to ignore the line's endpoints (false by default)</param>
    /// <param name="epsilon">Maximum tolerance distance for a point to be considered on a line segment</param>
    /// <returns>True if the point is on the line</returns>
    public static bool BooleanPointOnLine(
        IFeature pointFeature,
        IFeature lineFeature,
        bool ignoreEndVertices = false,
        double? epsilon = null
    )
    {
        if (pointFeature == null)
            throw new ArgumentNullException(nameof(pointFeature), "Point feature is required");

        if (!(pointFeature.Geometry is Point point))
            throw new ArgumentException("Feature geometry must be a Point", nameof(pointFeature));

        if (lineFeature == null)
            throw new ArgumentNullException(nameof(lineFeature), "LineString feature is required");

        if (!(lineFeature.Geometry is LineString line))
            throw new ArgumentException(
                "Feature geometry must be a LineString",
                nameof(lineFeature)
            );

        return BooleanPointOnLine(point, line, ignoreEndVertices, epsilon);
    }

    private static bool IsPointOnLineSegment(
        Coordinate lineSegmentStart,
        Coordinate lineSegmentEnd,
        Coordinate pt,
        string? excludeBoundary,
        double? epsilon
    )
    {
        double x = pt.X;
        double y = pt.Y;
        double x1 = lineSegmentStart.X;
        double y1 = lineSegmentStart.Y;
        double x2 = lineSegmentEnd.X;
        double y2 = lineSegmentEnd.Y;

        double dxc = pt.X - x1;
        double dyc = pt.Y - y1;
        double dxl = x2 - x1;
        double dyl = y2 - y1;
        double cross = dxc * dyl - dyc * dxl;

        // Check if point is within epsilon distance of the line segment
        if (epsilon.HasValue)
        {
            if (Math.Abs(cross) > epsilon.Value)
            {
                return false;
            }
        }
        else if (cross != 0)
        {
            return false;
        }

        // Handle special case where line segment is a point
        if (Math.Abs(dxl) == 0 && Math.Abs(dyl) == 0)
        {
            if (excludeBoundary != null)
            {
                return false;
            }

            if (pt.X == lineSegmentStart.X && pt.Y == lineSegmentStart.Y)
            {
                return true;
            }

            return false;
        }

        // Check if point is on the line segment based on exclude boundary parameter
        if (excludeBoundary == null)
        {
            if (Math.Abs(dxl) >= Math.Abs(dyl))
            {
                return dxl > 0 ? x1 <= x && x <= x2 : x2 <= x && x <= x1;
            }

            return dyl > 0 ? y1 <= y && y <= y2 : y2 <= y && y <= y1;
        }
        else if (excludeBoundary == "start")
        {
            if (Math.Abs(dxl) >= Math.Abs(dyl))
            {
                return dxl > 0 ? x1 < x && x <= x2 : x2 <= x && x < x1;
            }

            return dyl > 0 ? y1 < y && y <= y2 : y2 <= y && y < y1;
        }
        else if (excludeBoundary == "end")
        {
            if (Math.Abs(dxl) >= Math.Abs(dyl))
            {
                return dxl > 0 ? x1 <= x && x < x2 : x2 < x && x <= x1;
            }

            return dyl > 0 ? y1 <= y && y < y2 : y2 < y && y <= y1;
        }
        else if (excludeBoundary == "both")
        {
            if (Math.Abs(dxl) >= Math.Abs(dyl))
            {
                return dxl > 0 ? x1 < x && x < x2 : x2 < x && x < x1;
            }

            return dyl > 0 ? y1 < y && y < y2 : y2 < y && y < y1;
        }

        return false;
    }
}
