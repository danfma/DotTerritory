# DotTerritory

> [!WARNING]
> **This project is archived and no longer maintained.**
>
> If you're looking for a .NET port of TurfJS, use **[Turfano](https://github.com/danfma/Turfano)** instead.
> It takes an approach much closer to TurfJS (its own GeoJSON types and typed quantities, a faithful port of the
> `@turf/*` function index) and keeps NetTopologySuite fully isolated: the core `Turfano` package has **zero external
> dependencies** and is AOT/trimming friendly, while the optional `Turfano.NetTopologySuite` satellite bridges to NTS
> only where a full planar geometry engine is genuinely needed (e.g. `Buffer`).
>
> DotTerritory remains available as a read-only reference, but it will not receive further updates.

---

A small package to port most functionalities from the excellent TurfJS library to .NET. It's based on the
NetTopologySuite (for reusing most structures from this amazing library) and the UnitsNet (to keep units and values
reasonable and flexible too).

## Why?

I tried hard but I was unable to make the NetTopologySuite 100% compatible with TurfJS and we have a game based on
Mapbox which uses that library.
Also, TurfJS makes some operations more findable for non-GIS programmers, while NetTopologySuite has another great set
of features to manipulate those structures.

## Objective

* Not copy all functionality from TurfJS but most of them which will complement the NetTopologySuite;
* Make usage of units flexible and safe by using UnitsNet when dealing with angles, lenghts, and other units.

## Future

* [ ] Evaluate if it's worth to create a new immutable struct-based representation of the NetTopologySuite types. A
  quick benchmark showed that it's possible to have a gain up to 25% in execution time using almost 40% of the memory (
  comparison of the WalkAlong method). The first tests looks promising but there is a lot of work to do yet.  