using GameEvents;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
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
	[SerializeField] private CinemachineTargetGroup ctg;
	[HideInInspector] public Transform interactedWaterT;
	[SerializeField] private Animator animator;
	[SerializeField] VoidEvent onInventory;
	private Camera cam;

	[Header("Stats")]
	[SerializeField] private float moveSpeed = 5f;

	[SerializeField] private float moveVelocityLimit = 10f;
	[SerializeField] private float bhopVelocityLimit = 20f;
	//[SerializeField] private float reelVelocityLimit = 40f;

	[SerializeField] private float jumpForce = 20f;
	[SerializeField] private float airControlMod = 0.4f;

	[SerializeField] private float bhopForce = 20f;

	[SerializeField] private float maxLineLength = 10f;
	public float MaxLineLength { get { return maxLineLength; } }

	[SerializeField, Tooltip("How long it takes for hook to reach point it was casted to for grappling")] 
	private float castTime = 0.5f;

	[SerializeField] private float reelSpeed = 10f;

	[SerializeField, Tooltip("When changing direction, the players curent speed is used for counteraction, this variable multiplies against the current player speed")]
	private float changeDirSpeedMult = 0.5f;

	[SerializeField, Tooltip("Linear X Velocity of player will get multiplied by this value while on the ground (1 = no friction)"), Range(0.8f, 1)]
	private float groundFriction = 0.98f;

	[SerializeField, Tooltip("Linear Velocity of player will get multiplied by this value while off the ground (1 = no friction)"), Range(0.8f, 1)]
	private float airFriction = 0.98f;

	[SerializeField, Tooltip("Time jump input will be stored so the player will jump again once then hit the ground")]
	private float jumpBufferTime = 0.1f;

	[SerializeField]
	private float jumpTime = 0.5f;
	private float startingGravity;

	[SerializeField, Tooltip("Higher = Player will lose wall sticking faster")]
	private float wallStickGravityIncreaseMult = 10f;

	[SerializeField] private float coyoteTime = 0.1f;

	[SerializeField] private int totalJumps = 1;
	private int currentJumps;

	[SerializeField] private int totalWallJumps = 3;
	private int currentWallJumps;
	[SerializeField] private float wallJumpUpwardsInfluence = 1;
	[SerializeField] private float wallJumpSidewaysInfluence = 1;

	[SerializeField, Tooltip("Time before and after landing where player will successfully bunny hop")]
	private float bunnyHopWindow = 0.05f;

	[SerializeField, Tooltip("When the hook is returning, once it reaches this distance from the player, it will become inactive")]
	private float completeReturnDistance = 0.2f;

	[SerializeField, Tooltip("When the hook is returning, it will apply this much linear dampening so it doesn't keep flying past the player")]
	private float hookReturnFriction = 0.95f;

	[SerializeField, Tooltip("Multiplies reel speed by this value to get force used when hook is returning")]
	private float hookReturnSpeedMod = 2f;

	[SerializeField, Tooltip("How big a radius will be created around the mouse when looking for a valid grapple point")]
	private float aimAssistRadius = 2;

	[SerializeField, Tooltip("Then the player gets this close to the hook while in hooked state, it will detach")]
	private float detachDistance = 0.5f;

	[SerializeField] private LayerMask grappleableLayer;

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

	[Header("Cast Visuals")]
	[SerializeField] private AnimationCurve fishCastCurve;
	public AnimationCurve grappleCastCurveBase;
	private AnimationCurve grappleCastCurve;
	private Vector2 waterMidpoint;

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
		//Time.timeScale = 0.2f;
		base.Start();

		if(BossFishController.caughtBoss) Inventory.Instance.AddFish(BossFishController.bossFish);
		BossFishController.caughtBoss = false;

		cam = Camera.main;

		currentJumps = totalJumps;
		currentWallJumps = totalWallJumps;
		startingGravity = rb.gravityScale;

		ChangeRodState(RodState.INACTIVE);
		ChangeMoveState(MoveState.IDLE);
	}

	public void Update()
	{
		onGround = isGrounded();

		ProcessUpdateTimers();

		ProcessRodStateUpdate();

		ProcessMoveStateUpdate();

		if (currentMoveState == MoveState.IDLE)
		{
			if (ctg.Targets[1].Weight > 1)
			{
				ctg.Targets[1].Weight -= Time.deltaTime * 10;
				if (ctg.Targets[1].Weight < 1) ctg.Targets[1].Weight = 1;
			}
		}
		else
		{
			if (ctg.Targets[1].Weight < 3)
			{
				ctg.Targets[1].Weight += Time.deltaTime * 10;
				if (ctg.Targets[1].Weight > 3) ctg.Targets[1].Weight = 3;
			}
		}
	}


	private void FixedUpdate()
	{
		DoGroundFriction();

		ProcessMoveInput();

		ProcessRodStateFixedUpdate();
	}

	#endregion

	#region INPUTS

	public void OnMove(InputValue value)
	{
		moveInput = value.Get<float>();
		if (moveInput != 0)
		{
			moveHeld = true;
			if (currentMoveState == MoveState.WALL_STICKING) ChangeMoveState(MoveState.FALLING);
			else if (currentMoveState == MoveState.IDLE) ChangeMoveState(MoveState.RUNNING);
			else if (currentMoveState == MoveState.GROUND_HOOKED) ChangeMoveState(MoveState.GROUND_HOOKED_WALKING);
			else if (currentMoveState == MoveState.GROUND_REELING) ChangeMoveState(MoveState.WALKING_REELING);

			if(moveInput * spriteT.localScale.x < 0)
			{
				FlipX();
			}
		}
		else
		{
			if(currentMoveState == MoveState.RUNNING) ChangeMoveState(MoveState.IDLE);

			moveHeld = false;
		}
	}

	public void OnJump(InputValue value)
	{
		if (value.isPressed)
		{
			jumpHeld = true;

			if(currentMoveState == MoveState.WALL_STICKING)
			{
				DoWallJump();
				return;
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

	public void OnInventory(InputValue value)
	{
		if (value.isPressed)
		{
			onInventory.Raise();
		}
	}

	public void OnInteract()
	{
		TryInteract();
	}

	public void OnFishCast(Transform waterT)
	{
		interactedWaterT = waterT;

		Collider2D waterCol = interactedWaterT.parent.GetComponent<Collider2D>();
		waterMidpoint = waterCol.bounds.center;

		Vector3 waterDir = (waterMidpoint - (Vector2)transform.position).normalized;
		if(spriteT.localScale.x * waterDir.x < 0) // facing away from water
		{
			FlipX();
		}
		ChangeRodState(RodState.FISHCASTING);
	}

	/// <summary>
	/// Call in fixed update
	/// </summary>
	private void ProcessMoveInput()
	{
		// Movement
		if (moveHeld)
		{
			float speed = !onGround ? moveSpeed * airControlMod : moveSpeed; // half move speed in air

			if (currentRodState == RodState.HOOKED) speed = speed * 0.7f;


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

	#region MOVE STATE
	public enum MoveState
	{
		IDLE, RUNNING, JUMPING, FALLING, WALLJUMPING, SWIMMING, AIR_CASTING, GROUND_CASTING, AIR_REELING, GROUND_REELING, GROUND_HOOKED, AIR_HOOKED, GROUND_HOOKED_WALKING, WALKING_REELING, WALL_STICKING
	}
	public MoveState currentMoveState = MoveState.IDLE;

	private void ChangeMoveState(MoveState state)
	{
		if (currentMoveState == MoveState.WALL_STICKING)
		{ // exiting wall stick
			rb.gravityScale = startingGravity;
		}

		//Debug.Log($"Changing from {currentMoveState} to {state}");
		currentMoveState = state;
		switch (state)
		{
			case MoveState.IDLE:
				SetTrigger("ToIdle");
				break;
			case MoveState.RUNNING:
				SetTrigger("ToRun");
				break;
			case MoveState.JUMPING:
				SetTrigger("ToJump");
				break;
			case MoveState.FALLING:
				SetTrigger("ToFall");
				break;
			case MoveState.WALLJUMPING:
				SetTrigger("ToWallJump");
				break;
			case MoveState.SWIMMING:
				SetTrigger("ToSwim");
				break;
			case MoveState.AIR_CASTING:
				SetTrigger("ToAirCast");
				break;
			case MoveState.GROUND_CASTING:
				SetTrigger("ToGroundCast");
				break;
			case MoveState.AIR_REELING:
				SetTrigger("ToAirReel");
				break;
			case MoveState.GROUND_REELING:
				SetTrigger("ToGroundReel");
				break;
			case MoveState.GROUND_HOOKED:
				SetTrigger("ToGroundHooked");
				break;
			case MoveState.AIR_HOOKED:
				SetTrigger("ToAirHooked");
				break;
			case MoveState.GROUND_HOOKED_WALKING:
				SetTrigger("ToGrappledWalk");
				break;
			case MoveState.WALKING_REELING:
				SetTrigger("ToReelWalk");
				break;
			case MoveState.WALL_STICKING:
				FlipX();
				rb.linearVelocityY = 0;
				rb.gravityScale = 0;

				SetTrigger("ToWallStick");
				break;
		}
	}

	bool touchingLeft;
	bool touchingRight;
	private void ProcessMoveStateUpdate()
	{
		switch (currentMoveState)
		{
			case MoveState.IDLE:
				if (!isGrounded() && isFalling()) ChangeMoveState(MoveState.FALLING);
				
				break;
			case MoveState.RUNNING:
				if (!isGrounded() && isFalling()) ChangeMoveState(MoveState.FALLING);
				if (isFacingLeft() && moveInput > 0 || isFacingRight() && moveInput < 0) FlipX();
				break;
			case MoveState.JUMPING:
				if (isFalling()) ChangeMoveState(MoveState.FALLING);
				isTouchingLeftWall();
				if(jumpTime - jumpTimer > 0.3f)
				{
					touchingLeft = isTouchingLeftWall();
					touchingRight = isTouchingRightWall();

					if (!onGround && currentWallJumps > 0)
					{
						if(touchingLeft && isFacingLeft() || touchingRight && isFacingRight()) ChangeMoveState(MoveState.WALL_STICKING);
					}
					
				}
				break;
			case MoveState.FALLING:
				if (isFacingLeft() && moveInput > 0 || isFacingRight() && moveInput < 0) FlipX();

				touchingLeft = isTouchingLeftWall();
				touchingRight = isTouchingRightWall();

				if (!onGround && currentWallJumps > 0)
				{
					if (touchingLeft && isFacingLeft() || touchingRight && isFacingRight()) ChangeMoveState(MoveState.WALL_STICKING);
				}
				break;
			case MoveState.WALLJUMPING:
				if (isFalling()) ChangeMoveState(MoveState.FALLING);
				touchingLeft = isTouchingLeftWall();
				touchingRight = isTouchingRightWall();

				if (!onGround && currentWallJumps > 0)
				{
					if (touchingLeft && isFacingLeft() || touchingRight && isFacingRight()) ChangeMoveState(MoveState.WALL_STICKING);
				}
				break;
			case MoveState.SWIMMING:
				if(!inWater) ChangeMoveState(MoveState.IDLE);
				break;
			case MoveState.AIR_CASTING:
				if(currentRodState == RodState.RETURNING) ChangeMoveState(MoveState.AIR_REELING);
				break;
			case MoveState.GROUND_CASTING:
				if(currentRodState == RodState.RETURNING) ChangeMoveState(MoveState.GROUND_REELING);
				break;
			case MoveState.AIR_REELING:
				if (!reelHeld && currentRodState == RodState.HOOKED) ChangeMoveState(MoveState.AIR_HOOKED);
				else if (currentRodState == RodState.INACTIVE)
				{
					if(isFalling()) ChangeMoveState(MoveState.FALLING);
				}
				break;
			case MoveState.GROUND_REELING:
				if (!reelHeld && currentRodState == RodState.HOOKED) ChangeMoveState(MoveState.GROUND_HOOKED);
				else if (currentRodState == RodState.INACTIVE) ChangeMoveState(MoveState.IDLE);
				break;
			case MoveState.GROUND_HOOKED:
				if (!onGround) ChangeMoveState(MoveState.AIR_HOOKED);
				if (currentRodState == RodState.RETURNING)
				{
					ChangeMoveState(MoveState.GROUND_REELING);
				}
				break;
			case MoveState.AIR_HOOKED:
				if(currentRodState == RodState.RETURNING)
				{
					if (isFalling()) ChangeMoveState(MoveState.FALLING);
				}
				//LookAtHook();
				break;
			case MoveState.GROUND_HOOKED_WALKING:
				if (!moveHeld) ChangeMoveState(MoveState.GROUND_HOOKED);
				if (!onGround) ChangeMoveState(MoveState.AIR_HOOKED);
				break;
			case MoveState.WALKING_REELING:
				if (!onGround) ChangeMoveState(MoveState.AIR_HOOKED);
				break;
			case MoveState.WALL_STICKING:
				if(isFacingRight() && !isTouchingLeftWall() || isFacingLeft() && !isTouchingRightWall())
				{
					ChangeMoveState(MoveState.FALLING);
				}

				if (rb.gravityScale < startingGravity)
				{
					rb.gravityScale += Time.deltaTime + rb.gravityScale;
					rb.gravityScale *= wallStickGravityIncreaseMult;
				}
				else if (rb.gravityScale >= startingGravity)
				{
					rb.gravityScale = startingGravity;
					ChangeMoveState(MoveState.FALLING);
				}

				break;
		}
	}

	private void DoGroundFriction()
	{
		// Ground friction
		if (onGround)
		{
			if (currentRodState != RodState.HOOKED)
			{
				rb.linearVelocityX *= groundFriction;
			}
		}
		else
		{
			rb.linearVelocity *= airFriction;
		}
	}

	private void FlipX()
	{
		spriteT.localScale = new Vector2(spriteT.localScale.x * -1, spriteT.localScale.y);
	}

	private void LookAtHook()
	{
		Vector2 direction = ((Vector2)hookRb.transform.position - (Vector2)transform.position).normalized;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0f, 0f, angle);
	}
	#endregion

	#region FISHING ROD
	public enum RodState
	{
		INACTIVE, CASTING, RETURNING, HOOKED, FISHCASTING
	}
	public RodState currentRodState = RodState.INACTIVE;

	public void ChangeRodState(RodState state)
	{
		if(currentRodState == RodState.CASTING)
		{
			if (castHookToPoint != null) StopCoroutine(castHookToPoint);
		}

		// SET NEW ROD STATE
		currentRodState = state;

		// ON ENTER
		switch (currentRodState) // new rod state
		{
			case RodState.INACTIVE:
				OnEnterInactiveState();
				if(currentMoveState == MoveState.WALKING_REELING) // returning
				{
					ChangeMoveState(MoveState.RUNNING);
				}
				break;

			case RodState.CASTING:
				OnEnterCastingState();
				if(onGround) ChangeMoveState(MoveState.GROUND_CASTING);
				else ChangeMoveState(MoveState.AIR_CASTING);

				break;

			case RodState.RETURNING:
				OnEnterReturningState();
				if (onGround) ChangeMoveState(MoveState.GROUND_REELING);
				else ChangeMoveState(MoveState.AIR_REELING);

				break;

			case RodState.HOOKED:
				OnEnterHookedState();
				if (onGround) ChangeMoveState(MoveState.GROUND_HOOKED);
				else ChangeMoveState(MoveState.AIR_HOOKED);
				break;

			case RodState.FISHCASTING:
				OnEnterFishCastingState();
				if (onGround) ChangeMoveState(MoveState.GROUND_CASTING);
				else ChangeMoveState(MoveState.AIR_CASTING);

				break;
		}
	}

	private void UpdateLineRendererEnds(bool playerEnd = true, bool hookEnd = true)
	{
		if (playerEnd) lineRenderer.SetPosition(1, transform.position);
		if (hookEnd) lineRenderer.SetPosition(0, hookRb.position);
	}

	private void ProcessRodStateUpdate()
	{
		switch (currentRodState)
		{
			case RodState.CASTING:
				UpdateLineRendererEnds();
				break;
			case RodState.RETURNING:
				UpdateLineRendererEnds();
				break;
			case RodState.HOOKED:
				UpdateLineRendererEnds();
				break;
			case RodState.FISHCASTING:
				UpdateLineRendererEnds();
				break;
			default:
				break;
		}
	}

	private void ProcessRodStateFixedUpdate()
	{
		// Process Rod State
		switch (currentRodState)
		{
			case RodState.RETURNING:
				//Debug.Log(hookRb.transform.position);
				hookRb.linearVelocity *= hookReturnFriction; // dampen so it doesnt constantly fly past player trying to return
				Vector2 dir = (transform.position - hookRb.transform.position).normalized;
				hookRb.AddForce(dir * reelSpeed * hookReturnSpeedMod, ForceMode2D.Force);

				if (Vector2.Distance(hookRb.gameObject.transform.position, transform.position) <= completeReturnDistance)
				{
					ChangeRodState(RodState.INACTIVE);
				}

				break;
			case RodState.HOOKED:
				float dist = Vector2.Distance(transform.position, hookRb.transform.position);
				if (distanceJoint.distance > dist) distanceJoint.distance = dist;
				if (distanceJoint.distance > maxLineLength)
				{
					distanceJoint.distance -= Time.deltaTime * reelSpeed * 0.05f;
				}

				if (dist < detachDistance)
				{
					ChangeRodState(RodState.RETURNING);
				}
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
						if(currentMoveState != MoveState.AIR_REELING || currentMoveState != MoveState.GROUND_REELING)
						{
							if(onGround) ChangeMoveState(MoveState.GROUND_REELING);
							else ChangeMoveState(MoveState.AIR_REELING);
						}

						distanceJoint.distance -= Time.deltaTime * reelSpeed * 0.05f;
						dir = (hookRb.transform.position - transform.position).normalized;
						rb.AddForce(dir * reelSpeed, ForceMode2D.Force);
					}
				}

				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Method responsible for starting coroutine to do visual casting for grappling hook
	/// </summary>
	/// <param name="point">Point to cast to</param>
	/// <param name="willHook">Pass as true if it should change to hooked state at the end</param>
	private IEnumerator CastHookToPoint(Vector2 point, bool willHook)
	{
		float t = 0;
		hookRb.transform.position = transform.position;
		Vector2 initialPosition = transform.position;
		while (t < castTime)
		{
			Vector2 currentPos;
			currentPos.x = Mathf.Lerp(initialPosition.x, point.x, t / castTime);
			currentPos.y = initialPosition.y + grappleCastCurve.Evaluate(t / castTime);
			hookRb.transform.position = currentPos;

			//float modifier = 1;

			//if(t/castTime < 0.5) modifier -= t / castTime;
			//else modifier += t / castTime;

			t += Time.deltaTime;//* modifier;
			yield return null;
		}
		hookRb.transform.position = point;

		if(willHook)ChangeRodState(RodState.HOOKED);
		else ChangeRodState(RodState.RETURNING);
		
	}
	private Coroutine castHookToPoint;

	private void OnEnterCastingState()
	{
		hookCol.isTrigger = true;
		hookRb.bodyType = RigidbodyType2D.Kinematic;
		hookRb.gameObject.SetActive(true);

		lineRenderer.enabled = true;

		Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
		Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
		if (dir.x * spriteT.localScale.x < 0f)
		{
			FlipX(); // flip to look in casting direction
		}

		Vector2 overlapPos = mousePos;
		float distanceToMouse = Vector2.Distance(transform.position, mousePos);
		if(distanceToMouse > maxLineLength)
		{
			overlapPos = (Vector2)transform.position + dir * maxLineLength;
		}

		Collider2D hit = Physics2D.OverlapCircle(overlapPos, aimAssistRadius, grappleableLayer);
		DebugDrawCircle(overlapPos, aimAssistRadius, Color.green, 1.5f);

		Vector2 hookPos = Vector2.zero;
		if (hit != null)
		{
			hookPos = hit.ClosestPoint(overlapPos);
			hookRb.transform.position = hookPos;
			hookRb.transform.parent = null;

			DefineCurveKeys(hookPos);

			if(castHookToPoint != null) StopCoroutine(castHookToPoint);
			castHookToPoint = StartCoroutine(CastHookToPoint(hookPos, true));
		}
		else
		{
			hookPos = (Vector2)transform.position + dir * MaxLineLength;
			hookRb.transform.position = hookPos;

			DefineCurveKeys(hookPos);

			if (castHookToPoint != null) StopCoroutine(castHookToPoint);
			castHookToPoint = StartCoroutine(CastHookToPoint(hookPos, false));
		}
		//hookRb.AddForce(dir * castTime);
	}

	private void DefineCurveKeys(Vector2 hookPos)
	{
		float yDiff = hookPos.y - transform.position.y;
		float distance = Vector2.Distance(hookPos, transform.position);

		castTime = distance * 0.2f;

		grappleCastCurve = new AnimationCurve();
		grappleCastCurve.CopyFrom(grappleCastCurveBase);

		// if grapple point is higher than player, hook will end higher than it started
		Keyframe[] keys = grappleCastCurve.keys;
		keys[1].value = distance * 0.5f; // mid point
		keys[2].value = yDiff; // end point

		if (keys[1].value <= yDiff)
		{
			// Rebuild curve without the midpoint
			List<Keyframe> filteredKeys = new List<Keyframe> { keys[0], keys[2] };
			grappleCastCurve.keys = filteredKeys.ToArray();
		}
		else
		{
			grappleCastCurve.keys = keys;
		}
	}

	private void OnEnterHookedState()
	{
		currentWallJumps = totalWallJumps;
		distanceJoint.connectedAnchor = hookRb.gameObject.transform.position;
		distanceJoint.enabled = true;

		hookRb.bodyType = RigidbodyType2D.Kinematic;
		hookRb.linearVelocity = Vector2.zero;

		UpdateLineRendererEnds();
	}

	private void OnEnterReturningState()
	{
		hookCol.isTrigger = true;
		distanceJoint.enabled = false;
		hookRb.bodyType = RigidbodyType2D.Dynamic;
		hookRb.transform.parent = transform;
	}

	private void OnEnterInactiveState()
	{
		hookCol.isTrigger = true;
		hookRb.transform.position = transform.position;
		hookRb.gameObject.SetActive(false);
		distanceJoint.enabled = false;
		lineRenderer.enabled = false;
	}

	private void OnEnterFishCastingState()
	{
		hookCol.isTrigger = false;
		hookRb.gameObject.SetActive(true);

		lineRenderer.enabled = true;

		StartCoroutine(VisualFishCast(2));
	}

	public IEnumerator VisualFishCast(float speed = 1)
	{
		float dist = waterMidpoint.x - transform.position.x;
		hookRb.bodyType = RigidbodyType2D.Kinematic;
		float t = 0;
		while (t < dist)
		{
			Vector2 pos;
			pos.x = (t / dist) * Mathf.Sign(spriteT.localScale.x);
			pos.y = fishCastCurve.Evaluate(t / dist);
			hookRb.transform.position = (Vector2)this.transform.position + pos;

			t += Time.deltaTime * speed;
			yield return null;
		}

		hookRb.bodyType = RigidbodyType2D.Dynamic;
	}

	public void OnDoneFishing()
	{
		ChangeRodState(RodState.RETURNING);
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
		if (currentRodState != RodState.INACTIVE) ChangeRodState(RodState.RETURNING);
		ChangeMoveState(MoveState.JUMPING);
		currentJumps--;
		jumpTimer = jumpTime;
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

	public void DoWallJump()
	{
		if (currentRodState != RodState.INACTIVE) ChangeRodState(RodState.RETURNING);
		ChangeMoveState(MoveState.WALLJUMPING);
		currentWallJumps--;

		jumpTimer = jumpTime;
		
		Vector2 dir = Vector2.up * wallJumpUpwardsInfluence;

		if(isTouchingLeftWall())
		{ // DO JUMP UP AND RIGHT
			dir += Vector2.right * wallJumpSidewaysInfluence;
		}
		else // touching right wall
		{ // DO JUMP UP AND LEFT
			dir += Vector2.left * wallJumpSidewaysInfluence;
		}

		rb.AddForce(dir * jumpForce, ForceMode2D.Impulse);
	}
	//--------------


	// LANDING
	private float landTimer = Mathf.Infinity;
	private void OnLand()
	{
		if (currentMoveState == MoveState.FALLING || currentMoveState == MoveState.JUMPING || currentMoveState == MoveState.WALLJUMPING || currentMoveState == MoveState.WALL_STICKING)
		{
			if(moveHeld) ChangeMoveState(MoveState.RUNNING);
			else		 ChangeMoveState(MoveState.IDLE);
		}
		else if(currentMoveState == MoveState.AIR_REELING)
		{
			ChangeMoveState(MoveState.GROUND_REELING);
		}
		else if(currentMoveState == MoveState.AIR_HOOKED)
		{
			ChangeMoveState(MoveState.GROUND_HOOKED);
		}

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

	#region TIMERS

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

	// Jump buffer counts in update
	private float jumpBuffer;
	public bool isJumpBufferActive()
	{
		return jumpBuffer > 0;
	}

	float jumpTimer = 0;
	public bool isJumpTimerActive()
	{
		return jumpTimer > 0;
	}

	public bool isFacingRight()
	{
		return spriteT.localScale.x > 0;
	}

	public bool isFacingLeft()
	{
		return spriteT.localScale.x < 0;
	}

	private void ProcessUpdateTimers()
	{
		// Timers
		if (jumpBuffer >= 0)
		{
			jumpBuffer -= Time.deltaTime;
		}
		if (jumpTimer >= 0)
		{
			jumpTimer -= Time.deltaTime;
		}
		if (landTimer <= bunnyHopWindow)
		{
			landTimer += Time.deltaTime;
		}
	}

	#endregion

	#region CONDITION CHECKS
	public void UpdateWater(bool newState)
	{
		inWater = newState;
		if(inWater && !onGround)
		{
			ChangeMoveState(MoveState.SWIMMING);
		}
	}

	private bool isGrounded()
	{
		if(jumpTime - jumpTimer > 0.1f) // dont check if left the ground 0.1 seconds ago (or less)
		{
			if (Physics2D.OverlapBox(groundCheckT.position, groundCheckSize, 0, groundLayer) || inWater)
			{
				if (!onGround) OnLand(); // first frame returning true, so just landed
				return true; // set onGround to true;
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

	private bool isFalling()
	{
		return (rb.linearVelocityY < 0 && currentRodState == RodState.INACTIVE);
	}

	#endregion

	string currentTrigger;

	void SetTrigger(string triggerName)
	{
		if (!string.IsNullOrEmpty(currentTrigger))
			animator.ResetTrigger(currentTrigger); // Clear the previous trigger

		animator.SetTrigger(triggerName); // Set the new trigger
		currentTrigger = triggerName; // Remember the new one
	}

	#region DEBUG DRAWING

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

	private void DebugDrawCircle(Vector2 center, float radius, Color color, float duration = 0f, int segments = 32)
	{
		float angleStep = 360f / segments;
		for (int i = 0; i < segments; i++)
		{
			float angleA = Mathf.Deg2Rad * angleStep * i;
			float angleB = Mathf.Deg2Rad * angleStep * (i + 1);

			Vector3 pointA = center + new Vector2(Mathf.Cos(angleA), Mathf.Sin(angleA)) * radius;
			Vector3 pointB = center + new Vector2(Mathf.Cos(angleB), Mathf.Sin(angleB)) * radius;

			Debug.DrawLine(pointA, pointB, color, duration);
		}
	}

	#endregion
}
