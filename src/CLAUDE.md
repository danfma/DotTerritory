# DotTerritory Development Guide

## Build Commands
- Build: `dotnet build`
- Build specific project: `dotnet build DotTerritory/DotTerritory.csproj`
- Build with configuration: `dotnet build -c Release`

## Test Commands
- Run all tests: `dotnet test`
- Run specific test: `dotnet test --filter "FullyQualifiedName=DotTerritory.Tests.AreaTest.AreaShouldCalculatePolygonArea"`
- Run tests in specific class: `dotnet test --filter "FullyQualifiedName~DotTerritory.Tests.AreaTest"`

## Code Style Guidelines
- Use C# 14 (preview) features with nullable reference types enabled
- Follow standard C# naming conventions (PascalCase for public members, camelCase for parameters)
- Use global usings for common imports (NetTopologySuite.Geometries, UnitsNet)
- Format functionality in partial classes by feature (Territory.Area.cs, Territory.Bbox.cs)
- Implement XML documentation for public methods
- Use nullable annotations consistently
- Prefer immutable data structures and pure functions
- Follow TurfJS method signatures where possible
- Use xUnit and Shouldly for writing tests
