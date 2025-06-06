using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectOnTileGenerator : MonoBehaviour
{
    private readonly Dictionary<Vector3Int, GameObject> currentObjects = new();
    private readonly List<Vector3Int> potentialPositions = new();
    [SerializeField] protected Tilemap tilemap;
    [SerializeField] protected GameObject[] objectPrefabs;
    [SerializeField] protected Vector3 offset = Vector3.zero;
    [SerializeField, Tooltip("Each value must be 0-1, and sorted in ascending order. " +
        "Object in Object Prefabs with the corresponding index will be selected if a random generation between 0 and 1 " +
        "is greater than the object, with higher indexes overrriding lower indexes")] protected float[] chances;
    [SerializeField, Tooltip("Time in seconds of the loop of all objects being destroyed and new ones replacing them, -1 to disable")] protected float resetWithClearTime = 300f;
    [SerializeField, Tooltip("Time in seconds of the loop of generating new objects without destroying old objects, -1 to disable")] protected float resetTime = 150f;
    [SerializeField] protected bool generateOnStart;
    private float currentClearResetTimer;
    private float currentResetTimer;
    [SerializeField] private bool onlyAtNight = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(position))
            {
                potentialPositions.Add(position);
            }
        }
        currentClearResetTimer = resetWithClearTime;
        currentResetTimer = resetTime;
        if (generateOnStart)
        {
            Generate();
        }
    }

    public void Update()
    {
        if(resetWithClearTime != -1)
        {
            currentClearResetTimer -= Time.deltaTime;
            if(currentClearResetTimer < 0)
            {
                currentClearResetTimer = resetWithClearTime;
                currentResetTimer = resetTime;
                Clear();
                Generate();
            }
        }
        if(resetTime != -1)
        {
            currentResetTimer += Time.deltaTime;
            if (currentResetTimer < 0)
            {
                currentResetTimer = resetTime;
                Prune();
                Generate();
            }
        }
        if(onlyAtNight && !DayNightCycle.isNight)
        {
            Clear();
        }
    }

    public void Prune()
    {
        for(int i = 0; i < potentialPositions.Count;)
        {
            if (currentObjects[currentObjects.Keys.ToArray()[i]] == null)
            {
                currentObjects.Remove(currentObjects.Keys.ToArray()[i]);
            }
            else
            {
                i++;
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < currentObjects.Count;)
        {
            if (currentObjects[currentObjects.Keys.ToArray()[i]] == null || Vector3.Distance(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y), tilemap.CellToWorld(currentObjects.Keys.ToArray()[i]) + offset) < Camera.main.orthographicSize * 4)
            {
                i++;
            }
            else
            {
                Destroy(currentObjects[currentObjects.Keys.ToArray()[i]]);
                currentObjects.Remove(currentObjects.Keys.ToArray()[i]);
            }
        }
    }

    public void Generate()
    {
        if (onlyAtNight && !DayNightCycle.isNight) return;

        foreach (Vector3Int position in potentialPositions)
        {
            if (!currentObjects.ContainsKey(position) && Vector3.Distance(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y), tilemap.CellToWorld(position) + offset) >= Camera.main.orthographicSize * 4)
            {
                float generatedType = Random.Range(0f, 1f);
                for (int i = chances.Length - 1; i >= 0; i--)
                {
                    if (chances[i] < generatedType)
                    {
                        currentObjects.Add(position, Instantiate(objectPrefabs[i]));
                        currentObjects[position].transform.position = tilemap.CellToWorld(position) + offset;
                        break;
                    }
                }
            }
        }
    }
}
