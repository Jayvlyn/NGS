using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class MapDisplay : Singleton<MapDisplay>
{
    [SerializeField] Tilemap[] importMaps;
    [SerializeField] Tilemap[] exportMaps;
    [SerializeField] Tilemap blockerMap;
    [SerializeField] TileBase blockerTile;
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;
    [SerializeField] GameObject[] ponds;
    [SerializeField] CinemachineCamera cam;
    private bool open = false;
    private GameObject[] createdObjects;

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
        cam.enabled = false;
        Camera.main.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(0, 180, 0);
        createdObjects = new GameObject[ponds.Length];
        for(int i = 0; i < ponds.Length; i++)
        {
            createdObjects[i] = Instantiate(ponds[i]);
            createdObjects[i].transform.position = ponds[i].transform.position + transform.position;
            //go.GetComponent<SpriteShapeRenderer>().sortingOrder = -1;
        }
        open = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (open)
            {
                Close();
            }
            else
            {
                Display();
            }
        }

        if(Input.mouseScrollDelta.y != 0)
        {
            Zoom(Input.mouseScrollDelta.y);
        }

    }
    public void Close()
    {
        Camera.main.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(0, 180, 0);
        cam.enabled = true;
        foreach(GameObject go in createdObjects)
        {
            if(go != null)
            {
                Destroy(go);
            }
        }
        open = false;
    }

    public void Zoom(float rate = 1)
    {
        foreach (Tilemap exportMap in exportMaps)
        {
            Vector3 newScale = exportMap.transform.localScale;
            newScale.x = Mathf.Clamp((newScale.x + 0.05f) * rate, minZoom, maxZoom);
            newScale.y = Mathf.Clamp((newScale.y + 0.05f) * rate, minZoom, maxZoom);
            exportMap.transform.localScale = newScale;
        }
        Vector3 result = blockerMap.transform.localScale;
        result.x = Mathf.Clamp((result.x + 0.05f) * rate, minZoom, maxZoom);
        result.y = Mathf.Clamp((result.y + 0.05f) * rate, minZoom, maxZoom);
        blockerMap.transform.localScale = result;
    }

    public void RevealTile((int, int) location)
    {
        blockerMap.SetTile(new Vector3Int(location.Item1, location.Item2), null);
    }


    private void Start()
    {
        BoundsInt bounds = importMaps[0].cellBounds;
        foreach (Tilemap tilemap in importMaps)
        {
            if (tilemap != null)
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

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int i = 0; i < importMaps.Length; i++)
                {
                    if (importMaps[i].GetTile(new Vector3Int(x, y)) != null)
                    {
                        exportMaps[i].SetTile(new Vector3Int(x, y), importMaps[i].GetTile(new Vector3Int(x, y)));
                        exportMaps[i].SetTransformMatrix(new Vector3Int(x, y), importMaps[i].GetTransformMatrix(new Vector3Int(x, y)));
                    }
                }
                if(!MapManager.Instance.GetVisibleTiles().Contains((x, y)))
                {
                    blockerMap.SetTile(new Vector3Int(x, y), blockerTile);
                }
            }
        }
    }
}
