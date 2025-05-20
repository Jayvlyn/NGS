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
	[SerializeField] private PlatformingHook hook;
	[SerializeField] private Transform spriteT;
	[SerializeField] private CinemachineTargetGroup ctg;
	[HideInInspector] public Transform interactedWaterT;
	[SerializeField] private Animator animator;
	[SerializeField] private PlayerAudioManager audioManager;
	[SerializeField] VoidEvent onInventory;
	[SerializeField] private GameObject rod;
	[SerializeField] private Transform rodT;
	public Transform rodEnd;
	[SerializeField] private Animator rodAnimator;
	private Camera cam;

	[Header("Stats")]
	[SerializeField] private float moveSpeed = 5f;

	[SerializeField] private float moveVelocityLimit = 10f;
	[SerializeField] private float bhopVelocityLimit = 20f;
	//[SerializeField] private float reelVelocityLimit = 40f;

	[SerializeField] private float jumpForce = 20f;
	[SerializeField] private float airControlMod = 0.4f;
	[SerializeField] private float iceMoveMod = 0.5f;

	[SerializeField] private float bhopForce = 20f;

	[SerializeField] private PlayerStats playerStats;
	public float MaxLineLength { get { return playerStats.platformingLineLength; } }

	[SerializeField, Tooltip("When changing direction, the players curent speed is used for counteraction, this variable multiplies against the current player speed")]
	private float changeDirSpeedMult = 0.5f;

	[SerializeField, Tooltip("Linear X Velocity of player will get multiplied by this value while on the ground (1 = no friction)"), Range(0.8f, 1)]
	private float groundFriction = 0.98f;

	[SerializeField, Tooltip("Linear Velocity of player will get multiplied by this value while off the ground (1 = no friction)"), Range(0.8f, 1)]
	private float airFriction = 0.98f;

	[SerializeField, Tooltip("Linear Velocity of player will get multiplied by this value while sliding on ice (1 = no friction)"), Range(0.8f, 1)]
	private float iceFriction = 0.98f;

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
	[SerializeField] private bool wallJumpLimit = false;
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
	[SerializeField] private LayerMask iceLayer;
	[SerializeField] private Transform groundCheckT;
	[SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
	[SerializeField] private float slopeCheckDistance = 0.15f;
	[SerializeField] private float slopeModifier = 1.5f;
	private bool onGround;
	private bool onIce = false;
	private bool touchingIceWall = false;
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

	private bool castHeld;
	public void OnCastHook(InputValue value)
	{
		if (value.isPressed)
		{
			castHeld = true;
			if (currentRodState == RodState.INACTIVE && !inWater)
			{
				ChangeRodState(RodState.CASTING);
			}
		}
		else // released
		{
			castHeld = false;
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
			audioManager.StartReelSound();
		}
		else // released
		{
			reelHeld = false;
			audioManager.StopReelSound();
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

		Vector3 waterDir = (waterMidpoint - (Vector2)rodEnd.position).normalized;
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
			float speed = moveSpeed;
			if (!onGround) speed *= airControlMod;
			else if (inWater) speed *= airControlMod * 0.5f;
			else if (onIce) speed *= iceMoveMod;
			else if (currentRodState == RodState.HOOKED && onGround) speed *= 0.5f;


			if (onGround && (rb.linearVelocityX * moveInput < 0)) // when velocity * input results in negative, they are opposite
			{ // changing dir on ground
				speed *= changeDirSpeedMult;
			}
			//Debug.Log(speed);
			// move when not moving max speed
			if (isUnderMaxMoveSpeed() && !isWallBlockingMoveDir())
			{
				//Vector2 dir = new Vector2(moveInput, 0);
				Vector2 movement = GetMovement(speed);
				if(onIce && movement.y != 0)
				{
					movement *= 0.1f;
				}
                rb.AddForce(movement, ForceMode2D.Force);
			}
		}
        if (onIce)
        {
            Vector2 slopeA = GetSlope(-1);
            Vector2 slopeB = GetSlope(1);
            if (slopeA.y < 0 || slopeB.y < 0)
            {
				Vector2 slope = slopeA.y < 0 ? slopeA : slopeB;
				slope.y *= 2f;
                rb.AddForce(2 * slope);
            }
			else
			{
				Debug.Log("Not");
			}
        }
    }

	private Vector2 GetMovement(float speed)
	{
		Vector2 movement = GetSlope(moveInput);
		Debug.Log(movement);
		movement *= speed;
		if(movement.y != 0)
		{
			movement += -1.5f * rb.gravityScale * Time.fixedDeltaTime * Physics2D.gravity;
        }
		return movement;
	}

	private Vector2 GetSlope(float direction = 0)
	{
		//Vector2 result = new Vector2(magnitude, 0);
		//float absMagnitude;
		//bool forMovement = true;
		//if(magnitude == 0)
		//{
		//          absMagnitude = magnitude == 0 ? 1 : Mathf.Abs(magnitude);
		//	magnitude = 1;
		//	forMovement = false;
		//      }
		//else
		//{
		//	absMagnitude = Mathf.Abs(magnitude);
		//}
		//Vector2 from = forMovement ? new Vector2(transform.position.x, transform.position.y + slopeCheckDistance * absMagnitude * 4f + 0.01f) : new Vector2(transform.position.x - slopeCheckDistance, transform.position.y + slopeCheckDistance * 4 + 0.01f);
		////Vector2 to = from + (Vector2.down * (slopeCheckDistance * 12f + 0.05f));
		//      RaycastHit2D hit = Physics2D.Raycast(from, Vector2.down, slopeCheckDistance * 12f + 0.05f, groundLayer);
		//      //Debug.DrawLine(from, to, hit ? Color.green : Color.red, 2);
		//      if (hit)
		//{
		//	from = new Vector2(transform.position.x + slopeCheckDistance * magnitude, transform.position.y + slopeCheckDistance * absMagnitude * 4f + 0.01f);
		//	//to = from + (Vector2.down * (slopeCheckDistance * 12f + 0.05f));
		//	RaycastHit2D hit2 = Physics2D.Raycast(from, Vector2.down, slopeCheckDistance * 12f + 0.05f, groundLayer);
		//	//Debug.DrawLine(from, to, hit2 ? Color.green : Color.red, 2);
		//          if (hit2)
		//          {
		//              result = hit2.point - hit.point;
		//              result = absMagnitude * result.normalized;
		//          }
		//      }
		if(direction == 0)
		{
			direction = isFacingLeft() ? -1 : 1;
		}
		else
		{
			direction = Mathf.Sign(direction);
		}
		Vector2 result = new Vector2(direction, 0);
		float divideBy = 0;
		RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x + slopeCheckDistance * direction, transform.position.y + slopeCheckDistance * 2), Vector2.down, 1, groundLayer); 
        if (hit)
        {
			result = Quaternion.Euler(0, 0, direction * -90) * hit.normal;
			Vector2Int psuedo = new Vector2Int((int)(result.x * 100), (int)(result.y * 100));
			result = new Vector2(psuedo.x * 0.01f, psuedo.y * 0.01f);
			divideBy++;
        }
        hit = Physics2D.Raycast(transform.position, Vector2.down, 1, groundLayer);
        if (hit)
        {
            Vector3 next = Quaternion.Euler(0, 0, direction * -90) * hit.normal;
            if (Mathf.Abs(next.y) < 0.1f && result.y < 0)
            {
				result = Vector3.zero;
				divideBy = 1;
            }
            Vector2Int psuedo = new Vector2Int((int)(next.x * 100), (int)(next.y * 100));
            result += new Vector2(psuedo.x * 0.01f, psuedo.y * 0.01f);
            divideBy++;
        }
        hit = Physics2D.Raycast(new Vector3(transform.position.x - slopeCheckDistance * direction, transform.position.y + slopeCheckDistance * 2), Vector2.down, 1, groundLayer);
		Debug.DrawLine(new Vector3(transform.position.x - slopeCheckDistance * direction, transform.position.y + slopeCheckDistance * 2), new Vector3(transform.position.x - slopeCheckDistance * direction, transform.position.y + slopeCheckDistance * 2 - 1), Color.black, 1);
        if (hit)
        {
            Vector3 next = Quaternion.Euler(0, 0, direction * -90) * hit.normal;
			if(Mathf.Abs(next.y) < 0.1f && result.y < 0)
			{
				result = Vector3.zero;
				divideBy = 1;
			}
            Vector2Int psuedo = new Vector2Int((int)(next.x * 100), (int)(next.y * 100));
            result += new Vector2(psuedo.x * 0.01f, psuedo.y * 0.01f);
            divideBy++;
        }
		//Debug.Log(result);
        return result / (divideBy > 0 ? divideBy : 1);
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
		castHeld = false;
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
			audioManager.StopWallSlideSound();
		}

		//Debug.Log($"Changing from {currentMoveState} to {state}");
		currentMoveState = state;
		switch (state)
		{
			case MoveState.IDLE:
				audioManager.StopRunSound();
				SetTrigger("ToIdle");
				break;
			case MoveState.RUNNING:
				audioManager.StartRunSound();
				SetTrigger("ToRun");
				break;
			case MoveState.JUMPING:
				audioManager.StopRunSound();
				SetTrigger("ToJump");
				break;
			case MoveState.FALLING:
				audioManager.StopRunSound();
				SetTrigger("ToFall");
				break;
			case MoveState.WALLJUMPING:
				SetTrigger("ToWallJump");
				break;
			case MoveState.SWIMMING:
				audioManager.StopRunSound();
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
				audioManager.PlayLandSound();
				audioManager.StartWallSlideSound();
				FlipX();
				rb.linearVelocityY = 0;
				if(touchingIceWall)
				{ // ice wall, not as sticky
					rb.gravityScale = startingGravity * 0.5f;
				}
				else // normal wall, full stick
				{
					rb.gravityScale = startingGravity * 0.1f;
				}

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
				else if (inWater) ChangeMoveState(MoveState.SWIMMING);
				
				break;
			case MoveState.RUNNING:
				if (!isGrounded() && isFalling()) ChangeMoveState(MoveState.FALLING);
				else if (inWater) ChangeMoveState(MoveState.SWIMMING);
				if (isFacingLeft() && moveInput > 0 || isFacingRight() && moveInput < 0) FlipX();
				break;
			case MoveState.JUMPING:
				if (isFalling()) ChangeMoveState(MoveState.FALLING);
				isTouchingLeftWall();
				if(jumpTime - jumpTimer > 0.15f)
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
					audioManager.UpdateWallSlideSound(rb.gravityScale);
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
			if (currentRodState != RodState.HOOKED && !onIce)
			{
				rb.linearVelocityX *= groundFriction;
			}
			else if(onIce)
			{
				rb.linearVelocityX *= iceFriction;
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
		rod.transform.localScale = new Vector2(rod.transform.localScale.x * -1, rod.transform.localScale.y);
		if (currentRodState == RodState.INACTIVE || currentRodState == RodState.FISHCASTING)
			hook.transform.localScale = new Vector2(hook.transform.localScale.x * -1, hook.transform.localScale.y);
	}

	private void LookAtHook()
	{
		Vector2 direction = ((Vector2)hook.rb.transform.position - (Vector2)transform.position).normalized;
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
				rod.transform.localRotation = Quaternion.identity;
				rod.SetActive(false);
				OnEnterInactiveState();
				if(currentMoveState == MoveState.WALKING_REELING) // returning
				{
					ChangeMoveState(MoveState.RUNNING);
				}
				break;

			case RodState.CASTING:
				audioManager.PlayCastSound();
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
				hook.PlayHookHitSound();
				OnEnterHookedState();
				distanceJoint.distance = Vector2.Distance(transform.position, hook.rb.transform.position);
                if (onGround) ChangeMoveState(MoveState.GROUND_HOOKED);
				else ChangeMoveState(MoveState.AIR_HOOKED);
				break;

			case RodState.FISHCASTING:
				audioManager.PlayCastSound();
				OnEnterFishCastingState();
				if (onGround) ChangeMoveState(MoveState.GROUND_CASTING);
				else ChangeMoveState(MoveState.AIR_CASTING);

				break;
		}
	}

	private void UpdateLineRendererEnds(bool playerEnd = true, bool hookEnd = true)
	{
		//if (playerEnd) lineRenderer.SetPosition(1, transform.position);
		if (playerEnd) lineRenderer.SetPosition(1, rodEnd.position);
		if (hookEnd) lineRenderer.SetPosition(0, hook.rb.position);
	}

	private void ProcessRodStateUpdate()
	{
		switch (currentRodState)
		{
			case RodState.CASTING:
				UpdateLineRendererEnds();
				if (doPostAnimRotation) UpdateRodRot();
				break;
			case RodState.RETURNING:
				UpdateLineRendererEnds();
				break;
			case RodState.HOOKED:
				UpdateLineRendererEnds();
				UpdateRodRot();
				break;
			case RodState.FISHCASTING:
				UpdateLineRendererEnds();
				break;
			default:
				break;
		}
	}

	private void UpdateRodRot()
	{
		Vector2 direction = (hook.transform.position - rodT.transform.position).normalized;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		rod.transform.localRotation = Quaternion.Euler(0f, 0f, angle - 90);
	}

	private void ProcessRodStateFixedUpdate()
	{
		// Process Rod State
		switch (currentRodState)
		{
			case RodState.RETURNING:
				//Debug.Log(hookRb.transform.position);
				hook.rb.linearVelocity *= hookReturnFriction; // dampen so it doesnt constantly fly past player trying to return
				Vector2 dir = (rodEnd.position - hook.rb.transform.position).normalized;
				hook.rb.AddForce(dir * playerStats.platformingReelSpeed * hookReturnSpeedMod, ForceMode2D.Force);

				if (Vector2.Distance(hook.rb.gameObject.transform.position, rodEnd.position) <= completeReturnDistance)
				{
					ChangeRodState(RodState.INACTIVE);
				}

				break;
			case RodState.HOOKED:
				{
					float dist = Vector2.Distance(rodEnd.position, hook.rb.transform.position);
					float maxLen = playerStats.platformingLineLength;
					float minLen = 0f;
					float currLen = distanceJoint.distance;

					//if (dist <= maxLen && currLen > dist) currLen = dist;

					if (dist < detachDistance)
					{
						ChangeRodState(RodState.RETURNING);
						break;
					}

					float delta = 0f;

					bool addForce = false;
					if(reelHeld && !slackHeld)
					{
						addForce = true;
					}

					if (reelHeld && !slackHeld || distanceJoint.distance > MaxLineLength) delta = -Time.deltaTime * playerStats.platformingReelSpeed * 0.05f;


					else if (reelHeld && slackHeld) delta = Time.deltaTime * playerStats.platformingReelSpeed;

					currLen += delta;

					if (delta > 0f) currLen = Mathf.Min(currLen, maxLen);

					else if (delta < 0f) currLen = Mathf.Max(currLen, minLen);

					distanceJoint.distance = currLen;

					if (delta < 0f)
					{
						audioManager.UpdateReelSound(rb.linearVelocity.magnitude);

						if (currentMoveState is not MoveState.GROUND_REELING and not MoveState.AIR_REELING)
							ChangeMoveState(onGround ? MoveState.GROUND_REELING : MoveState.AIR_REELING);

						if (addForce)
						{
							dir = (hook.rb.transform.position - rodEnd.position).normalized;
							rb.AddForce(dir * playerStats.platformingReelSpeed, ForceMode2D.Force);
						}
					}

					else if (delta > 0f) audioManager.UpdateReelSound(rb.linearVelocity.magnitude);


					break;
				}

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
		hook.rb.transform.position = rodEnd.position;
		Vector2 initialPosition = rodEnd.position;
		while (t < playerStats.grappleMaxCastSpeed)
		{
			Vector2 currentPos;
			currentPos.x = Mathf.Lerp(initialPosition.x, point.x, t / playerStats.grappleMaxCastSpeed);
			currentPos.y = initialPosition.y + grappleCastCurve.Evaluate(t / playerStats.grappleMaxCastSpeed);
			hook.rb.transform.position = currentPos;

			float modifier = 2;

			//if(t/castTime < 0.5) modifier -= t / castTime;
			//else modifier += t / castTime;

			t += Time.deltaTime * modifier;
			yield return null;
		}
		hook.rb.transform.position = point;

		if(willHook)ChangeRodState(RodState.HOOKED);
		else ChangeRodState(RodState.RETURNING);
		
	}
	private Coroutine castHookToPoint;

	private void OnEnterCastingState()
	{
		if (!AttemptMouseFish())
		{
			StartCoroutine(RodCast());
		}
	}

	private bool doPostAnimRotation = false;
	private IEnumerator RodCast()
	{ //HERE2
		rod.SetActive(true);
		rodAnimator.enabled = true;
		doPostAnimRotation = false;
		Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
		Vector2 dir = (mousePos - (Vector2)rodEnd.position).normalized;
		if (dir.x * spriteT.localScale.x < 0f)
		{
			FlipX(); // flip to look in casting direction
		}

		Vector2 overlapPos = mousePos;
		float distanceToMouse = Vector2.Distance(rodEnd.position, mousePos);
		if (distanceToMouse > playerStats.platformingLineLength)
		{
			overlapPos = (Vector2)rodEnd.position + dir * playerStats.platformingLineLength;
		}

		Collider2D hit = Physics2D.OverlapCircle(overlapPos, aimAssistRadius, grappleableLayer);
		DebugDrawCircle(overlapPos, aimAssistRadius, Color.green, 1.5f);

		yield return new WaitForSeconds(0.15f);
		if (!castHeld && currentRodState != RodState.FISHCASTING) yield break;

		hook.col.isTrigger = true;
		hook.rb.bodyType = RigidbodyType2D.Kinematic;
		hook.rb.transform.position = rodEnd.position;
		hook.rb.gameObject.SetActive(true);

		lineRenderer.enabled = true;

		Vector2 hookPos = Vector2.zero;
		if (hit != null)
		{
			hookPos = hit.ClosestPoint(overlapPos);
			hook.rb.transform.position = hookPos;
			hook.rb.transform.parent = null;

			DefineCurveKeys(hookPos);

			if (castHookToPoint != null) StopCoroutine(castHookToPoint);
			castHookToPoint = StartCoroutine(CastHookToPoint(hookPos, true));
		}
		else
		{
			hookPos = (Vector2)rodEnd.position + dir * MaxLineLength;
			hook.rb.transform.position = hookPos;

			DefineCurveKeys(hookPos);

			if (castHookToPoint != null) StopCoroutine(castHookToPoint);
			castHookToPoint = StartCoroutine(CastHookToPoint(hookPos, false));
		}
		
		yield return new WaitForSeconds(0.15f);
		Vector3 cachedPos = rodT.localPosition;
		rodAnimator.enabled = false;
		rodT.localPosition = cachedPos;
		rodT.localRotation = Quaternion.identity;
		doPostAnimRotation = true;
	}

	private bool AttemptMouseFish()
	{
		foreach(Collider2D col in Physics2D.OverlapCircleAll(cam.ScreenToWorldPoint(Input.mousePosition), aimAssistRadius))
		{
			Transform tr = col.gameObject.transform;
			while(tr.parent != null)
			{
				tr = tr.parent;
			}
			InteractableObject obj = tr.gameObject.GetComponentInChildren<InteractableObject>();
			if (CanInteractWith(obj) && obj.interactionType == InteractionType.Fish && Time.timeScale != 0)
			{
				TryInteract(obj);
				return true;
			}
		}
		return false;
	}

    private void DefineCurveKeys(Vector2 hookPos)
	{
		float yDiff = hookPos.y - rodEnd.position.y;
		float distance = Vector2.Distance(hookPos, rodEnd.position);

		playerStats.grappleMaxCastSpeed = distance * 0.2f;

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
		distanceJoint.connectedAnchor = hook.rb.gameObject.transform.position;
		distanceJoint.enabled = true;

		hook.rb.bodyType = RigidbodyType2D.Kinematic;
		hook.rb.linearVelocity = Vector2.zero;

		UpdateLineRendererEnds();
	}

	private void OnEnterReturningState()
	{
		hook.col.isTrigger = true;
		distanceJoint.enabled = false;
		hook.rb.bodyType = RigidbodyType2D.Dynamic;
		hook.rb.transform.parent = transform;
	}

	private void OnEnterInactiveState()
	{
		hook.col.isTrigger = true;
		hook.rb.transform.position = rodEnd.position;
		hook.rb.gameObject.SetActive(false);
		distanceJoint.enabled = false;
		lineRenderer.enabled = false;
	}

	private void OnEnterFishCastingState()
	{
		hook.col.isTrigger = false;
		hook.rb.gameObject.SetActive(true);

		lineRenderer.enabled = true;

		StartCoroutine(VisualFishCast(2));
	}

	public IEnumerator VisualFishCast(float speed = 1)
	{ //HERE
		rod.SetActive(true);
		rodAnimator.enabled = true;
		doPostAnimRotation = false;
		yield return new WaitForSeconds(0.3f);

		Vector3 cachedPos = rodT.localPosition;
		Quaternion cachedRot = rodT.localRotation;
		doPostAnimRotation = true;
		rodAnimator.enabled = false;
		rodT.localPosition = cachedPos;
		rodT.localRotation = cachedRot;


		float dist = Mathf.Abs(waterMidpoint.x - rodEnd.position.x);
		hook.rb.bodyType = RigidbodyType2D.Kinematic;
		float t = 0;
		while (t < dist)
		{
			Vector2 pos;
			pos.x = (t / dist) * Mathf.Sign(spriteT.localScale.x);
			pos.y = fishCastCurve.Evaluate(t / dist);
			hook.rb.transform.position = (Vector2)this.transform.position + pos;

			t += Time.deltaTime * speed;
			yield return null;
		}

		hook.rb.bodyType = RigidbodyType2D.Dynamic;
		hook.rb.linearVelocityY = -1;
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
			audioManager.StopJumpSound();
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
		audioManager.PlayJumpSound();
		onIce = false;
		rb.gravityScale = startingGravity;
		//if (currentRodState != RodState.INACTIVE) ChangeRodState(RodState.RETURNING);
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
		audioManager.PlayJumpSound();
		if (currentRodState != RodState.INACTIVE) ChangeRodState(RodState.RETURNING);
		ChangeMoveState(MoveState.WALLJUMPING);
		if(wallJumpLimit) currentWallJumps--;

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
		audioManager.PlayLandSound();
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
		Vector2 slope = GetSlope();
		if (Mathf.Abs(slope.x) >= Mathf.Abs(slope.y))
		{
			currentJumps = totalJumps;
		}
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
				isOnIce();
				//Debug.Log("checks past");
                if (!onGround) OnLand(); // first frame returning true, so just landed
				else //stayed on ground
				{
                    //regenerates jump if you were on a steep slope and no longer are, or disables it if inverse
                    Vector2 slope = GetSlope();
                    if (Mathf.Abs(slope.x) >= Mathf.Abs(slope.y))
                    {
                        currentJumps = totalJumps;
                    }
					else
					{
						currentJumps = 0;
					}
                }
				return true; // set onGround to true;
			}
			// not on currently ground:
			if(onIce) // on ice last check
			{ 
				onIce = false;
				rb.gravityScale = startingGravity;
			}
		

            if (onGround) // was grounded last check
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

	private bool isOnIce()
	{
		if(Physics2D.OverlapBox(groundCheckT.position, groundCheckSize, 0, iceLayer))
		{
			onIce = true;
			if(true/*GetSlope().y == 0*/)
			{
				rb.gravityScale = 0;
			}
			return true;
		}
		else
		{
			onIce = false;
			rb.gravityScale = startingGravity;
			return false;
		}
	}

	public LayerMask iceWallLayer;
	private bool isTouchingRightWall()
	{
		Collider2D col = Physics2D.OverlapBox(rightCheckT.position, rightCheckSize, 0, wallLayer);
		if (col != null)
		{
			touchingIceWall = (iceWallLayer == (iceWallLayer | (1 << col.gameObject.layer)));
			return true;
		}
		return false;
	}

	private bool isTouchingLeftWall()
	{
		Collider2D col = Physics2D.OverlapBox(leftCheckT.position, leftCheckSize, 0, wallLayer);
		if (col != null)
		{
			touchingIceWall = (iceWallLayer == (iceWallLayer | (1 << col.gameObject.layer)));
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

		//Slope Checks
		Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(new Vector3(transform.position.x + slopeCheckDistance, transform.position.y + slopeCheckDistance * 4 + 0.01f), new Vector3(transform.position.x + slopeCheckDistance, transform.position.y - (slopeCheckDistance * 8f + 0.04f)));
        //Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + slopeCheckDistance * 4 + 0.01f), new Vector3(transform.position.x, transform.position.y - (slopeCheckDistance * 8f + 0.04f)));
        //Gizmos.DrawLine(new Vector3(transform.position.x - slopeCheckDistance, transform.position.y + slopeCheckDistance * 4 + 0.01f), new Vector3(transform.position.x - slopeCheckDistance, transform.position.y - (slopeCheckDistance * 8f + 0.04f)));

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
