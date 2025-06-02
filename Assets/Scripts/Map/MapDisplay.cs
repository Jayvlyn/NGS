using NUnit.Framework;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UIElements;

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
    [SerializeField] CinemachinePositionComposer composer;
    [SerializeField] CinemachineConfiner2D confiner;
    [SerializeField] Material mapMaterial;
    private bool open = false;
    private GameObject[] createdObjects;
    private float size;
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
        GameUI.Instance.pi.SwitchCurrentActionMap("RebindKeys");
        //Camera.main.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(0, 180, 0);
        cam.transform.rotation = Quaternion.Euler(180, 0, 0);
        //cam.get
        composer.enabled = false;
        confiner.enabled = true;
        createdObjects = new GameObject[ponds.Length];
        for(int i = 0; i < ponds.Length; i++)
        {
            createdObjects[i] = Instantiate(ponds[i]);
            createdObjects[i].transform.position = ponds[i].transform.position + transform.position;
            //go.GetComponent<SpriteShapeRenderer>().sortingOrder = -1;
        }
        size = cam.Lens.OrthographicSize;
        Camera.main.ResetProjectionMatrix();
        Camera.main.projectionMatrix *= Matrix4x4.Scale(new Vector3(1, -1, 1));
        //Matrix4x4 mat = Camera.main.projectionMatrix;
        //Matrix4x4 next = Matrix4x4.Scale(new Vector3(-1, 1, 1));
        //mat *= next;
        //Camera.main.projectionMatrix = mat;
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
        if (open)
        {
            cam.transform.position = Camera.main.transform.position;
            if (Input.mouseScrollDelta.y != 0)
            {
                Zoom(-Input.mouseScrollDelta.y);
            }

            Vector3 movement = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                movement.y += Time.deltaTime * Camera.main.orthographicSize;
            }
            if (Input.GetKey(KeyCode.S))
            {
                movement.y -= Time.deltaTime * Camera.main.orthographicSize;
            }
            if (Input.GetKey(KeyCode.D))
            {
                movement.x += Time.deltaTime * Camera.main.orthographicSize;
            }
            if (Input.GetKey(KeyCode.A))
            {
                movement.x -= Time.deltaTime * Camera.main.orthographicSize;
            }
            if (Input.GetKey(KeyCode.Minus))
            {
                Zoom(Time.deltaTime * 20);
            }
            if (Input.GetKey(KeyCode.Equals))
            {
                Zoom(-Time.deltaTime * 20);
            }
            if(Input.mousePosition.x < Screen.width * 0.1f)
            {
                movement.x -= Time.deltaTime * Camera.main.orthographicSize;
            }
            else if(Input.mousePosition.x > Screen.width * 0.9f)
            {
                movement.x += Time.deltaTime * Camera.main.orthographicSize;
            }
            if(Input.mousePosition.y < Screen.height * 0.1f)
            {
                movement.y -= Time.deltaTime * Camera.main.orthographicSize;
            }
            else if(Input.mousePosition.y > Screen.height * 0.9f)
            {
                movement.y += Time.deltaTime * Camera.main.orthographicSize;
            }
            cam.transform.position += movement;
        }
    }
    public void Close()
    {
        GameUI.Instance.pi.SwitchCurrentActionMap("Platformer");
        //Camera.main.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(0, 180, 0); 
        confiner.enabled = false;
        cam.transform.rotation = Quaternion.identity;
        Camera.main.ResetProjectionMatrix();
        composer.enabled = true;
        foreach(GameObject go in createdObjects)
        {
            if(go != null)
            {
                Destroy(go);
            }
        }
        cam.Lens.OrthographicSize = size;
        open = false;
    }

    public void Zoom(float rate = 1)
    {
        cam.Lens.OrthographicSize = Mathf.Clamp(cam.Lens.OrthographicSize * (1 + rate * 0.05f), 1 / maxZoom, 1 / minZoom);
        Camera.main.ResetProjectionMatrix();
        Camera.main.projectionMatrix *= Matrix4x4.Scale(new Vector3(1, -1, 1));
        confiner.InvalidateBoundingShapeCache();
    }

    public void RevealTile(ComparableTuple<int, int> location)
    {
        blockerMap.SetTile(new Vector3Int(location.Item1, location.Item2), null);
    }
    public void Start()
    {
        BoundsInt bounds = new BoundsInt();
        for (int i = 0; i < importMaps.Length; i++)
        {
            Tilemap tilemap = importMaps[i];
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
                if (!MapManager.Instance.GetVisibleTiles().Contains(new ComparableTuple<int, int>(x, y)))
                {
                    blockerMap.SetTile(new Vector3Int(x, y), blockerTile);
                }
            }
        }
    }



    public void GenerateMap()
    {
        if(exportMaps == null)
        {
            exportMaps = new Tilemap[0];
        }
        BoundsInt bounds = importMaps[0].cellBounds;
        foreach (Tilemap tilemap in exportMaps)
        {
            if (tilemap != null)
            {
                Destroy(tilemap.gameObject);
            }
        }
        exportMaps = new Tilemap[importMaps.Length];
        for(int i = 0; i < importMaps.Length; i++)
        {
            Tilemap tilemap = importMaps[i];
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
                GameObject go = new();
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.name = $"{tilemap.gameObject.name} (Map)";
                exportMaps[i] = go.AddComponent<Tilemap>();
                exportMaps[i].color = tilemap.color;
                exportMaps[i].tileAnchor = tilemap.tileAnchor;
                TilemapRenderer mapRenderer = go.AddComponent<TilemapRenderer>();
                TilemapRenderer normalRenderer = tilemap.gameObject.GetComponent<TilemapRenderer>();
                mapRenderer.renderingLayerMask = normalRenderer.renderingLayerMask;
                mapRenderer.material = mapMaterial;
                mapRenderer.sortingOrder = normalRenderer.sortingOrder;
                mapRenderer.sortOrder = normalRenderer.sortOrder;
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
            }
        }
    }
}
