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
			StartCoroutine(ReturnIfPastHeight(obj, 410));
			timer = spawnInterval;
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
			if (transform.position.y > height) pool.Return(obj);
			yield return null;
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
