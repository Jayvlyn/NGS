using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SmallFishVisualizer : MonoBehaviour
{
    public Transform left;
    public Transform right;
    public float minSpawnrate = 0.2f;
    public float maxSpawnrate = 1f;
    public float heightVariation = 0.2f;
    private float spawnTimer = 0;
    public GameObject fishPrefab;

    private List<GameObject> leftFish = new List<GameObject>();
    private List<GameObject> rightFish = new List<GameObject>();

    private float fishSpawnrate;

	private void Start()
	{
		fishSpawnrate = Random.Range(minSpawnrate, maxSpawnrate);
	}

	private void Update()
	{
		if(spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            spawnTimer = fishSpawnrate;
            SpawnFish();
        }


        for (int i = leftFish.Count - 1; i >= 0; i--)
        {
            GameObject obj = leftFish[i];
			if (obj.transform.position.x > right.transform.position.x)
			{
				leftFish.Remove(obj);
				Destroy(obj);
			}
		}        
        for (int i = rightFish.Count - 1; i >= 0; i--)
        {
            GameObject obj = rightFish[i];
			if (obj.transform.position.x < left.transform.position.x)
			{
				rightFish.Remove(obj);
				Destroy(obj);
			}
		}

	}

	private void SpawnFish()
    {
        Vector2 spawnLocation = Vector2.zero;
        float flip = 1;
        if (Random.Range(0, 2) == 0)
        {
            spawnLocation = left.position;
        }
        else
        {
            spawnLocation = right.position;
            flip = -1;
        }
        spawnLocation.y += Random.Range(-heightVariation, heightVariation);
        GameObject fish = Instantiate(fishPrefab, spawnLocation, Quaternion.identity);
        fish.transform.localScale = new Vector3(fish.transform.localScale.x * flip, fish.transform.localScale.y, fish.transform.localScale.y);
        if(flip == 1)
        {
            leftFish.Add(fish);
        }
        else
        {
            rightFish.Add(fish);
        }
	}
}
