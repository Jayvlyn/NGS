using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformingHook : MonoBehaviour
{
	public Rigidbody2D rb;
	public Collider2D col;
	[SerializeField] private PlatformingPlayerController ppc;

	private void Update()
	{
		if (ppc != null)
		{
			Vector2 direction = (ppc.transform.position - transform.position).normalized;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
		}
	}

	//private void OnTriggerEnter2D(Collider2D collision)
	//{
	//	if(ppc.currentRodState == PlatformingPlayerController.RodState.CASTING && !collision.CompareTag("Water"))
	//	{
	//		ppc.ChangeRodState(PlatformingPlayerController.RodState.HOOKED);
	//		transform.parent = collision.transform;
	//	}
	//}
}
