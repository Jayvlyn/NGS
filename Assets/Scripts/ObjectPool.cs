using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	[SerializeField] private GameObject prefab;
	[SerializeField] private int initialSize = 10;

	private Queue<GameObject> pool = new();

	private void Awake()
	{
		for (int i = 0; i < initialSize; i++)
		{
			GameObject obj = Instantiate(prefab, transform);
			obj.SetActive(false);
			pool.Enqueue(obj);
		}
	}

	public GameObject Get(Vector3 position, Quaternion rotation)
	{
		GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab);
		obj.transform.SetPositionAndRotation(position, rotation);
		obj.SetActive(true);
		return obj;
	}

	public void Return(GameObject obj)
	{
		obj.SetActive(false);
		pool.Enqueue(obj);
	}
}
