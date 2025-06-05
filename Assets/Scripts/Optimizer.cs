using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimizer : MonoBehaviour
{
	public List<GameObject> list = new List<GameObject>();
	public Transform playerT;
	public float cullDistance = 200f;
	public bool onlyCheckX = false;
	public float tick = 2.5f;

	private void Start()
	{
		EnableAll();
		StartCoroutine(DistanceCheck());
	}

	private IEnumerator DistanceCheck()
	{
		while (true)
		{
			DoCheck();
			yield return new WaitForSeconds(tick);
		}
	}

	private void EnableAll()
	{
		for (int i = list.Count - 1; i >= 0; i--)
		{
			GameObject go = list[i];
			go.SetActive(true);
		}
	}

	private void DoCheck()
	{
		for (int i = list.Count - 1; i >= 0; i--)
		{
			GameObject go = list[i];

			if (go == null)
			{
				list.RemoveAt(i);
				continue;
			}

			float dist;
			if (onlyCheckX)
			{
				dist = Mathf.Abs(go.transform.position.x - playerT.transform.position.x);
			}
			else
			{
				dist = Vector2.Distance(go.transform.position, playerT.transform.position);
			}
			if (dist >= cullDistance)
			{
				if (go.activeSelf)
				{
					go.SetActive(false);
				}
			}
			else if (!go.activeSelf)
			{
				go.SetActive(true);
			}
		}
	}
}
