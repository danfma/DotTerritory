# DotTerritory Efficiency Analysis Report

## Executive Summary

This report documents performance optimization opportunities identified in the DotTerritory codebase. The analysis focused on finding allocation-heavy patterns, particularly related to LINQ operations and unnecessary intermediate collections. A total of 10 efficiency issues were identified across the codebase that could improve performance, especially for high-frequency operations in geospatial calculations.

## Methodology

The analysis used targeted searches for common inefficiency patterns:
- `.ToArray()` and `.ToList()` materializations
- `.Select()` and `.Reverse()` LINQ operations
- String concatenations in hot paths
- Multiple enumerations of collections
- Unnecessary intermediate allocations

## Identified Issues

### 1. ⚠️ HIGH PRIORITY: Unnecessary `Reverse().ToArray()` in Territory.Along.cs

**File:** `src/DotTerritory/Territory.Along.cs`  
**Line:** 11  
**Severity:** High  
**Impact:** Used in hot path for coordinate walking operations

**Current Code:**
```csharp
if (distance < Length.Zero)
{
    line = new LineString(line.Coordinates.Reverse().ToArray());
    distance *= -1;
}
```

**Issue:**
- `Reverse()` creates an intermediate `IEnumerable<Coordinate>`
- `ToArray()` then materializes this into a new array
- Two allocations instead of one

**Recommended Fix:**
```csharp
if (distance < Length.Zero)
{
    var reversed = line.Coordinates.ToArray();
    Array.Reverse(reversed);
    line = new LineString(reversed);
    distance *= -1;
}
```

**Benefits:**
- Eliminates intermediate enumerable allocation
- Uses in-place `Array.Reverse()` which is more efficient
- Reduces GC pressure

**Status:** ✅ FIXED in this PR

---

### 2. String Concatenation for HashSet Keys in Territory.CleanCoords.cs

**File:** `src/DotTerritory/Territory.CleanCoords.cs`  
**Line:** 68  
**Severity:** Medium  
**Impact:** Called for each coordinate comparison during cleaning operations

**Current Code:**
```csharp
var alreadySeenCoordinates = new HashSet<string>();
// ...
var key = $"{point.X},{point.Y}";
if (!alreadySeenCoordinates.Add(key))
    continue;
```

**Issue:**
- String allocation for each coordinate comparison
- Unnecessary string formatting overhead
- HashSet of strings has higher memory footprint than value types

**Recommended Fix:**
```csharp
var alreadySeenCoordinates = new HashSet<(double X, double Y)>();
// ...
var key = (point.X, point.Y);
if (!alreadySeenCoordinates.Add(key))
    continue;
```

**Benefits:**
- Zero string allocations
- ValueTuple has better memory layout
- More efficient equality comparison

---

### 3. Multiple Collection Enumerations in Territory.Centroid.cs

**File:** `src/DotTerritory/Territory.Centroid.cs`  
**Lines:** 108-113  
**Severity:** Low  
**Impact:** Called when computing centroids of feature collections

**Current Code:**
```csharp
var centroids = features
    .Select(feature => Centroid(feature.Geometry))
    .ToList();

var sumX = centroids.Select(point => point.X).Sum();
var sumY = centroids.Select(point => point.Y).Sum();
```

**Issue:**
- Materializes to list with `ToList()`
- Then uses `Select()` again which creates new enumerables
- Could be done in a single pass

**Recommended Fix:**
```csharp
var centroids = features
    .Select(feature => Centroid(feature.Geometry))
    .ToList();

double sumX = 0, sumY = 0;
foreach (var point in centroids)
{
    sumX += point.X;
    sumY += point.Y;
}
```

**Benefits:**
- Single enumeration of the list
- No intermediate LINQ allocations
- More cache-friendly access pattern

---

### 4. Unnecessary LINQ in Territory.Explode.cs

**File:** `src/DotTerritory/Territory.Explode.cs`  
**Line:** 25  
**Severity:** Low  
**Impact:** Called when exploding geometries into points

**Current Code:**
```csharp
var points = coordinates
    .Select(coord => factory.CreatePoint(coord))
    .ToArray();
```

**Issue:**
- LINQ `Select()` creates intermediate enumerable
- Could use pre-allocated array or loop

**Recommended Fix:**
```csharp
var coordArray = coordinates.ToArray();
var points = new Point[coordArray.Length];
for (int i = 0; i < coordArray.Length; i++)
{
    points[i] = factory.CreatePoint(coordArray[i]);
}
```

**Benefits:**
- Single allocation for the array
- No LINQ overhead
- More predictable performance

---

### 5. Unnecessary LINQ for Coordinate Cloning in Territory.BezierSpline.cs

**File:** `src/DotTerritory/Territory.BezierSpline.cs`  
**Line:** 30  
**Severity:** Low  
**Impact:** Called during bezier spline generation

**Current Code:**
```csharp
var coords = line.Coordinates
    .Select(c => new Coordinate(c))
    .ToList();
```

**Issue:**
- LINQ creates intermediate enumerable just for copying
- `ToList()` allocates list infrastructure

**Recommended Fix:**
```csharp
var originalCoords = line.Coordinates;
var coords = new List<Coordinate>(originalCoords.Length);
for (int i = 0; i < originalCoords.Length; i++)
{
    coords.Add(new Coordinate(originalCoords[i]));
}
```

**Benefits:**
- Pre-sized list avoids resizing
- No LINQ allocation overhead
- Clearer intent

---

### 6. Multiple ToArray() Allocations in Territory.LineChunk.cs

**File:** `src/DotTerritory/Territory.LineChunk.cs`  
**Lines:** 71, 91, 102, 105  
**Severity:** Medium  
**Impact:** Called when chunking lines into segments

**Current Code:**
```csharp
var chunk = currentChunkCoords.ToArray();
// ... multiple times in the method
```

**Issue:**
- Multiple `ToArray()` calls throughout the method
- Each creates a new array allocation
- List grows dynamically causing multiple resizes

**Recommended Fix:**
- Consider using `ArrayPool<Coordinate>` for temporary storage
- Pre-calculate approximate chunk sizes to pre-allocate lists
- Reuse coordinate arrays where possible

**Benefits:**
- Reduced allocations through pooling
- Better memory reuse
- Lower GC pressure for repeated operations

---

### 7. ToArray Without Pre-allocation in Territory.LineSliceAlong.cs

**File:** `src/DotTerritory/Territory.LineSliceAlong.cs`  
**Line:** 134  
**Severity:** Low  
**Impact:** Called during line slicing operations

**Current Code:**
```csharp
return new LineString(slicedCoords.ToArray());
```

**Issue:**
- List grows dynamically during building
- Final `ToArray()` creates yet another allocation
- Could pre-calculate size in many cases

**Recommended Fix:**
- If possible, estimate final size and pre-allocate
- Consider using `ArrayPool<Coordinate>` for temporary storage
- Build directly into appropriately-sized array

**Benefits:**
- Fewer allocations
- Better memory locality
- Reduced list resize operations

---

### 8. Duplicate LINQ Chains in TerritoryUtils.cs

**File:** `src/DotTerritory/TerritoryUtils.cs`  
**Lines:** 17-18  
**Severity:** Low  
**Impact:** Utility method, impact depends on usage frequency

**Current Code:**
```csharp
var firstPoint = coords.First();
var lastPoint = coords.Last();
```

**Issue:**
- `First()` and `Last()` may enumerate the sequence
- For arrays/lists this is fine, but for `IEnumerable<T>` it's wasteful
- Could be optimized with pattern matching

**Recommended Fix:**
```csharp
var coordArray = coords as Coordinate[] ?? coords.ToArray();
var firstPoint = coordArray[0];
var lastPoint = coordArray[^1];
```

**Benefits:**
- Single materialization if needed
- Direct array access instead of LINQ
- Index-based access is faster

---

### 9. Select().ToArray() Pattern in Territory.LineIntersect.cs

**File:** `src/DotTerritory/Territory.LineIntersect.cs`  
**Line:** 126 (approximate)  
**Severity:** Low  
**Impact:** Called during line intersection calculations

**Issue:**
- Common pattern of `Select().ToArray()` creating intermediate enumerables
- Could use pre-allocated array with loop

**Recommended Fix:**
- Replace LINQ chains with explicit loops
- Pre-allocate arrays when size is known
- Use for loops instead of Select

**Benefits:**
- Eliminates LINQ overhead
- More predictable performance
- Better for hot paths

---

### 10. Coordinate Enumeration Inefficiencies in Territory.FeatureCollection.cs

**File:** `src/DotTerritory/Territory.FeatureCollection.cs`  
**Severity:** Low  
**Impact:** Depends on how frequently feature collections are processed

**Issue:**
- Multiple methods traverse coordinate sequences
- Coordinates may be enumerated multiple times
- Could benefit from caching when reused

**Recommended Fix:**
- Cache coordinate arrays when they'll be reused
- Avoid repeated traversals
- Consider lazy evaluation patterns

**Benefits:**
- Reduced traversals
- Better cache utilization
- Lower CPU usage

---

## Prioritization and Next Steps

### High Priority
1. **Territory.Along.cs** - Fixed in this PR ✅
   - Hot path operation
   - Simple fix with immediate benefit

### Medium Priority
2. **Territory.CleanCoords.cs** - String concatenation
   - Moderate impact
   - Straightforward value tuple solution

3. **Territory.LineChunk.cs** - Multiple ToArray() calls
   - Could benefit from ArrayPool
   - More complex refactoring needed

### Low Priority
4-10. Other LINQ and allocation issues
   - Lower impact or less frequently called
   - Good candidates for incremental improvements

## Performance Testing Recommendations

To validate these optimizations:
1. Create benchmark suite using BenchmarkDotNet
2. Focus on hot path operations (WalkAlong, Distance, etc.)
3. Measure allocation rates and GC pressure
4. Profile real-world usage patterns
5. Consider memory-constrained scenarios

## Conclusion

The DotTerritory codebase is well-structured and functional. These optimization opportunities represent incremental improvements that could yield significant benefits in high-throughput scenarios. The fixes range from simple (Reverse().ToArray()) to more complex (ArrayPool usage), allowing for gradual implementation based on priority and available resources.

The most impactful change (Territory.Along.cs) has been implemented in this PR as a proof of concept. Future PRs can address the remaining issues incrementally, with appropriate benchmarking to validate the improvements.

---

**Report Generated:** October 6, 2025  
**Analyzed By:** Devin AI  
**Session:** https://app.devin.ai/sessions/078e83b75da94204a924a2d1a8ac826c
