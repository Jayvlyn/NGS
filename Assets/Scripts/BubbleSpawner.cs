using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BubbleSpawner : MonoBehaviour
{
	[SerializeField] private Transform parent;
	[SerializeField] private ObjectPool pool;
	[SerializeField] private float minSpawnInterval = 0.2f;
	[SerializeField] private float maxSpawnInterval = 0.4f;
	private float timer;
	List<GameObject> activeBubbles = new List<GameObject>();

	private void Update()
	{
		if(timer <= 0)
		{
			GameObject obj = pool.Get(transform.position, Quaternion.identity);
			activeBubbles.Add(obj);
			StartCoroutine(ReturnAfterDelay(obj, 5f));
			StartCoroutine(ReturnIfPastHeight(obj, 900));
			timer = Random.Range(minSpawnInterval, maxSpawnInterval);
		}
		else
		{
			timer -= Time.deltaTime;
		}
	}

	private void OnDisable()
	{
		foreach(GameObject bubble in activeBubbles)
		{
			pool.Return(bubble);
		}
	}

	private IEnumerator ReturnIfPastHeight(GameObject obj, float height)
	{
		while(obj != null && obj.activeSelf)
		{
			//Debug.Log(obj.transform.position.y);
			if (obj.transform.position.y > height)
			{
				pool.Return(obj); // higher = lower number
				activeBubbles.Remove(obj);
			}
			yield return null;
		}
	}

	private IEnumerator ReturnAfterDelay(GameObject obj, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (obj != null && obj.activeSelf)
		{
			pool.Return(obj);
			activeBubbles.Remove(obj);
		}
	}
}
