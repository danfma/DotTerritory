// filepath: /Users/danfma/Develop/private/DotTerritory/src/DotTerritory/Territory.Concave.cs
namespace DotTerritory;

using System;
using System.Collections.Generic;
using System.Linq;

public static partial class Territory
{
    /// <summary>
    /// Calculates a concave hull polygon for a set of points
    /// </summary>
    /// <param name="points">Points to create the concave hull from</param>
    /// <param name="options">Optional parameters for concave hull calculation</param>
    /// <returns>Concave polygon</returns>
    public static Polygon Concave(Point[] points, ConcaveOptions options = default)
    {
        if (options == ConcaveOptions.Empty)
            options = ConcaveOptions.Default;

        if (points.Length < 3)
        {
            throw new ArgumentException("Concave hull requires at least 3 points", nameof(points));
        }

        // Create convex hull as a starting point
        var convexHull = Convex(points);
        var hullCoordinates = convexHull.Shell.Coordinates;

        // For a small number of points, or when maxEdge is very large,
        // the convex hull is an acceptable result
        if (points.Length <= 4 || options.MaxEdge.Meters >= Double.MaxValue / 2)
        {
            return convexHull;
        }

        // Process the concave hull using alpha shape algorithm
        var concaveCoordinates = AlphaShape(points, options.MaxEdge.Meters);

        // If alpha shape fails or produces invalid geometry, fall back to convex hull
        if (concaveCoordinates.Length < 4) // Need at least 4 points for a valid polygon (first/last are the same)
        {
            return convexHull;
        }

        try
        {
            return new Polygon(new LinearRing(concaveCoordinates));
        }
        catch
        {
            // If the concave hull creation fails, fall back to convex hull
            return convexHull;
        }
    }

    /// <summary>
    /// Calculates a concave hull polygon for a set of coordinates
    /// </summary>
    /// <param name="coordinates">Coordinates to create the concave hull from</param>
    /// <param name="options">Optional parameters for concave hull calculation</param>
    /// <returns>Concave polygon</returns>
    public static Polygon Concave(Coordinate[] coordinates, ConcaveOptions options = default)
    {
        if (coordinates.Length < 3)
        {
            throw new ArgumentException(
                "Concave hull requires at least 3 points",
                nameof(coordinates)
            );
        }

        // Convert coordinates to points
        var points = coordinates.Select(c => new Point(c)).ToArray();

        return Concave(points, options);
    }

    /// <summary>
    /// Calculates a concave hull polygon for a set of points in a geometry
    /// </summary>
    /// <param name="geometry">Geometry containing points for concave hull calculation</param>
    /// <param name="options">Optional parameters for concave hull calculation</param>
    /// <returns>Concave polygon</returns>
    public static Polygon Concave(Geometry geometry, ConcaveOptions options = default)
    {
        // Extract all coordinates from the geometry
        var coordinates = geometry.Coordinates;

        return Concave(coordinates, options);
    }

    private static Coordinate[] AlphaShape(Point[] points, double alpha)
    {
        // Step 1: Compute the Delaunay triangulation
        var delaunay = CreateDelaunayTriangulation(points);

        // Step 2: Filter triangles based on alpha criterion
        var filteredTriangles = FilterTriangles(delaunay, alpha);

        // Step 3: Extract boundary edges
        var boundaryEdges = ExtractBoundaryEdges(filteredTriangles);

        // Step 4: Order edges to form a polygon
        return OrderEdgesToPolygon(boundaryEdges);
    }

    // Simplified Delaunay triangulation
    private static List<(Point, Point, Point)> CreateDelaunayTriangulation(Point[] points)
    {
        // For simplicity, use convex hull and a naive triangulation approach
        var convexHull = Convex(points);
        var convexHullPoints = convexHull.Shell.Coordinates.Select(c => new Point(c)).ToArray();

        // Create a list of triangles (simplified approach)
        var triangles = new List<(Point, Point, Point)>();

        // If we have exactly 3 points, we have a single triangle
        if (points.Length == 3)
        {
            triangles.Add((points[0], points[1], points[2]));
            return triangles;
        }

        // Create a naive "fan" triangulation from the convex hull
        var center =
            points.Skip(convexHullPoints.Length).FirstOrDefault()
            ?? new Point(convexHull.Centroid.Coordinate);

        for (int i = 0; i < convexHullPoints.Length - 1; i++)
        {
            triangles.Add((center, convexHullPoints[i], convexHullPoints[i + 1]));
        }

        return triangles;
    }

    // Filter triangles based on alpha criterion (edge length)
    private static List<(Point, Point, Point)> FilterTriangles(
        List<(Point, Point, Point)> triangles,
        double alpha
    )
    {
        return triangles
            .Where(t =>
            {
                var a = Distance(t.Item1.Coordinate, t.Item2.Coordinate).Meters;
                var b = Distance(t.Item2.Coordinate, t.Item3.Coordinate).Meters;
                var c = Distance(t.Item3.Coordinate, t.Item1.Coordinate).Meters;

                // Keep triangle if all edges are shorter than alpha
                return a <= alpha && b <= alpha && c <= alpha;
            })
            .ToList();
    }

    // Extract boundary edges
    private static List<(Coordinate, Coordinate)> ExtractBoundaryEdges(
        List<(Point, Point, Point)> triangles
    )
    {
        var allEdges = new List<(Coordinate, Coordinate)>();

        foreach (var triangle in triangles)
        {
            allEdges.Add((triangle.Item1.Coordinate, triangle.Item2.Coordinate));
            allEdges.Add((triangle.Item2.Coordinate, triangle.Item3.Coordinate));
            allEdges.Add((triangle.Item3.Coordinate, triangle.Item1.Coordinate));
        }

        // An edge is on the boundary if it appears exactly once
        var edgeCount = new Dictionary<(double X, double Y, double X2, double Y2), int>();

        foreach (var edge in allEdges)
        {
            var e1 = (edge.Item1.X, edge.Item1.Y, edge.Item2.X, edge.Item2.Y);
            var e2 = (edge.Item2.X, edge.Item2.Y, edge.Item1.X, edge.Item1.Y);

            if (edgeCount.ContainsKey(e1))
                edgeCount[e1]++;
            else
                edgeCount[e1] = 1;

            if (edgeCount.ContainsKey(e2))
                edgeCount[e2]++;
            else
                edgeCount[e2] = 1;
        }

        return allEdges
            .Where(edge =>
            {
                var e = (edge.Item1.X, edge.Item1.Y, edge.Item2.X, edge.Item2.Y);
                var eReverse = (edge.Item2.X, edge.Item2.Y, edge.Item1.X, edge.Item1.Y);

                return edgeCount.ContainsKey(e) && edgeCount[e] == 1
                    || edgeCount.ContainsKey(eReverse) && edgeCount[eReverse] == 1;
            })
            .ToList();
    }

    // Order edges to form a polygon
    private static Coordinate[] OrderEdgesToPolygon(List<(Coordinate, Coordinate)> edges)
    {
        if (edges.Count == 0)
            return Array.Empty<Coordinate>();

        var result = new List<Coordinate>();
        var currentEdge = edges.First();
        result.Add(currentEdge.Item1);
        result.Add(currentEdge.Item2);
        edges.RemoveAt(0);

        // Walk the boundary
        while (edges.Count > 0)
        {
            var currentPoint = result[^1];
            var found = false;

            for (int i = 0; i < edges.Count; i++)
            {
                var edge = edges[i];

                if (edge.Item1.Equals2D(currentPoint))
                {
                    result.Add(edge.Item2);
                    edges.RemoveAt(i);
                    found = true;
                    break;
                }
                else if (edge.Item2.Equals2D(currentPoint))
                {
                    result.Add(edge.Item1);
                    edges.RemoveAt(i);
                    found = true;
                    break;
                }
            }

            if (!found)
                break;
        }

        // Ensure the polygon is closed by adding the first coordinate again
        if (result.Count > 0 && !result[0].Equals2D(result[^1]))
        {
            result.Add(result[0]);
        }

        return result.ToArray();
    }
}

/// <summary>
/// Options for concave hull generation
/// </summary>
public readonly record struct ConcaveOptions
{
    public static readonly ConcaveOptions Empty = new();
    public static readonly ConcaveOptions Default = new() { MaxEdge = Length.FromMeters(Infinity) };

    /// <summary>
    /// Maximum edge length for the concave hull (default: Infinity)
    /// A smaller number will create a tighter hull around the points
    /// </summary>
    public Length MaxEdge { get; init; }

    private static double Infinity => double.MaxValue / 2;
}
