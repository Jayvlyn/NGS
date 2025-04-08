using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformingPlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera cam;
    [SerializeField] private DistanceJoint2D distanceJoint;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private float moveVelocityLimit = 10f;
    [SerializeField] private float bhopVelocityLimit = 20f;

    [SerializeField] private float jumpForce = 20f;

    [SerializeField] private float bhopForce = 20f;

    [SerializeField] private float maxLineLength = 10f;

    [SerializeField] private float castSpeed = 10f;

    [SerializeField] private float reelSpeed = 10f;

    [SerializeField,Tooltip("When changing direction, the players curent speed is used for counteraction, this variable multiplies against the current player speed")] 
    private float changeDirSpeedMult = 0.5f;

    [SerializeField,Tooltip("Linear X Velocity of player will get multiplied by this value while on ground (1 = no friction)"),Range(0.8f, 1)] 
    private float groundFriction = 0.98f;

    [SerializeField,Tooltip("Time jump input will be stored so the player will jump again once then hit the ground")] 
    private float jumpBufferTime = 0.1f;

    [SerializeField] private float coyoteTime = 0.1f;

    [SerializeField] private int totalJumps = 1;
    private int currentJumps;

    [SerializeField,Tooltip("Time player must hold jump input for full jump height")] 
    private float fullJumpInputTime = 0.5f;

    [SerializeField,Tooltip("Time before and after landing where player will successfully bunny hop")] 
    private float bunnyHopWindow = 0.05f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckT;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    private bool onGround;

    [Header("Wall Checks")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform rightCheckT;
    [SerializeField] private Vector2 rightCheckSize = new Vector2(0.1f, 0.5f);
    [SerializeField] private Transform leftCheckT;
    [SerializeField] private Vector2 leftCheckSize = new Vector2(0.1f, 0.5f);

    // Inputs
    private float moveInput; // left-right 1D axis
    // Handle Held Inputs
    private bool moveHeld = false;
    private bool jumpHeld = false;
    private bool reelHeld = false;

	private void Start()
	{
        currentJumps = totalJumps;

        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
	}

	private void Update()
    {
        onGround = isGrounded();

        // Timers
        if(jumpBuffer >= 0)
        {
            jumpBuffer -= Time.deltaTime;
        }
        if(landTimer <= bunnyHopWindow)
        {
            landTimer += Time.deltaTime;
        }

        // Graphic Update
		if (distanceJoint.enabled) // grapple active
		{
			// Update line start position (where player is)
			lineRenderer.SetPosition(1, transform.position);
		}
	}

	private void FixedUpdate()
    {
        // Ground friction
		if (onGround)
		{
			rb.linearVelocityX *= groundFriction;
		}

        // Movement
		if (moveHeld)
		{
			float speed = !onGround ? moveSpeed * 0.5f : moveSpeed; // half move speed in air
																   // change dir

			if (onGround && (rb.linearVelocityX * moveInput < 0)) // when velocity * input results in negative, they are opposite
			{ // changing dir on ground
				speed *= changeDirSpeedMult;
			}

			// move when not moving max speed
            if(isUnderMaxMoveSpeed() && !isWallBlockingMoveDir())
            {
				Vector2 dir = new Vector2(moveInput, 0);
				rb.AddForce(dir * speed, ForceMode2D.Force);
			}
		}
	}

	#region INPUTS

	public void OnMove(InputValue value)
    {
        moveInput = value.Get<float>();
        if (moveInput != 0)
        {
            moveHeld = true;
        }
        else
        {
            moveHeld = false;
        }
    }

	public void OnJump(InputValue value)
	{
		if (value.isPressed)
		{
			jumpHeld = true;

			if (currentJumps > 0 || isCoyoteTimerActive())
			{ // Do jump
				DoJump(inBunnyHopWindow());
			}
			else
			{
				jumpBuffer = jumpBufferTime;
			}
		}
		else // released
		{
			jumpHeld = false;
			if (isJumpTimerActive()) // released during jump timer (released "early")
			{
				DampenUpVelocity();
			}
		}
	}

    public void OnCastHook(InputValue value)
    {
        if (value.isPressed)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(1, transform.position); // start point
            lineRenderer.SetPosition(0, mousePos); // end point

            distanceJoint.connectedAnchor = mousePos;
            distanceJoint.enabled = true;
            lineRenderer.enabled = true;
        }
        else // released
        {
            distanceJoint.enabled = false;
            lineRenderer.enabled = false;
        }
    }

	#endregion

    private void DampenUpVelocity()
    {
		if (rb.linearVelocityY > 0) // only reduce when going up
		{
			rb.linearVelocityY *= 0.5f;
		}
	}

    public void DoJump(bool bHop)
    {
        jumpTimer = StartCoroutine(JumpTimer(0.5f));
        currentJumps--;
        if (rb.linearVelocityY < 0) rb.linearVelocityY = 0;
        Vector2 force = Vector2.up * jumpForce;

		if (bHop && rb.linearVelocityX < bhopVelocityLimit)
        {
            Vector2 dir = new Vector2(moveInput, 0);
			force += dir * bhopForce;
        }

        rb.AddForce(force, ForceMode2D.Impulse);
    }

	private float landTimer = Mathf.Infinity;
	private void OnLand()
    {
        landTimer = 0; // will count up until bunny hop window is passed
        currentJumps = totalJumps;
        if (isJumpBufferActive())
        {
            DoJump(inBunnyHopWindow());
            if (!jumpHeld) DampenUpVelocity();
        }
    }


	#region COROUTINE TIMERS

	public Coroutine coyoteTimer;
    public IEnumerator CoyoteTimer(float coyoteTime)
    {
        yield return new WaitForSeconds(coyoteTime);
        coyoteTimer = null;
    }

    public bool isCoyoteTimerActive()
    {
        return coyoteTimer != null;
    }

    public Coroutine jumpTimer;
    public IEnumerator JumpTimer(float jumpTime)
    {
        yield return new WaitForSeconds(jumpTime);
        jumpTimer = null;
    }

    // Jump buffer counts in update
    private float jumpBuffer;
    public bool isJumpBufferActive()
    {
        return jumpBuffer > 0;
    }

	public bool isJumpTimerActive()
    {
        return jumpTimer != null;
    }

	#endregion

	#region CHECKS

	private bool isGrounded()
    {
        if (Physics2D.OverlapBox(groundCheckT.position, groundCheckSize, 0, groundLayer))
        {
            if (!onGround) OnLand(); // first frame returning true, so just landed
            return true;
        }
        // not on currently ground:
        if(onGround) // was grounded last frame
        {
            if(currentJumps == totalJumps) // left ground without jumping off ground
            {
                currentJumps--;
                if (coyoteTimer != null) StopCoroutine(coyoteTimer);
                coyoteTimer = StartCoroutine(CoyoteTimer(coyoteTime));
            }
        }
        return false;
    }

    private bool isTouchingRightWall()
    {
		if (Physics2D.OverlapBox(rightCheckT.position, rightCheckSize, 0, wallLayer))
		{
			return true;
		}
		return false;
    }

	private bool isTouchingLeftWall()
	{
		if (Physics2D.OverlapBox(leftCheckT.position, leftCheckSize, 0, wallLayer))
		{
			return true;
		}
		return false;
	}

	public bool isUnderMaxMoveSpeed()
	{
		return ((moveInput > 0 && rb.linearVelocityX < moveVelocityLimit) ||    // trying to move right and under positive max
			   (moveInput < 0 && rb.linearVelocityX > moveVelocityLimit * -1)); // trying to move left and above negative max
	}

	public bool isWallBlockingMoveDir()
	{
		if (moveInput < 0) // trying to move left
		{
			return isTouchingLeftWall();
		}
		else if (moveInput > 0) // trying to move right
		{
			return isTouchingRightWall();
		}
		return false; // not trying to move
	}

	private bool inBunnyHopWindow()
	{
		return (isJumpBufferActive() && jumpBuffer < bunnyHopWindow) || // jump buffer is within bhop window before landing
			   (landTimer < bunnyHopWindow); // land timer is within bhop window after landing
	}

	#endregion

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
        moveHeld = false;
        jumpHeld = false;
        reelHeld = false;
    }

	#endregion

	private void OnDrawGizmos()
    {
        // Ground Check
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawCube(groundCheckT.position, groundCheckSize);

		// Left Check
		Gizmos.color = new Color(1, 0, 0, 0.5f);
		Gizmos.DrawCube(leftCheckT.position, leftCheckSize);

		// Right Check
		Gizmos.color = new Color(0, 0, 1, 0.5f);
		Gizmos.DrawCube(rightCheckT.position, rightCheckSize);
	}
}
