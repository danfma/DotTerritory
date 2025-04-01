using NetTopologySuite.Geometries;
using UnitsNet;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Takes a bounding box and calculates the minimum square bounding box that
    /// would contain the input.
    /// </summary>
    /// <param name="bbox">The bounding box to square</param>
    /// <returns>A squared BBox</returns>
    /// <example>
    /// <code>
    /// var bbox = new[] { -20, -20, -15, 0 };
    /// var squared = Territory.Square(bbox);
    /// // squared =>  [-27.5, -20, -7.5, 0]
    /// </code>
    /// </example>
    public static BBox Square(BBox bbox)
    {
        double width = bbox.East - bbox.West;
        double height = bbox.North - bbox.South;
        double midX = (bbox.West + bbox.East) / 2;
        double midY = (bbox.South + bbox.North) / 2;

        double maxSize = Math.Max(width, height);
        double west = midX - maxSize / 2;
        double east = midX + maxSize / 2;
        double south = midY - maxSize / 2;
        double north = midY + maxSize / 2;

        return new BBox(west, south, east, north);
    }

    /// <summary>
    /// Takes a bounding box and calculates the minimum square bounding box that
    /// would contain the input.
    /// </summary>
    /// <param name="west">The westernmost longitude</param>
    /// <param name="south">The southernmost latitude</param>
    /// <param name="east">The easternmost longitude</param>
    /// <param name="north">The northernmost latitude</param>
    /// <returns>A squared BBox</returns>
    public static BBox Square(double west, double south, double east, double north)
    {
        return Square(new BBox(west, south, east, north));
    }
}
