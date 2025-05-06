using UnityEngine;
using System.Collections;

public class BubbleSpawner : MonoBehaviour
{
	[SerializeField] private Transform parent;
	[SerializeField] private ObjectPool pool;
	[SerializeField] private float spawnInterval = 0.5f;
	private float timer;

	private void Update()
	{
		if(timer <= 0)
		{
			GameObject obj = pool.Get(transform.position, Quaternion.identity);
			StartCoroutine(ReturnAfterDelay(obj, 3f));
			timer = spawnInterval;
		}
		else
		{
			timer -= Time.deltaTime;
		}

	}

	private IEnumerator ReturnAfterDelay(GameObject obj, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (obj != null)
		{
			pool.Return(obj);
		}
	}
}
