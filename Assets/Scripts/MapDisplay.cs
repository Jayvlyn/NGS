using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplay : Singleton<MapDisplay>
{
    [SerializeField] Tilemap[] importMaps;
    [SerializeField] Tilemap exportMap;
    [SerializeField] TileBase blockerTile;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;

    public Vector3Int WorldToCell(Vector3 world, int mapIndex = 0)
    {
        if(mapIndex > -1 && mapIndex < importMaps.Length && importMaps[mapIndex] != null)
        {
            return importMaps[mapIndex].WorldToCell(world);
        }
        return new Vector3Int((int)world.x, (int)world.y, (int)world.z);
    }
    public void Display()
    {
        BoundsInt bounds = importMaps[0].cellBounds;
        foreach(Tilemap tilemap in importMaps)
        {
            if(tilemap != null)
            {
                if (tilemap.cellBounds.xMin < bounds.xMin)
                {
                    bounds.xMin = tilemap.cellBounds.xMin;
                }
                if (tilemap.cellBounds.yMin < bounds.yMin)
                {
                    bounds.yMin = tilemap.cellBounds.yMin;
                }
                if (tilemap.cellBounds.xMax > bounds.xMax)
                {
                    bounds.xMax = tilemap.cellBounds.xMax;
                }
                if (tilemap.cellBounds.yMax > bounds.yMax)
                {
                    bounds.yMax = tilemap.cellBounds.yMax;
                }
            }
        }

        for(int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for(int y = bounds.yMin; y < bounds.yMax; y++)
            {
                if (true || MapManager.Instance.GetVisibleTiles().Contains((x, y)))
                {
                    foreach (Tilemap tilemap in importMaps)
                    {
                        if (tilemap.GetTile(new Vector3Int(x, y)) != null)
                        {
                            exportMap.SetTile(new Vector3Int(x, y), tilemap.GetTile(new Vector3Int(x, y)));
                            exportMap.SetTransformMatrix(new Vector3Int(x, y), tilemap.GetTransformMatrix(new Vector3Int(x, y)));
                            break;
                        }
                    }
                }
                else
                {
                    exportMap.SetTile(new Vector3Int(x, y), blockerTile);
                }
            }
        }
    }

    public void ZoomIn()
    {
        Vector3 result = exportMap.transform.localScale;
        result.x = Mathf.Clamp(result.x + 0.05f, minZoom, maxZoom);
        result.y = Mathf.Clamp(result.y + 0.05f, minZoom, maxZoom);
        exportMap.transform.localScale = result;
    }

    public void ZoomOut()
    {
        Vector3 result = exportMap.transform.localScale;
        result.x = Mathf.Clamp(result.x + 0.05f, minZoom, maxZoom);
        result.y = Mathf.Clamp(result.y + 0.05f, minZoom, maxZoom);
        exportMap.transform.localScale = result;
    }

    public void ChangePos()
    {

    }
}
