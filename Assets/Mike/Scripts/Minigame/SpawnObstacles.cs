using GameEvents;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacles : MonoBehaviour
{
    [SerializeField] List<GameObject> obstaclesToSpawn;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] private int spawnAmount = 0;

    private void OnEnable()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Spawn();
        }
    }

    public void SetAmount(int amt)
    {
        spawnAmount = amt;
    }

    private void Spawn()
    {
        if (spawnPoints.Count == 0 || obstaclesToSpawn.Count == 0) return;

        int sIndex = Random.Range(0, spawnPoints.Count);
        int oIndex = Random.Range(0, obstaclesToSpawn.Count);

        Vector3 randomOffset = Random.insideUnitCircle * 150;
        Vector3 spawnPosition = spawnPoints[sIndex].position + randomOffset;

        GameObject spawnedObject = Instantiate(obstaclesToSpawn[oIndex], spawnPosition, spawnPoints[sIndex].rotation, transform);

        spawnedObject.GetComponent<MinigameObstacle>().fishMinigame = GetComponentInChildren<FishMinigame>();
        spawnedObject.SetActive(true);

        
    }
}
