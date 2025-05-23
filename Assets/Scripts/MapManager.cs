using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    public List<(int, int)> loadedForestTiles;
    public List<(int, int)> loadedDesertTiles;
    public List<(int, int)> loadedSnowTiles;
    [SerializeField] private int horizontalView = 11;
    [SerializeField] private int verticalView = 6;
    [SerializeField] private GameSettings gameSettings;
    public void UpdateVisibility(Vector3 worldPosition)
    {
        Vector3Int position = MapDisplay.Instance.WorldToCell(worldPosition);
        List<(int, int)> currentList;
        switch(gameSettings.position.currentLocation)
        {
            case "Desert":
                currentList = loadedDesertTiles;
                break;
            case "Snow":
                currentList = loadedSnowTiles;
                break;
            default:
                currentList = loadedForestTiles;
                break;
        }

        for(int x = -horizontalView; x <= horizontalView; x++)
        {
            for(int y = -verticalView; y <= verticalView; y++)
            {
                if(!currentList.Contains((x + position.x, y + position.y)))
                {
                    currentList.Add((x + position.x, y + position.y));
                }
            }
        }
    }


}
