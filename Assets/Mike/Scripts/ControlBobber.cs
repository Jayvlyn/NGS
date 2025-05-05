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

    public bool useMouseControl = false;

    [SerializeField] private Canvas gameCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //pi.SwitchCurrentActionMap("Minigame");
        hookBehavior = hookRb.gameObject.GetComponent<HookBehavior>();
        //hookRb.transform.position = new Vector2(transform.position.x, transform.position.y - lineLength);
        
        // If canvas reference is not set, try to find it
        if (gameCanvas == null)
        {
            gameCanvas = GetComponentInParent<Canvas>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (useMouseControl)
        {
            useMouseMovement();
        }
        else
        {
            useWASDMovement();
        }


    }

    public void useWASDMovement()
    {
        float moveSpeed = baseMoveSpeed;
        //Debug.Log($"Bobber Input: {bobberInput}");
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

			//hookRb.MovePosition(movement); // doesnt work in diff scene :(
			hookRb.transform.position = movement;
		}
        if (hookInput > 0 && hookRb.transform.localPosition.y < depthLength)
        {
            lineLength -= reelSpeed;
            Vector2 direction = bobberRb.transform.position - hookRb.transform.position;
            direction.Normalize();
            direction.Scale(new Vector2(reelSpeed, reelSpeed));


            //hookRb.MovePosition(direction + hookRb.position); // doesnt work in diff scene for some reason
			hookRb.transform.position += (Vector3)direction;
		}
    }

    public void useMouseMovement()
    {
        float moveSpeed = baseMoveSpeed;
        // Get raw mouse position
        mouseInput = Input.mousePosition;

        float distance = Vector2.Distance(mouseInput, bobberRb.transform.position);
        if (distance/20 < 1)
        {
            moveSpeed *=  distance/20;  // Adjust speed based on distance from bobber
        }    

        if (Mathf.Abs(mouseInput.x - bobberRb.position.x) > 5)
        {
            if (mouseInput.x - bobberRb.position.x > 0) bobberInput = 1;
            else if (mouseInput.x - bobberRb.position.x < 0) bobberInput = -1;
        }
        else bobberInput = 0;

        if (Mathf.Abs(mouseInput.y - hookRb.position.y) > 5)
        {
            if (mouseInput.y - hookRb.position.y > 0) hookInput = 1;
            else if (mouseInput.y - hookRb.position.y < 0) hookInput = -1;
        }
        else hookInput = 0;
       
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

            //hookRb.MovePosition(movement); // doesnt work in diff scene :(
            hookRb.transform.position = movement;
        }
        if (hookInput > 0 && hookRb.transform.localPosition.y < depthLength)
        {
            lineLength -= reelSpeed;
            Vector2 direction = bobberRb.transform.position - hookRb.transform.position;
            direction.Normalize();
            direction.Scale(new Vector2(reelSpeed, reelSpeed));


            //hookRb.MovePosition(direction + hookRb.position); // doesnt work in diff scene for some reason
            hookRb.transform.position += (Vector3)direction;
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
    Vector2 mouseInput;


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
