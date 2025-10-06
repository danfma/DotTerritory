# Missing TurfJS Functions in DotTerritory

Last Updated: October 6, 2025

## 📊 Summary

- **Total TurfJS packages:** 114
- **Currently implemented in DotTerritory:** 72 (63%)
- **Missing functions:** 42 (37%)

---

## 🚫 Missing Functions by Category

### 📐 Measurement & Center Calculations (3 missing)

- [ ] `turf-center-mean` - Geographic mean center
- [ ] `turf-center-median` - Median center (less affected by outliers)
- [ ] `turf-center-of-mass` - Center based on area weighting

### 🔄 Feature Conversion (11 missing)

- [ ] `turf-line-arc` - Creates circular arc lines
- [ ] `turf-line-overlap` - Finds overlapping portions of lines
- [ ] `turf-line-segment` - Splits lines into segments
- [ ] `turf-line-split` - Splits a line by splitter
- [ ] `turf-line-to-polygon` - Converts lines to polygons
- [ ] `turf-nearest-point-to-line` - Finds nearest point on a line
- [ ] `turf-points-within-polygon` - Filters points within a polygon
- [ ] `turf-polygon-tangents` - Finds tangent lines to polygons
- [ ] `turf-polygon-to-line` - Converts polygons to lines
- [ ] `turf-polygonize` - Creates polygons from line network
- [ ] `turf-unkink-polygon` - Removes self-intersections

### 🌐 Grid Generation (5 missing)

- [ ] `turf-hex-grid` - Hexagonal grid
- [ ] `turf-point-grid` - Point grid
- [ ] `turf-rectangle-grid` - Rectangular grid
- [ ] `turf-square-grid` - Square grid
- [ ] `turf-triangle-grid` - Triangular grid

### 📊 Classification & Analysis (6 missing)

- [ ] `turf-clusters` - Generic clustering
- [ ] `turf-clusters-dbscan` - DBSCAN clustering algorithm
- [ ] `turf-clusters-kmeans` - K-means clustering
- [ ] `turf-moran-index` - Spatial autocorrelation
- [ ] `turf-nearest-neighbor-analysis` - Spatial pattern analysis
- [ ] `turf-quadrat-analysis` - Point pattern analysis

### 🔀 Aggregation & Data Operations (4 missing)

- [ ] `turf-collect` - Collect properties from points within polygons
- [ ] `turf-combine` - Combines features into multi-features
- [ ] `turf-dissolve` - Merges adjacent polygons
- [ ] `turf-tag` - Tags points with polygon properties

### 🎲 Random & Sampling (2 missing)

- [ ] `turf-random` - Generate random features
- [ ] `turf-sample` - Random sampling from FeatureCollection

### 📈 Interpolation (2 missing)

- [ ] `turf-interpolate` - IDW/TIN interpolation
- [ ] `turf-planepoint` - Point on a plane

### ✅ Boolean Operations (2 missing)

- [ ] `turf-boolean-concave` - Tests if polygon is concave
- [ ] `turf-boolean-valid` - Tests if geometry is valid

### 🔧 Meta & Utilities (2 missing)

- [ ] `turf-flatten` - Flattens multi-geometries
- [ ] `turf-geojson-rbush` - Spatial indexing with RBush

### 🎨 Miscellaneous Geometry (8 missing)

- [ ] `turf-directional-mean` - Mean direction of linear features
- [ ] `turf-distance-weight` - Distance-based weighting
- [ ] `turf-ellipse` - Creates ellipse polygons
- [ ] `turf-great-circle` - Great circle routes
- [ ] `turf-mask` - Masks geometries
- [ ] `turf-sector` - Creates circular sectors
- [ ] `turf-shortest-path` - A* pathfinding
- [ ] `turf-standard-deviational-ellipse` - Standard deviational ellipse

---

## ✨ DotTerritory-Specific Implementations

These exist in DotTerritory but are **not** in standard TurfJS:

- `Configuration` - DotTerritory-specific settings (e.g., EarthRadius)
- `Conversion` - Unit/coordinate conversion utilities
- `FeatureCollection` - Additional FeatureCollection helpers

---

## 💡 Implementation Priority Recommendations

### High Priority (Core Geospatial Operations)

1. **Grid generation** - Very common for spatial analysis
2. **Line-to-polygon / Polygon-to-line** - Essential conversions
3. **Points-within-polygon** - Fundamental spatial query
4. **Flatten** - Useful for working with multi-geometries
5. **Boolean-valid** - Important for geometry validation

### Medium Priority (Advanced Features)

1. **Clustering algorithms** (kmeans, dbscan) - Useful for data analysis
2. **Interpolation** - Common in spatial analysis
3. **Great-circle** - Important for long-distance calculations
4. **Ellipse** - Useful geometric shape

### Lower Priority (Specialized)

1. **Statistical analysis** (Moran's Index, quadrat analysis)
2. **Advanced center calculations** (center-of-mass, center-median)
3. **Unkink-polygon** - Edge case handling

### Potentially Skip (May Conflict with NetTopologySuite)

- `geojson-rbush` - NTS has its own spatial indexing
- Some boolean operations - NTS may already provide these

---

## 📝 Notes

- This list is based on comparing the [TurfJS repository](https://github.com/Turfjs/turf) (114 packages) with the current DotTerritory implementation
- Some TurfJS functions may be intentionally excluded if they conflict with NetTopologySuite's approach
- The project is actively evaluating which functions are worth porting vs. relying on NetTopologySuite's built-in capabilities
- Performance and memory considerations are being evaluated for potential struct-based implementations

---

## ✅ Currently Implemented (72 functions)

The following TurfJS functions are already implemented in DotTerritory:

- turf-along
- turf-angle
- turf-area
- turf-bbox-clip
- turf-bbox-polygon
- turf-bbox
- turf-bearing
- turf-bezier-spline
- turf-boolean-clockwise
- turf-boolean-contains
- turf-boolean-crosses
- turf-boolean-disjoint
- turf-boolean-equal
- turf-boolean-intersects
- turf-boolean-overlap
- turf-boolean-parallel
- turf-boolean-point-in-polygon
- turf-boolean-point-on-line
- turf-boolean-touches
- turf-boolean-within
- turf-buffer
- turf-center
- turf-centroid
- turf-circle
- turf-clean-coords
- turf-clone
- turf-concave
- turf-convex
- turf-destination
- turf-difference
- turf-distance
- turf-envelope
- turf-explode
- turf-flip
- turf-helpers
- turf-intersect
- turf-invariant
- turf-isobands
- turf-isolines
- turf-kinks
- turf-length
- turf-line-chunk
- turf-line-intersect
- turf-line-offset
- turf-line-slice-along
- turf-line-slice
- turf-meta
- turf-midpoint
- turf-nearest-point-on-line
- turf-nearest-point
- turf-point-on-feature
- turf-point-to-line-distance
- turf-point-to-polygon-distance
- turf-polygon-smooth
- turf-projection
- turf-rewind
- turf-rhumb-bearing
- turf-rhumb-destination
- turf-rhumb-distance
- turf-simplify
- turf-square
- turf-tesselate
- turf-tin
- turf-transform-rotate
- turf-transform-scale
- turf-transform-translate
- turf-truncate
- turf-union
- turf-voronoi
