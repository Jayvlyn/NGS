using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacles : MonoBehaviour
{
    [SerializeField] List<GameObject> obstaclesToSpawn;
    [SerializeField] List<Transform> spawnPoints;
    public int min { get; set; }
    public int max { get; set; }

    private void OnEnable()
    {
        int spawnAmount = Random.Range(min, max);
        for (int i = 0; i < spawnAmount; i++)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        if (spawnPoints.Count == 0 || obstaclesToSpawn.Count == 0) return;

        int sIndex = Random.Range(0, spawnPoints.Count);
        int oIndex = Random.Range(0, obstaclesToSpawn.Count);

        Vector3 randomOffset = Random.insideUnitCircle * 150;
        Vector3 spawnPosition = spawnPoints[sIndex].position + randomOffset;

        GameObject spawnedObject = Instantiate(obstaclesToSpawn[oIndex], spawnPosition, spawnPoints[sIndex].rotation, transform.Find("Background").transform);

        spawnedObject.GetComponent<MinigameObstacle>().fishMinigame = GetComponentInChildren<FishMinigame>();
        spawnedObject.SetActive(true);

        
    }
}
