using UnityEngine;

public class SwimmingFishManager : MonoBehaviour
{
	// Treat as component on player in boss scene

	[SerializeField] private GameObject fishPrefab;
	[SerializeField] private AllFish allFish;

	[SerializeField] private Transform spawnpointsParent;
	[SerializeField] private Transform[] fishSpawnpoints;

	[SerializeField] private float spawnInterval;
	private float timer;

	[SerializeField] private int lowerRot = 30;
	[SerializeField] private int upperRot = 160;
	[SerializeField] private float distMult = 1.5f;

    private void Update()
    {
        if(timer <= 0)
		{
			SpawnSchool();
			timer = spawnInterval;
		}
		else
		{
			timer -= Time.deltaTime;
		}
    }

    public void SpawnSchool()
	{
		Vector3 offset = transform.up;
		float randomZ = -Random.Range(lowerRot,upperRot);
        if (Random.Range(0, 2) == 0)
		{
			offset.y *= -1;
			randomZ *= -1;
		}

		spawnpointsParent.position = transform.position + ((transform.right + offset) * distMult);

		int fishAmt = Random.Range(3, 10);

		Sprite fishSprite = allFish.list[Random.Range(0, allFish.list.Count)].sprite;

		for (int i = 0; i < fishAmt; i++)
		{
			GameObject fish = Instantiate(fishPrefab, fishSpawnpoints[i].position, Quaternion.Euler(0,0,randomZ));
			if(fish.TryGetComponent(out SpriteRenderer sr))
			{
				sr.sprite = fishSprite;
			}
		}
	}

}
