using UnityEngine;
using UnityEngine.InputSystem;

public class ControlBobber : MonoBehaviour
{
	[SerializeField] PlayerInput pi;
	[SerializeField] public float baseMoveSpeed = 5.0f;
	[SerializeField] public float reelSpeed = 0.5f;

	[SerializeField] public Rigidbody2D hookRb;
	[SerializeField] private Rigidbody2D bobberRb;
	[SerializeField] public float lineLength = 500.0f;
	[SerializeField] public float maxLength = 850.0f;

	public float sideLength;
	public float depthLength;

	private HookBehavior hookBehavior;

	[SerializeField] private GameSettings settings;
	[SerializeField] private Canvas gameCanvas;

	private float bobberInput;
	private float hookInput;
	private Vector2 mouseInput;

	void Start()
	{
		hookBehavior = hookRb.GetComponent<HookBehavior>();
		if (gameCanvas == null)
		{
			gameCanvas = GetComponentInParent<Canvas>();
		}
	}

	void FixedUpdate()
	{
		if (settings.toggleData.isMouseModeMinigame)
		{
			HandleMouseInput();
		}
		HandleMovement();
	}

	private void HandleMouseInput()
	{
		float moveSpeed = baseMoveSpeed;
		mouseInput = Input.mousePosition;

		float distance = Vector2.Distance(mouseInput, bobberRb.position);
		if (distance / 20 < 1)
			moveSpeed *= distance / 20;

		if (Mathf.Abs(mouseInput.x - bobberRb.position.x) > 5)
			bobberInput = Mathf.Sign(mouseInput.x - bobberRb.position.x);
		else
			bobberInput = 0;

		if (Mathf.Abs(mouseInput.y - hookRb.position.y) > 5)
			hookInput = Mathf.Sign(mouseInput.y - hookRb.position.y);
		else
			hookInput = 0;
	}

	private void HandleMovement()
	{
		float moveSpeed = baseMoveSpeed;

		// Move bobber horizontally
		if (bobberInput < 0 && bobberRb.transform.localPosition.x >= -sideLength)
		{
			bobberRb.MovePosition(new Vector2(bobberRb.position.x - moveSpeed, bobberRb.position.y));
		}
		else if (bobberInput > 0 && bobberRb.transform.localPosition.x <= sideLength)
		{
			bobberRb.MovePosition(new Vector2(bobberRb.position.x + moveSpeed, bobberRb.position.y));
		}

		// Move hook vertically
		if (hookInput < 0 && hookRb.transform.localPosition.y > -depthLength)
		{
			lineLength += reelSpeed;
			float moveDistance = Vector2.Distance(hookRb.transform.position, new Vector2(bobberRb.transform.position.x, hookRb.transform.position.y)) / hookBehavior.hookResistanceVal;
			float moveFinal = moveDistance * hookBehavior.hookDirection;
			Vector2 movement = new Vector2(hookRb.transform.position.x + moveFinal, hookRb.transform.position.y - reelSpeed);
			hookRb.transform.position = movement;
		}
		else if (hookInput > 0 && hookRb.transform.localPosition.y < depthLength)
		{
			lineLength -= reelSpeed;
			Vector2 direction = (bobberRb.transform.position - hookRb.transform.position).normalized * reelSpeed;
			hookRb.transform.position += (Vector3)direction;
		}
	}

	public void OnMoveBobber(InputValue value)
	{
		bobberInput = value.Get<float>();
	}

	public void OnMoveHook(InputValue value)
	{
		hookInput = value.Get<float>();
	}

	#region INPUT HOLDING HANDLING

	private void OnEnable()
	{
		Application.focusChanged += OnFocusChanged;
	}

	private void OnDisable()
	{
		Application.focusChanged -= OnFocusChanged;
	}

	private void OnFocusChanged(bool hasFocus)
	{
		if (!hasFocus)
		{
			UnholdAllInputs();
		}
	}

	private void UnholdAllInputs()
	{
		hookInput = 0;
		bobberInput = 0;
	}

	#endregion
}
