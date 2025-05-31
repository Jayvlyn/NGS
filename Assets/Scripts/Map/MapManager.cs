using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    public List<ComparableTuple<int, int>> loadedForestTiles = new();
    public List<ComparableTuple<int, int>> loadedDesertTiles = new();
    public List<ComparableTuple<int, int>> loadedSnowTiles = new();
    [SerializeField] private int horizontalView = 11;
    [SerializeField] private int verticalView = 6;
    [SerializeField] private GameSettings gameSettings;
    public void UpdateVisibility(Vector3 worldPosition)
    {
        Vector3Int position = MapDisplay.Instance.WorldToCell(worldPosition);
        List<ComparableTuple<int, int>> currentList = GetVisibleTiles();
        int totalAdded = 0;
        int totalAttempted = 0;
        for(int x = -horizontalView; x <= horizontalView; x++)
        {
            for(int y = -verticalView; y <= verticalView; y++)
            {
                totalAttempted++;
                ComparableTuple<int, int> currentLocation = new ComparableTuple<int, int>(x + position.x, y + position.y);
                if (currentList.BinarySearch(currentLocation) < 0)
                {
                    ListUtils.AddSorted<ComparableTuple<int, int>>(ref currentList, currentLocation);
                    //currentList.Add((x + position.x, y + position.y));
                    MapDisplay.Instance.RevealTile(currentLocation);
                    totalAdded++;
                }
            }
        }
        //Debug.Log($"totalAdded: {totalAdded}");
        //Debug.Log($"totalAttempted: {totalAttempted}");
        //Debug.Log($"Total Discovered Tiles: {currentList.Count}");
    }

    public List<ComparableTuple<int, int>> GetVisibleTiles()
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
