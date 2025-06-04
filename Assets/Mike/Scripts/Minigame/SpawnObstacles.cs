using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacles : MonoBehaviour
{
    [SerializeField] List<WeightedObject> obstaclesToSpawn;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] PlayerStats playerStats;
    public int min { get; set; }
    public int max { get; set; }
    public bool randomPosOffset = true;
    public bool reuseSpawnpoints = true;

    private void OnEnable()
    {
        if (!reuseSpawnpoints)
        {
            unusedSpawnPoints = new List<Transform>(spawnPoints);
        }
        int spawnAmount = (int)(Random.Range(min, max) * playerStats.obstacleCount);
        for (int i = 0; i < spawnAmount;)
        {
            i += Spawn(spawnAmount - i);
        }
    }

    private List<Transform> unusedSpawnPoints = new List<Transform>();
    private int Spawn(int maxWeight)
    {
        if (spawnPoints.Count == 0 || obstaclesToSpawn.Count == 0) return maxWeight;

        int sIndex = -1;
        if(reuseSpawnpoints)
        {
            sIndex = Random.Range(0, spawnPoints.Count);
        }
        else
        {
            sIndex = Random.Range(0, unusedSpawnPoints.Count);
        }
        int oIndex = Random.Range(0, obstaclesToSpawn.Count);
        int i = 0;
        for (; obstaclesToSpawn[oIndex].cost > maxWeight && i < obstaclesToSpawn.Count; i++)
        {
            oIndex++;
            if(oIndex == obstaclesToSpawn.Count)
            {
                oIndex = 0;
            }
        }
        if(i == obstaclesToSpawn.Count)
        {
            return maxWeight;
        }
        Vector3 randomOffset = Vector3.zero;
        if(randomPosOffset) randomOffset = Random.insideUnitCircle * 150;

        Vector3 spawnPosition;
        if(reuseSpawnpoints)
        {
            spawnPosition = spawnPoints[sIndex].position + randomOffset;
        }
        else
        {
            spawnPosition = unusedSpawnPoints[sIndex].position + randomOffset;
        }

        GameObject spawnedObject;
        if(reuseSpawnpoints)
        {
            spawnedObject = Instantiate(obstaclesToSpawn[oIndex].obj, spawnPosition, spawnPoints[sIndex].rotation, transform.Find("BackgroundArtContainer").transform);
            spawnedObject.GetComponent<MinigameObstacle>().fishMinigame = GetComponentInChildren<FishMinigame>();
        }
        else
        {
            spawnedObject = Instantiate(obstaclesToSpawn[oIndex].obj, spawnPosition, unusedSpawnPoints[sIndex].rotation, transform.parent.Find("BackgroundArtContainer").transform);
            spawnedObject.GetComponent<MinigameObstacle>().fishMinigame = transform.parent.gameObject.GetComponentInChildren<FishMinigame>();
            unusedSpawnPoints.RemoveAt(sIndex);
        }

        spawnedObject.SetActive(true);
        return obstaclesToSpawn[oIndex].cost;
        
    }
}
