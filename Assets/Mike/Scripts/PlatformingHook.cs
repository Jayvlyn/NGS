using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformingHook : MonoBehaviour
{
	public Rigidbody2D rb;
	public Collider2D col;
	[SerializeField] private PlatformingPlayerController ppc;
	[SerializeField] private AudioSource audioSource;


	private void Update()
	{
		if (ppc != null)
		{
			Vector2 direction = (ppc.rodEnd.transform.position - transform.position).normalized;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
		}
	}

	public void PlayHookHitSound()
	{
		audioSource.pitch = Random.Range(0.8f, 1.2f);
		audioSource.Play();
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
