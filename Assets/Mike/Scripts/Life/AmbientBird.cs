using UnityEngine;

public class AmbientBird : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Animator animator;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Collider2D col;
	[Header("Tweaking")]
	[SerializeField] private float minFlySpeed = 2;
	[SerializeField] private float maxFlySpeed = 4;
	[SerializeField] private float eatTimeLowerLimit = 2;
	[SerializeField] private float eatTimeUpperLimit = 10;
	[SerializeField] private Color[] colors;
	private float flySpeed;
	private float eatTimer;
	private bool flying;
	private Transform playerT;
	private Vector2 target;

	private void Start()
	{
		StartEatTimer();
		flySpeed = Random.Range(minFlySpeed, maxFlySpeed);
		FlipX(Mathf.Sign(Random.Range(-1f, 1f)));
		spriteRenderer.color = colors[Random.Range(0, colors.Length - 1)];
	}

	private void Update()
	{
		if (eatTimer > 0)
		{
			eatTimer -= Time.deltaTime;
			if (eatTimer < 0)
			{
				animator.SetTrigger("ToEatingBird");
				StartEatTimer();
			}
		}
		if(flying)
		{
			transform.position = Vector3.MoveTowards(transform.position, target, flySpeed * Time.deltaTime);
			if (Vector2.Distance(transform.position, playerT.position) > 30)
			{
				Destroy(gameObject);
			}
		}
	}

	private void StartEatTimer()
	{
		eatTimer = Random.Range(eatTimeLowerLimit, eatTimeUpperLimit);
	}

	private void FlipX(float dir)
	{
		transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.CompareTag("Player"))
		{
			col.enabled = false;
			FlyOff(Mathf.Sign(transform.position.x - collision.transform.position.x));
			playerT = collision.transform;
		}
	}

	/// <summary>
	/// Call this to make the bird fly off
	/// </summary>
	/// <param name="dir">-1 = left, 1 = right</param>
	private void FlyOff(float dir)
	{
		flying = true;
		animator.SetTrigger("ToFlyingBird");
		FlipX(dir);
		target = new Vector2((transform.position.x + 100) * Mathf.Sign(transform.localScale.x), transform.position.y + Random.Range(20, 100));
	}
}
