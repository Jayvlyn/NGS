using UnityEngine;
using System.Collections;

public class BubbleSpawner : MonoBehaviour
{
	[SerializeField] private Transform parent;
	[SerializeField] private ObjectPool pool;
	[SerializeField] private float minSpawnInterval = 0.2f;
	[SerializeField] private float maxSpawnInterval = 0.4f;
	private float timer;

	private void Update()
	{
		if(timer <= 0)
		{
			GameObject obj = pool.Get(transform.position, Quaternion.identity);
			StartCoroutine(ReturnAfterDelay(obj, 5f));
			StartCoroutine(ReturnIfPastHeight(obj, 900));
			timer = Random.Range(minSpawnInterval, maxSpawnInterval);
		}
		else
		{
			timer -= Time.deltaTime;
		}
	}

	private IEnumerator ReturnIfPastHeight(GameObject obj, float height)
	{
		while(obj != null && obj.activeSelf)
		{
			Debug.Log(obj.transform.position.y);
			if (obj.transform.position.y > height) pool.Return(obj); // higher = lower number
			yield return null;
		}
	}

	private IEnumerator ReturnAfterDelay(GameObject obj, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (obj != null && obj.activeSelf)
		{
			pool.Return(obj);
		}
	}
}
