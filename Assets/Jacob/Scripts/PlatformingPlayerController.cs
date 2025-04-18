using GameEvents;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformingPlayerController : Interactor
{
	#region VARIABLES
	[Header("References")]
	[SerializeField] private Rigidbody2D rb;
	[SerializeField] private DistanceJoint2D distanceJoint;
	[SerializeField] private LineRenderer lineRenderer;
	[SerializeField] private Rigidbody2D hookRb;
	[SerializeField] private Collider2D hookCol;
	[SerializeField] private Transform spriteT;
	[HideInInspector] public Transform interactedWaterT;
	private Camera cam;

	[Header("Stats")]
	[SerializeField] private float moveSpeed = 5f;

	[SerializeField] private float moveVelocityLimit = 10f;
	[SerializeField] private float bhopVelocityLimit = 20f;
	//[SerializeField] private float reelVelocityLimit = 40f;

	[SerializeField] private float jumpForce = 20f;

	[SerializeField] private float bhopForce = 20f;

	[SerializeField] private float maxLineLength = 10f;
	public float MaxLineLength { get { return maxLineLength; } }

	[SerializeField, Tooltip("Force at which the hook gets launched from player when casting")] 
	private float castForce = 10f;

	[SerializeField] private float reelSpeed = 10f;

	[SerializeField, Tooltip("When changing direction, the players curent speed is used for counteraction, this variable multiplies against the current player speed")]
	private float changeDirSpeedMult = 0.5f;

	[SerializeField, Tooltip("Linear X Velocity of player will get multiplied by this value while on ground (1 = no friction)"), Range(0.8f, 1)]
	private float groundFriction = 0.98f;

	[SerializeField, Tooltip("Time jump input will be stored so the player will jump again once then hit the ground")]
	private float jumpBufferTime = 0.1f;

	[SerializeField] private float coyoteTime = 0.1f;

	[SerializeField] private int totalJumps = 1;
	private int currentJumps;

	[SerializeField] private int totalWallJumps = 3;
	private int currentWallJumps;

	[SerializeField, Tooltip("Time before and after landing where player will successfully bunny hop")]
	private float bunnyHopWindow = 0.05f;

	[SerializeField, Tooltip("When the hook is returning, once it reaches this distance from the player, it will become inactive")]
	private float completeReturnDistance = 0.2f;

	[SerializeField, Tooltip("When the hook is returning, it will apply this much linear dampening so it doesn't keep flying past the player")]
	private float hookReturnFriction = 0.95f;

	[SerializeField, Tooltip("Multiplies reel speed by this value to get force used when hook is returning")]
	private float hookReturnSpeedMod = 2f;

	[Header("Ground Check")]
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundCheckT;
	[SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
	private bool onGround;
	private bool inWater;

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
	private bool slackHeld = false;
	#endregion

	#region START & UPDATES

	public override void Start()
	{
		base.Start();

		cam = Camera.main;

		currentJumps = totalJumps;
		currentWallJumps = totalWallJumps;

		ChangeRodState(RodState.INACTIVE);
	}

	public void Update()
	{
		onGround = isGrounded();


		// Timers
		if (jumpBuffer >= 0)
		{
			jumpBuffer -= Time.deltaTime;
		}
		if (landTimer <= bunnyHopWindow)
		{
			landTimer += Time.deltaTime;
		}

		// Process Rod State (Visuals)
		switch (currentRodState)
		{
			case RodState.CASTING:
				UpdateLineRendererEnds();
				break;
			case RodState.RETURNING:
				UpdateLineRendererEnds();
				break;
			case RodState.HOOKED:
				UpdateLineRendererEnds(true, false);
				break;
			case RodState.FISHCASTING:
				UpdateLineRendererEnds();
				break;
			default:
				break;
		}
	}

	private void FixedUpdate()
	{
		// Ground friction
		if (onGround && currentRodState != RodState.HOOKED)
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
			if (isUnderMaxMoveSpeed() && !isWallBlockingMoveDir())
			{
				Vector2 dir = new Vector2(moveInput, 0);
				rb.AddForce(dir * speed, ForceMode2D.Force);
			}
		}

		// Process Rod State
		switch (currentRodState)
		{
			case RodState.CASTING:
				if (Vector2.Distance(hookRb.transform.position, transform.position) >= maxLineLength)
				{ // Reached max distance before hitting something
					ChangeRodState(RodState.RETURNING);
				}

				break;
			case RodState.RETURNING:
				hookRb.linearVelocity *= hookReturnFriction; // dampen so it doesnt constantly fly past player trying to return
				Vector2 dir = (transform.position - hookRb.transform.position).normalized;
				hookRb.AddForce(dir * reelSpeed * hookReturnSpeedMod, ForceMode2D.Force);

				if (Vector2.Distance(hookRb.gameObject.transform.position, transform.position) <= completeReturnDistance)
				{
					ChangeRodState(RodState.INACTIVE);
				}

				break;
			case RodState.HOOKED:
				// Reeling
				if (reelHeld)
				{
					if (slackHeld) // Give Slack
					{
						if (distanceJoint.distance < maxLineLength)
						{
							distanceJoint.distance += Time.deltaTime * reelSpeed;
							if (Vector2.Distance(transform.position, hookRb.transform.position) < 0.5)
							{
								dir = (transform.position - hookRb.transform.position).normalized;
								rb.AddForce(dir * 50, ForceMode2D.Force);
							}
						}
					}
					else // Reel In
					{
						//old
						//distanceJoint.distance -= Time.deltaTime * reelSpeed;

						//new
						dir = (hookRb.transform.position - transform.position).normalized;
						rb.AddForce(dir * reelSpeed, ForceMode2D.Force);

						float dist = Vector2.Distance(transform.position, hookRb.transform.position);
						if (distanceJoint.distance > dist) distanceJoint.distance = dist;
					}
				}

				break;
			default:
				break;
		}
	}

	#endregion

	#region INPUTS

	public void OnMove(InputValue value)
	{
		moveInput = value.Get<float>();
		if (moveInput != 0)
		{
			moveHeld = true;
			if((moveInput < 0 && spriteT.localScale.x > 0) || (moveInput > 0 && spriteT.localScale.x < 0))
			{
				spriteT.localScale = new Vector2(spriteT.localScale.x * -1, spriteT.localScale.y);
			}

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

			if((isTouchingLeftWall() ^ isTouchingRightWall()) && !onGround && !inWater)
			{
				TryWallJump();
			}
			else 
			{
				TryJump();
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
			if (currentRodState == RodState.INACTIVE)
			{
				ChangeRodState(RodState.CASTING);
			}
		}
		else // released
		{
			if (currentRodState != RodState.INACTIVE && currentRodState != RodState.RETURNING)
			{
				ChangeRodState(RodState.RETURNING);
			}
		}
	}

	public void OnSlack(InputValue value)
	{
		if (value.isPressed)
		{
			slackHeld = true;
		}
		else // released
		{
			slackHeld = false;
		}
	}

	public void OnReelHook(InputValue value)
	{
		if (value.isPressed)
		{
			reelHeld = true;
		}
		else // released
		{
			reelHeld = false;
		}
	}

	public void OnInteract()
	{
		TryInteract();
	}

	public void OnFishCast(Transform waterT)
	{
		interactedWaterT = waterT;
		ChangeRodState(RodState.FISHCASTING);
	}

	public void OnDoneFishing()
	{
		ChangeRodState(RodState.RETURNING);
	}

	#endregion

	#region FISHING ROD GRAPPLING HOOK
	public enum RodState
	{
		INACTIVE, CASTING, RETURNING, HOOKED, FISHCASTING
	}
	public RodState currentRodState = RodState.INACTIVE;

	public void ChangeRodState(RodState state)
	{
		// SET NEW ROD STATE
		currentRodState = state;

		// ON ENTER
		switch (currentRodState) // new rod state
		{
			case RodState.INACTIVE:
				hookCol.isTrigger = true;
				hookRb.transform.position = transform.position;
				hookRb.gameObject.SetActive(false);
				distanceJoint.enabled = false;
				lineRenderer.enabled = false;
				break;

			case RodState.CASTING:
				hookCol.isTrigger = true;
				hookRb.bodyType = RigidbodyType2D.Dynamic;
				hookRb.gameObject.SetActive(true);
				
				lineRenderer.enabled = true;

				Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
				Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
				hookRb.AddForce(dir * castForce);
				break;

			case RodState.RETURNING:
				hookCol.isTrigger = true;
				distanceJoint.enabled = false;
				hookRb.transform.parent = transform;
				hookRb.bodyType = RigidbodyType2D.Dynamic;
				break;

			case RodState.HOOKED:
				currentWallJumps = totalWallJumps;
				distanceJoint.connectedAnchor = hookRb.gameObject.transform.position;
				distanceJoint.enabled = true;

				hookRb.bodyType = RigidbodyType2D.Kinematic;
				hookRb.linearVelocity = Vector2.zero;

				UpdateLineRendererEnds();

				break;

			case RodState.FISHCASTING:
				hookRb.bodyType = RigidbodyType2D.Dynamic;
				hookCol.isTrigger = false;
				hookRb.gameObject.SetActive(true);

				lineRenderer.enabled = true;

				dir = spriteT.localScale.x * Vector2.right + Vector2.up;
				dir.Normalize();

				float dist = Vector2.Distance(transform.position, interactedWaterT.position);

				hookRb.AddForce(dir * 40 * dist);


				break;
		}
	}

	private void UpdateLineRendererEnds(bool playerEnd = true, bool hookEnd = true)
	{
		if (playerEnd) lineRenderer.SetPosition(1, transform.position);
		if (hookEnd) lineRenderer.SetPosition(0, hookRb.position);
	}


	#endregion

	#region JUMPING & LANDING

	// HANDLE EARLY RELEASE FOR JUMP
	private void DampenUpVelocity()
	{
		if (rb.linearVelocityY > 0) // only reduce when going up
		{
			rb.linearVelocityY *= 0.5f;
		}
	}


	// REGULAR JUMP
	public void TryJump()
	{
		if (currentJumps > 0 || isCoyoteTimerActive())
		{ // Do jump
			DoJump(inBunnyHopWindow());
		}
		else
		{ // Start jump buffer
			jumpBuffer = jumpBufferTime;
		}
	}

	public void DoJump(bool bHop)
	{
		currentJumps--;
		jumpTimer = StartCoroutine(JumpTimer(0.5f));
		if (rb.linearVelocityY < 0) rb.linearVelocityY = 0;
		Vector2 force = Vector2.up * jumpForce * (inWater ? 0.5f : 1);

		if (bHop && rb.linearVelocityX < bhopVelocityLimit)
		{
			Vector2 dir = new Vector2(moveInput, 0);
			force += dir * bhopForce;
		}

		rb.AddForce(force, ForceMode2D.Impulse);
		onGround = false;
	}
	//-----------


	// WALL JUMPING
	public void TryWallJump()
	{
		if (currentWallJumps > 0)
		{
			DoWallJump();
		}
	}

	public void DoWallJump()
	{
		currentWallJumps--;

		Vector2 dir = Vector2.up;

		if(isTouchingLeftWall())
		{ // DO JUMP UP AND RIGHT
			dir += Vector2.right;
		}
		else // touching right wall
		{ // DO JUMP UP AND LEFT
			dir += Vector2.left;
		}

		rb.AddForce(dir * jumpForce, ForceMode2D.Impulse);
	}
	//--------------


	// LANDING
	private float landTimer = Mathf.Infinity;
	private void OnLand()
	{
		landTimer = 0; // will count up until bunny hop window is passed
		currentJumps = totalJumps;
		currentWallJumps = totalWallJumps;
		if (isJumpBufferActive())
		{
			DoJump(inBunnyHopWindow());
			if (!jumpHeld) DampenUpVelocity();
		}
	}

	#endregion

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

	#region CONDITION CHECKS
	public void UpdateWater(bool newState)
	{
		inWater = newState;
	}
	private bool isGrounded()
	{
		if (Physics2D.OverlapBox(groundCheckT.position, groundCheckSize, 0, groundLayer) || inWater)
		{
			if (!onGround) OnLand(); // first frame returning true, so just landed
			return true;
		}
		// not on currently ground:
		if (onGround) // was grounded last frame
		{
			if (currentJumps == totalJumps) // left ground without jumping off ground
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
		slackHeld = false;
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
