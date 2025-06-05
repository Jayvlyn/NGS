using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class BossfightPlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float reelSpeed = 0.1f;
    [SerializeField] private BossFishController boss;
    [SerializeField] private float deathDistance = 10;
    [SerializeField] private float radius;
    [SerializeField] private float angleTolerance = 0.1f;
    [SerializeField] private bool immortalForTesting = false;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float desiredDistance = 2.5f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] GameSettings settings;
    [SerializeField] AudioClip waterLoop;
    [SerializeField] PlayerAudioManager playerAudioManager;
    private void Start()
    {
        GlobalAudioManager.Instance.StartLoopingAudioSource(waterLoop);
        deathDistance *= playerStats.bossLineLength;
    }

    // Update is called once per frame
    void Update()
    {
        float currentDistance = Vector2.Distance(boss.transform.position, transform.position);
        playerAudioManager.UpdateReelSound(1/currentDistance);
        if (holdingReel)
        {
            desiredDistance -= reelSpeed * desiredDistance * Time.deltaTime * playerStats.bossReelSpeed;
            desiredDistance = Mathf.Max(desiredDistance, radius * 5);
        }
        if(currentDistance > desiredDistance && !holdingSlack)
        {
            AttemptMovement((transform.position - boss.transform.position).normalized * (desiredDistance - currentDistance));
        }
        currentDistance = Vector2.Distance(boss.transform.position, transform.position);
        desiredDistance = currentDistance;
        if (currentDistance >= deathDistance && !immortalForTesting)
        {
            SceneLoader.LoadScene(settings.position.currentLocation);
        }
        if (GetMovement().magnitude > 0.1f)
        {
            //body.MovePosition(transform.position + movement);
            AttemptMovement(movementSpeed * Time.deltaTime * GetMovement());
            currentDistance = Vector3.Distance(boss.transform.position, transform.position);
            desiredDistance = Mathf.Max(Mathf.Min(deathDistance, currentDistance, desiredDistance), radius * 5);
        }
        if(currentDistance < radius * 5)
        {
            BossFishController.caughtBoss = true;
            SceneLoader.LoadScene(settings.position.currentLocation);
        }

        Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, radius);
        while(results.Length > 0)
        {
            Vector3 pos = results[0].ClosestPoint(transform.position);
            transform.position += (transform.position - pos).normalized * (radius - Vector3.Distance(transform.position, pos) + 0.01f);
            results = Physics2D.OverlapCircleAll(transform.position, radius);
        }
		//transform.rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), boss.transform.position - transform.position);
		//transform.rotation *= Quaternion.Euler(0, 0, -90);

		Vector3 direction = (boss.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.right, direction);// * Quaternion.Euler(0, 0, -90);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

	}

    public CinemachineCamera vCam;
    private Vector2 GetMovement()
    {
        Vector3 mousePosition = Input.mousePosition;

        // Get the current camera state from Cinemachine
        var cameraState = vCam.State;

        // Use the actual camera transform & orthographic size from Cinemachine state
        Vector3 camPos = cameraState.GetFinalPosition();
        Quaternion camRot = cameraState.GetFinalOrientation();
        float orthoSize = cameraState.Lens.OrthographicSize;

        // Calculate world position manually based on Cinemachine camera position and orthographic size:
        // For orthographic camera:
        // Screen space coords (0 to Screen.width, 0 to Screen.height)
        // map to world space based on camera pos, ortho size, and screen size

        // Normalize mouse screen position between -1 and 1 (centered)
        float normalizedX = (mousePosition.x / Screen.width) * 2 - 1;
        float normalizedY = (mousePosition.y / Screen.height) * 2 - 1;

        // Calculate world space offset
        float worldX = camPos.x + normalizedX * orthoSize * Camera.main.aspect;
        float worldY = camPos.y + normalizedY * orthoSize;

        Vector3 mouseWorldPosition = new Vector3(worldX, worldY, 0);

        if (settings.toggleData.isMouseModeBossgame)
        {
            return (mouseWorldPosition - transform.position).normalized;
        }
        else
        {
            return moveInput.normalized;
        }
    }


    #region INPUT

    private Vector2 moveInput;
	public void OnMove(InputValue value)
	{
        moveInput = value.Get<Vector2>();
	}

    private bool holdingReel;
    public void OnReel(InputValue value)
    {
        if (value.isPressed)
        {
            holdingReel = true;
            playerAudioManager.StartReelSound();

        }
        else
        {
            holdingReel = false;
            if(!holdingSlack)playerAudioManager.StopReelSound();
        }
    }

    private bool holdingSlack;
    public void OnSlack(InputValue value)
    {
		if (value.isPressed)
		{
            holdingSlack = true;
            playerAudioManager.StartReelSound();
            //joint.enabled = false;
        }
		else
		{
            holdingSlack = false;
            desiredDistance = Vector3.Distance(transform.position, boss.transform.position);
            if(!holdingReel)playerAudioManager.StopReelSound();
            //joint.enabled = activeBlockers.Count == 0;
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
        moveInput = Vector2.zero;
        holdingReel = false;
        holdingSlack = false;
	}

    #endregion

    private void AttemptMovement(Vector3 movement, bool continueAfter = true)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, movement.normalized, movement.magnitude,
            LayerMask.GetMask("BossLevel"));
        if(hit)
        {
            transform.position += movement.normalized * (hit.distance - 0.01f);
            desiredDistance = Vector3.Distance(transform.position, boss.transform.position);
            float dot = Vector3.Dot(movement.normalized, hit.normal.normalized);
            if (dot < angleTolerance && dot > angleTolerance)
            {
                Vector3 AMovement = Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * hit.normal.normalized;
                Vector3 BMovement = Quaternion.AngleAxis(-90, new Vector3(0, 0, 1)) * hit.normal.normalized;
                float ADot = Vector3.Dot(movement.normalized, AMovement);
                float BDot = Vector3.Dot(movement.normalized, BMovement);
                if (ADot > BDot)
                {
                    AttemptMovement(AMovement * (movement.magnitude - hit.distance + 0.01f) * ADot, false);
                }
                else
                {
                    AttemptMovement(BMovement * (movement.magnitude - hit.distance + 0.01f) * BDot, false);
                }
            }
        }
        else
        {
            transform.position += movement;
        }
    }

    public float DesiredDistance { get { return desiredDistance; } }
    public float MaxDistance { get { return deathDistance; } }
}
