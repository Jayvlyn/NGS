using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacles : MonoBehaviour
{
    [SerializeField] List<WeightedObject> obstaclesToSpawn;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] PlayerStats playerStats;
    public int min { get; set; }
    public int max { get; set; }

    private void OnEnable()
    {
        int spawnAmount = (int)(Random.Range(min, max) * playerStats.obstacleCount);
        for (int i = 0; i < spawnAmount;)
        {
            i += Spawn(spawnAmount - i);
        }
    }

    private int Spawn(int maxWeight)
    {
        if (spawnPoints.Count == 0 || obstaclesToSpawn.Count == 0) return maxWeight;

        int sIndex = Random.Range(0, spawnPoints.Count);
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
        Vector3 randomOffset = Random.insideUnitCircle * 150;
        Vector3 spawnPosition = spawnPoints[sIndex].position + randomOffset;

        GameObject spawnedObject = Instantiate(obstaclesToSpawn[oIndex].obj, spawnPosition, spawnPoints[sIndex].rotation, transform.Find("BackgroundArtContainer").transform);

        spawnedObject.GetComponent<MinigameObstacle>().fishMinigame = GetComponentInChildren<FishMinigame>();
        spawnedObject.SetActive(true);
        return obstaclesToSpawn[oIndex].cost;
        
    }
}
