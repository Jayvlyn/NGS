using UnityEngine;

public class AmbientBird : MonoBehaviour
{
	public Animator animator;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.CompareTag("Player"))
		{
			FlyOff(Mathf.Sign(transform.position.x - collision.transform.position.x));
		}
	}

	/// <summary>
	/// Call this to make the bird fly off
	/// </summary>
	/// <param name="dir">-1 = left, 1 = right</param>
	private void FlyOff(float dir)
	{
		animator.SetTrigger("ToFlyingBird");
		transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y);
		//Vector3.MoveTowards(transform.position, transform.right )
	}
}
