using System.Collections.Generic;
using UnityEngine;

public class SmallFishVisualizer : MonoBehaviour
{
    public Transform left;
    public Transform right;
    public float fishSpawnrate = 0.5f;
    public float heightVariation = 0.2f;
    private float spawnTimer = 0;
    public GameObject fishPrefab;

    private List<GameObject> leftFish = new List<GameObject>();
    private List<GameObject> rightFish = new List<GameObject>();

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

        foreach (GameObject obj in leftFish)
        {
            if (obj.transform.position.x > right.transform.position.x)
            {
                leftFish.Remove(obj);
                Destroy(obj);
            }
        }
        foreach (GameObject obj in rightFish)
        {
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
