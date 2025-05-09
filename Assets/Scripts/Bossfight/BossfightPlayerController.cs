using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    [SerializeField] GameSettings settings;

    private void Start()
    {
        deathDistance *= playerStats.bossLineLength;
    }

    // Update is called once per frame
    void Update()
    {
        float currentDistance = Vector3.Distance(boss.transform.position, transform.position);
        if (holdingReel)
        {
            desiredDistance -= reelSpeed * desiredDistance * Time.deltaTime * playerStats.bossReelSpeed;
            desiredDistance = Mathf.Max(desiredDistance, radius * 5);
        }
        if (holdingSlack)
        {
            desiredDistance = Mathf.Min(deathDistance, currentDistance);
        }
        else if(currentDistance > desiredDistance)
        {
            AttemptMovement((transform.position - boss.transform.position).normalized * (desiredDistance - currentDistance));
        }
        currentDistance = Vector3.Distance(boss.transform.position, transform.position);
        if(currentDistance >= deathDistance && !immortalForTesting)
        {
            SceneManager.LoadScene("GameScene");
        }
        if (GetMovement().magnitude > 0.1f)
        {
            //body.MovePosition(transform.position + movement);
            AttemptMovement(movementSpeed * Time.deltaTime * GetMovement().normalized);
            currentDistance = Vector3.Distance(boss.transform.position, transform.position);
            desiredDistance = Mathf.Max(Mathf.Min(deathDistance, currentDistance, desiredDistance), radius * 5);
        }
        if(currentDistance < radius * 2)
        {
            BossFishController.caughtBoss = true;
            SceneManager.LoadScene("GameScene");
        }
        transform.rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), boss.transform.position - transform.position);
        transform.rotation *= Quaternion.Euler(0, 0, -90);

        //if (holdingReel)
        //{
        //    //joint.distance -= reelSpeed * Time.deltaTime * Mathf.Pow(joint.distance * 0.2f, 2);
        //}
        //if (holdingSlack || activeBlockers.Count > 0)
        //{
        //    //joint.distance = currentDistance;
        //    if(/*joint.distance >= deathDistance)*/true)
        //    {
        //        if(activeBlockers.Count > 0)
        //        {
        //            SceneManager.LoadScene("GameScene");
        //        }
        //        else
        //        {
        //            //joint.distance = deathDistance;
        //        }
        //    }
        //}
        //body.linearVelocity = Vector3.zero;
        //body.angularVelocity = 0;
    }

    private Vector2 GetMovement()
    {
        Vector2 result;
        if(settings.toggleData.isMouseModeBossgame)
        {
            result = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        }
        else
        {
            result = moveInput.normalized;
        }
        return result;
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
        }
        else
        {
            holdingReel = false;
        }
    }

    private bool holdingSlack;
    public void OnSlack(InputValue value)
    {
		if (value.isPressed)
		{
            holdingSlack = true;
			//joint.enabled = false;
		}
		else
		{
            holdingSlack = false;
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

    private void AttemptMovement(Vector3 movement)
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
                    AttemptMovement(AMovement * (movement.magnitude - hit.distance + 0.01f) * ADot);
                }
                else
                {
                    AttemptMovement(BMovement * (movement.magnitude - hit.distance + 0.01f) * BDot);
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
