using UnityEngine;
using UnityEngine.InputSystem;


public class ControlBobber : MonoBehaviour
{
    [SerializeField] PlayerInput pi;
    [SerializeField] public float moveSpeed = 5.0f;
    [SerializeField] public float reelSpeed = 0.5f;

    [SerializeField] public Rigidbody2D hookRb;
	[SerializeField] private Rigidbody2D bobberRb;
    [SerializeField] public float lineLength = 500.0f;
    [SerializeField] public float maxLength = 850.0f;

    public float sideLength;
    public float depthLength;

    private HookBehavior hookBehavior;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pi.SwitchCurrentActionMap("Minigame");
        hookBehavior = hookRb.gameObject.GetComponent<HookBehavior>();
        //hookRb.transform.position = new Vector2(transform.position.x, transform.position.y - lineLength);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (bobberInput < 0 && bobberRb.transform.localPosition.x >= -sideLength)
        {
            bobberRb.MovePosition(new Vector2(bobberRb.position.x - moveSpeed, bobberRb.position.y));
        }

        if (bobberInput > 0 && bobberRb.transform.localPosition.x <= sideLength)
        {
            bobberRb.MovePosition(new Vector2(bobberRb.position.x + moveSpeed, bobberRb.position.y));
        }
        if (hookInput < 0 && hookRb.transform.localPosition.y > -depthLength)
        {
            lineLength += reelSpeed;

            float moveDistance = Vector2.Distance(hookRb.transform.position, new Vector2(bobberRb.transform.position.x, hookRb.transform.position.y)) / hookBehavior.hookResistanceVal;
            float moveFinal = moveDistance * hookBehavior.hookDirection;

            Vector2 movement = new Vector2(hookRb.transform.position.x + moveFinal, hookRb.transform.position.y - reelSpeed);

            hookRb.MovePosition(movement);
        }
        if (hookInput > 0 && hookRb.transform.localPosition.y < depthLength)
        {
            lineLength -= reelSpeed;
            Vector2 direction = bobberRb.transform.position - hookRb.transform.position;
            direction.Normalize();
            direction.Scale(new Vector2(reelSpeed, reelSpeed));
            hookRb.MovePosition(direction + hookRb.position);
        }

    }

    float bobberInput;
    public void OnMoveBobber(InputValue value)
    {
		bobberInput = value.Get<float>();
	}

    float hookInput;
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
		if (hasFocus)
		{
		}
		else
		{ // game lost focus aka player tabbed out
			UnholdAllInputs(); // player is no longer holding input if tabbed out
		}
	}

	private void UnholdAllInputs()
	{
        hookInput = 0;
        bobberInput = 0;
	}

	#endregion
}
