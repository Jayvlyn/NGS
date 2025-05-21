using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FishPopupEffect : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private float appearDelay = 2f;
    [SerializeField] private float disappearDelay = 5f;
    [SerializeField] private float appearTime = 0.3f;
    [SerializeField] private float disappearTime = 0.3f;

	private void Update()
	{
		transform.Rotate(0,0,turnSpeed * Time.deltaTime);
	}

	private void Start()
	{
		gameObject.transform.localScale = Vector3.zero;

		if(appearCoroutine != null ) StopCoroutine(appearCoroutine);
		appearCoroutine = StartCoroutine(AppearCoroutine());

		//if(disappearCoroutine != null ) StopCoroutine(disappearCoroutine);
		//disappearCoroutine = StartCoroutine(DisappearCoroutine());
	}

	private Coroutine appearCoroutine;
	private IEnumerator AppearCoroutine()
	{
		yield return new WaitForSeconds(appearDelay);
		float t = 0;
		while (t < appearTime)
		{
			t += Time.deltaTime;
			gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / appearTime);
			yield return null;
		}
		appearCoroutine = null;
	}

	private Coroutine disappearCoroutine;
	private IEnumerator DisappearCoroutine()
	{
		yield return new WaitForSeconds(disappearDelay);
		float t = 0;
		while (t < disappearTime)
		{
			t += Time.deltaTime;
			gameObject.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / disappearTime);
			yield return null;
		}
		disappearCoroutine = null;
	}
}
