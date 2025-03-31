using NetTopologySuite.Features;

namespace DotTerritory;

public static partial class Territory
{
    /// <summary>
    /// Removes redundant coordinates from a geometry.
    /// This includes points that are duplicated, or points which are collinear and can be removed without affecting the shape.
    /// </summary>
    /// <param name="geometry">The geometry to clean</param>
    /// <returns>The cleaned geometry</returns>
    public static Geometry CleanCoords(Geometry geometry)
    {
        if (geometry == null)
            throw new ArgumentNullException(nameof(geometry), "Geometry is required");
            
        return geometry switch
        {
            Point point => point, // Points don't need cleaning
            MultiPoint multiPoint => CleanMultiPoint(multiPoint),
            LineString lineString => CleanLineString(lineString),
            MultiLineString multiLineString => CleanMultiLineString(multiLineString),
            Polygon polygon => CleanPolygon(polygon),
            MultiPolygon multiPolygon => CleanMultiPolygon(multiPolygon),
            _ => throw new ArgumentException($"Geometry type {geometry.GeometryType} not supported", nameof(geometry))
        };
    }
    
    /// <summary>
    /// Removes redundant coordinates from a feature.
    /// This includes points that are duplicated, or points which are collinear and can be removed without affecting the shape.
    /// </summary>
    /// <param name="feature">The feature to clean</param>
    /// <returns>A new feature with the cleaned geometry</returns>
    public static IFeature CleanCoords(IFeature feature)
    {
        if (feature == null)
            throw new ArgumentNullException(nameof(feature), "Feature is required");
            
        var cleanedGeometry = CleanCoords(feature.Geometry);
        return new Feature(cleanedGeometry, feature.Attributes);
    }
    
    private static MultiPoint CleanMultiPoint(MultiPoint multiPoint)
    {
        var points = multiPoint.Geometries;
        var existingPoints = new HashSet<string>();
        var cleanedPoints = new List<Point>();
        
        foreach (Point point in points)
        {
            var coord = point.Coordinate;
            var key = $"{coord.X}-{coord.Y}";
            
            if (!existingPoints.Contains(key))
            {
                cleanedPoints.Add(point);
                existingPoints.Add(key);
            }
        }
        
        return new MultiPoint(cleanedPoints.ToArray());
    }
    
    private static LineString CleanLineString(LineString lineString)
    {
        var points = lineString.Coordinates;
        
        // If it's just two points and they're different, it's already clean
        if (points.Length == 2 && !ArePointsEqual(points[0], points[1]))
            return lineString;
        
        var cleanedPoints = CleanLine(points, "LineString");
        return new LineString(cleanedPoints);
    }
    
    private static MultiLineString CleanMultiLineString(MultiLineString multiLineString)
    {
        var cleanedLines = new List<LineString>();
        
        foreach (LineString lineString in multiLineString.Geometries)
        {
            var cleanedLine = CleanLineString(lineString);
            if (!cleanedLine.IsEmpty)
                cleanedLines.Add(cleanedLine);
        }
        
        return new MultiLineString(cleanedLines.ToArray());
    }
    
    private static Polygon CleanPolygon(Polygon polygon)
    {
        var shell = polygon.ExteriorRing;
        var holes = new List<LinearRing>();
        
        var cleanedShellPoints = CleanLine(shell.Coordinates, "Polygon");
        
        // Ensure the polygon shell is valid (at least 4 points with first and last equal for closed ring)
        if (cleanedShellPoints.Length < 4)
            return Polygon.Empty;
        
        // Check if first and last points are equal (closed ring)
        var first = cleanedShellPoints[0];
        var last = cleanedShellPoints[^1];
        
        if (!ArePointsEqual(first, last))
        {
            // Add the first point to close the ring
            var closedCoords = new Coordinate[cleanedShellPoints.Length + 1];
            Array.Copy(cleanedShellPoints, closedCoords, cleanedShellPoints.Length);
            closedCoords[^1] = first;
            cleanedShellPoints = closedCoords;
        }
        
        var cleanedShell = new LinearRing(cleanedShellPoints);
        
        // Clean each hole
        for (int i = 0; i < polygon.NumInteriorRings; i++)
        {
            var hole = polygon.GetInteriorRingN(i);
            var cleanedHolePoints = CleanLine(hole.Coordinates, "Polygon");
            
            // Ensure the hole is valid
            if (cleanedHolePoints.Length >= 4)
            {
                // Check if first and last points are equal (closed ring)
                first = cleanedHolePoints[0];
                last = cleanedHolePoints[^1];
                
                if (!ArePointsEqual(first, last))
                {
                    // Add the first point to close the ring
                    var closedCoords = new Coordinate[cleanedHolePoints.Length + 1];
                    Array.Copy(cleanedHolePoints, closedCoords, cleanedHolePoints.Length);
                    closedCoords[^1] = first;
                    cleanedHolePoints = closedCoords;
                }
                
                holes.Add(new LinearRing(cleanedHolePoints));
            }
        }
        
        return new Polygon(cleanedShell, holes.ToArray());
    }
    
    private static MultiPolygon CleanMultiPolygon(MultiPolygon multiPolygon)
    {
        var cleanedPolygons = new List<Polygon>();
        
        foreach (Polygon polygon in multiPolygon.Geometries)
        {
            var cleanedPolygon = CleanPolygon(polygon);
            if (!cleanedPolygon.IsEmpty)
                cleanedPolygons.Add(cleanedPolygon);
        }
        
        return new MultiPolygon(cleanedPolygons.ToArray());
    }
    
    private static Coordinate[] CleanLine(Coordinate[] points, string type)
    {
        // If it's just two points and they're different, it's already clean
        if (points.Length == 2 && !ArePointsEqual(points[0], points[1]))
            return points;
        
        var cleanedPoints = new List<Coordinate>();
        var secondToLast = points.Length - 1;
        
        // Add the first point
        cleanedPoints.Add(points[0]);
        
        // Process intermediate points
        for (int i = 1; i < secondToLast; i++)
        {
            var prevAddedPoint = cleanedPoints[^1];
            
            // Skip duplicate points
            if (ArePointsEqual(points[i], prevAddedPoint))
                continue;
            
            cleanedPoints.Add(points[i]);
            var numPoints = cleanedPoints.Count;
            
            // Check if the last three points are collinear and remove middle one if so
            if (numPoints > 2)
            {
                if (IsPointOnLineSegment(cleanedPoints[numPoints - 3], cleanedPoints[numPoints - 1], cleanedPoints[numPoints - 2]))
                {
                    cleanedPoints.RemoveAt(numPoints - 2);
                }
            }
        }
        
        // Add the last point
        cleanedPoints.Add(points[^1]);
        var finalLength = cleanedPoints.Count;
        
        // Special handling for polygons and multipolygons (closed rings)
        if ((type == "Polygon" || type == "MultiPolygon") && 
            ArePointsEqual(points[0], points[^1]) && 
            finalLength < 4)
        {
            throw new InvalidOperationException("Invalid polygon: not enough coordinates after cleaning");
        }
        
        // Special handling for linestrings
        if (type == "LineString" && finalLength < 3)
        {
            return cleanedPoints.ToArray();
        }
        
        // Final check for collinearity between first, last, and second-to-last points
        if (finalLength > 2 && 
            IsPointOnLineSegment(cleanedPoints[finalLength - 3], cleanedPoints[finalLength - 1], cleanedPoints[finalLength - 2]))
        {
            cleanedPoints.RemoveAt(finalLength - 2);
        }
        
        return cleanedPoints.ToArray();
    }
    
    private static bool ArePointsEqual(Coordinate pt1, Coordinate pt2)
    {
        return pt1.X == pt2.X && pt1.Y == pt2.Y;
    }
    
    private static bool IsPointOnLineSegment(Coordinate start, Coordinate end, Coordinate point)
    {
        double x = point.X;
        double y = point.Y;
        double startX = start.X;
        double startY = start.Y;
        double endX = end.X;
        double endY = end.Y;
        
        double dxc = x - startX;
        double dyc = y - startY;
        double dxl = endX - startX;
        double dyl = endY - startY;
        double cross = dxc * dyl - dyc * dxl;
        
        // Check if points are not collinear
        if (cross != 0)
            return false;
        
        // Check if point is within the bounds of the line segment
        if (Math.Abs(dxl) >= Math.Abs(dyl))
            return dxl > 0 ? startX <= x && x <= endX : endX <= x && x <= startX;
        else
            return dyl > 0 ? startY <= y && y <= endY : endY <= y && y <= startY;
    }
}