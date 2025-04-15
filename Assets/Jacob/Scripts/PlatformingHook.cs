using UnityEngine;

public class PlatformingHook : MonoBehaviour
{
	[SerializeField] private PlatformingPlayerController ppc;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(ppc.currentRodState == PlatformingPlayerController.RodState.CASTING && !collision.CompareTag("Water"))
		{
			ppc.ChangeRodState(PlatformingPlayerController.RodState.HOOKED);
			transform.parent = collision.transform;
		}
	}
}
