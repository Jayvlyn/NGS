using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class MapDisplay : Singleton<MapDisplay>
{
    [SerializeField] Tilemap[] importMaps;
    [SerializeField] Tilemap[] exportMaps;
    [SerializeField] TileBase blockerTile;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;
    [SerializeField] GameObject[] ponds;

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
                if (MapManager.Instance.GetVisibleTiles().Contains((x, y)))
                {
                    for(int i = 0; i < importMaps.Length; i++)
                    {
                        if (importMaps[i].GetTile(new Vector3Int(x, y)) != null)
                        {
                            exportMaps[i].SetTile(new Vector3Int(x, y), importMaps[i].GetTile(new Vector3Int(x, y)));
                            exportMaps[i].SetTransformMatrix(new Vector3Int(x, y), importMaps[i].GetTransformMatrix(new Vector3Int(x, y)));
                        }
                    }
                }
                else
                {
                    exportMaps[0].SetTile(new Vector3Int(x, y), blockerTile);
                }
            }
        }
        foreach(GameObject pond in ponds)
        {
            GameObject go = Instantiate(pond);
            go.transform.position = pond.transform.position + transform.position;
            go.GetComponent<SpriteShapeRenderer>().sortingOrder = -1;
        }
    }

    public void ZoomIn()
    {
        foreach (Tilemap exportMap in exportMaps)
        {
            Vector3 result = exportMap.transform.localScale;
            result.x = Mathf.Clamp(result.x + 0.05f, minZoom, maxZoom);
            result.y = Mathf.Clamp(result.y + 0.05f, minZoom, maxZoom);
            exportMap.transform.localScale = result;
        }
    }

    public void ZoomOut()
    {
        foreach (Tilemap exportMap in exportMaps)
        {
            Vector3 result = exportMap.transform.localScale;
            result.x = Mathf.Clamp(result.x + 0.05f, minZoom, maxZoom);
            result.y = Mathf.Clamp(result.y + 0.05f, minZoom, maxZoom);
            exportMap.transform.localScale = result;
        }
    }

    public void ChangePos()
    {

    }
}
