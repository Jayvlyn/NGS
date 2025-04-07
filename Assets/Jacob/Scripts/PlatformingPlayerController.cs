using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformingPlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveVelocityLimit = 10f;
    [SerializeField] private float jumpStrength = 50f;
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

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;
    private bool onGround;

    // Inputs
    private float moveInput; // left-right 1D axis
    // Handle Held Inputs
    private bool moveHeld = false;
    private bool jumpHeld = false;
    private bool reelHeld = false;

	private void Start()
	{
        currentJumps = totalJumps;
	}

	private void Update()
    {
        onGround = isGrounded();
    }

	private void FixedUpdate()
	{
		if (onGround)
		{
			rb.linearVelocityX *= groundFriction;
		}

		if (moveHeld)
		{
			float speed = onGround ? moveSpeed * 0.5f : moveSpeed; // half move speed in air
																   // change dir

			if (onGround && ((rb.linearVelocityX > 0 && moveInput < 0) || (rb.linearVelocityX < 0 && moveInput > 0)))
			{ // changing dir on ground
				speed *= rb.linearVelocityX * changeDirSpeedMult;
			}

			// move when not moving max speed
			if ((moveInput > 0 && rb.linearVelocityX < moveVelocityLimit) || // trying to move right and under positive max
				(moveInput < 0 && rb.linearVelocityX > moveVelocityLimit * -1)) // trying to move left and above negative max
																				// able to move in either direction when not moving in max speed in that direction
			{
				Vector2 dir = new Vector2(moveInput, 0);
				rb.AddForce(dir * speed, ForceMode2D.Force);
			}
		}
	}

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
        if(currentJumps > 0 || isCoyoteTimerActive())
        { // Do jump
            DoJump();
        }
        else
        {
            if (jumpBuffer != null) StopCoroutine(jumpBuffer);
            jumpBuffer = StartCoroutine(JumpBuffer(jumpBufferTime));
        }
    }

    public void DoJump()
    {
        currentJumps--;
        if (rb.linearVelocityY < 0) rb.linearVelocityY = 0;
        rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
    }

    public Coroutine jumpBuffer;
    public IEnumerator JumpBuffer(float bufferTime)
    {
        yield return new WaitForSeconds(bufferTime);
        jumpBuffer = null;
    }

    public bool isJumpBufferActive()
    {
        return jumpBuffer != null;
    }


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


    private bool isGrounded()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
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
    
    private void OnLand()
    {
        if (isJumpBufferActive()) DoJump();
        currentJumps = totalJumps;
    }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundCheckPos.position, groundCheckSize);
    }
}
