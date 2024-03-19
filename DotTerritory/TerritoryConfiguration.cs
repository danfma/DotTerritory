using UnitsNet;

namespace DotTerritory;

public static class TerritoryConfiguration
{
    public static Length EarthRadius { get; set; } = Length.FromMeters(6371008.8);
}
