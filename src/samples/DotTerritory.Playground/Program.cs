using DotTerritory;
using NetTopologySuite.Geometries;
using UnitsNet;

var point = new Coordinate(-216.64061942615203, -121.90886183483946);
var reference = new Coordinate(-122.39923954010011, 37.79706837753342);

var unitX = new Coordinate(-122.39923954010011 + 10, 37.79706837753342);
var unitY = new Coordinate(-122.39923954010011, 37.79706837753342 + 10);

var lngLine = new LineString([reference, unitX]);
var latLine = new LineString([reference, unitY]);

var lngPoint = Territory.WalkAlong(lngLine, Length.FromMeters(point.X));
var latPoint = Territory.WalkAlong(latLine, Length.FromMeters(point.Y));
var finalPoint = new Coordinate(lngPoint.X, latPoint.Y);

Console.WriteLine($"Longitude point: {lngPoint}");
Console.WriteLine($"Latitude point: {latPoint}");
Console.WriteLine($"Final point: {finalPoint}");
