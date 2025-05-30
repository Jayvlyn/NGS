using UnityEngine;

public class LookAtTarget2D : MonoBehaviour
{
	public Transform target;

	void Update()
	{
		if (target == null) return;

		Vector2 direction = target.position - transform.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
		transform.rotation = Quaternion.Euler(0f, 0f, angle);
	}
}
