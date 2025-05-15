using UnityEngine;

public class HookBehavior : MonoBehaviour
{
	[SerializeField] public Transform hookParent;
	public float hookResistanceVal = 25.0f; // Higher = slower
	private float originalResistanceVal;
	public int hookDirection = 0;

	private float verticalInput = 0f;
	public float verticalSpeed = 0.5f;

	private Rigidbody2D rb;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.gravityScale = 0; // Prevent falling
		originalResistanceVal = hookResistanceVal;
	}

	void FixedUpdate()
	{
		HookOutOfBoundsCheck();
		MoveHook();
		RotateHookToBobber();
	}

	public void SetVerticalInput(float input)
	{
		verticalInput = input;
	}

	void MoveHook()
	{
		float horizontalMove = 0f;

		if (hookDirection != 0)
		{
			float distanceToBobber = Vector2.Distance(transform.position, new Vector2(hookParent.position.x, transform.position.y));
			horizontalMove = (distanceToBobber / hookResistanceVal) * hookDirection;
		}

		float verticalMove = verticalInput * verticalSpeed;

		Vector2 newPos = rb.position + new Vector2(horizontalMove, verticalMove);
		rb.MovePosition(newPos);
	}

	void HookOutOfBoundsCheck()
	{
		float xDiff = hookParent.position.x - transform.position.x;

		if (Mathf.Abs(xDiff) >= 10f)
		{
			hookDirection = xDiff < 0 ? -1 : 1;
		}
		else
		{
			hookDirection = 0;
		}
	}

	void RotateHookToBobber()
	{
		Vector2 dir = hookParent.position - transform.position;
		float angle = -dir.x / 10f;
		Vector3 rotation = transform.localEulerAngles;
		rotation.z = angle;
		transform.localEulerAngles = rotation;
	}

	public void ChangeSpeed(float newVal)
	{
		hookResistanceVal = newVal;
	}

	public void ResetSpeed()
	{
		hookResistanceVal = originalResistanceVal;
	}
}
