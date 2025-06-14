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
		if (hookBehavior.settings.toggleData.isMouseModeMinigame)
		{
			HandleMouseInput();
		}

		HandleBobberMovement();

		// Pass vertical input to hookBehavior for vertical movement
		hookBehavior.SetVerticalInput(hookInput);

		// Note: HookBehavior handles horizontal movement internally based on bobber position and resistance
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

	private void HandleBobberMovement()
	{
		float moveSpeed = baseMoveSpeed;

		float horiSpeedMod = 1;

		if (hookBehavior.settings.toggleData.isMouseModeMinigame)
		{
			// hookBehavior.hookParent = bobber
			float dist = Mathf.Abs(Input.mousePosition.x - hookBehavior.hookParent.position.x);
			dist = Mathf.Clamp(dist, 0, HookBehavior.MAX_DIST);
			horiSpeedMod += dist / HookBehavior.MAX_DIST * HookBehavior.NORMALIZE_UPPER_END;
		}

		moveSpeed *= horiSpeedMod * Screen.width/1000;


		// Move bobber horizontally
		if (bobberInput < 0 && bobberRb.transform.localPosition.x >= -sideLength)
		{
			bobberRb.MovePosition(new Vector2(bobberRb.position.x - moveSpeed, bobberRb.position.y));
		}
		else if (bobberInput > 0 && bobberRb.transform.localPosition.x <= sideLength)
		{
			bobberRb.MovePosition(new Vector2(bobberRb.position.x + moveSpeed, bobberRb.position.y));
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
