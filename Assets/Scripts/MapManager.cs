using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    public List<(int, int)> loadedForestTiles;
    public List<(int, int)> loadedDesertTiles;
    public List<(int, int)> loadedSnowTiles;
    public void UpdateVisibility(Vector3 worldPosition )
    {
        
    }
}
