using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    public List<(int, int)> loadedForestTiles = new();
    public List<(int, int)> loadedDesertTiles = new();
    public List<(int, int)> loadedSnowTiles = new();
    [SerializeField] private int horizontalView = 11;
    [SerializeField] private int verticalView = 6;
    [SerializeField] private GameSettings gameSettings;
    public void UpdateVisibility(Vector3 worldPosition)
    {
        Vector3Int position = MapDisplay.Instance.WorldToCell(worldPosition);
        List<(int, int)> currentList = GetVisibleTiles();

        for(int x = -horizontalView; x <= horizontalView; x++)
        {
            for(int y = -verticalView; y <= verticalView; y++)
            {
                if(!currentList.Contains((x + position.x, y + position.y)))
                {
                    currentList.Add((x + position.x, y + position.y));
                    MapDisplay.Instance.RevealTile((x + position.x, y + position.y));
                }
            }
        }
    }

    public List<(int, int)> GetVisibleTiles()
    {
        return gameSettings.position.currentLocation switch
        {
            "Desert" => loadedDesertTiles,
            "Snow" => loadedSnowTiles,
            _ => loadedForestTiles,
        };
    }

    private void Start()
    {
        //MapDisplay.Instance.Display();
    }

    
}
