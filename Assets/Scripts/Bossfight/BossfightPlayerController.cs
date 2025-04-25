using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(DistanceJoint2D))]

public class BossfightPlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float reelSpeed = 1.0f;
    [SerializeField] private BossFishController boss;
    private Rigidbody2D body;
    private DistanceJoint2D joint;
    private readonly List<Collision2D> activeBlockers = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        joint = GetComponent<DistanceJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentDistance = Vector3.Distance(boss.transform.position, transform.position);
        Vector3 movement = new(0, 0);
        if (moveInput != Vector2.zero)
        {
            movement = movementSpeed * Time.deltaTime * moveInput.normalized;
            body.MovePosition(transform.position + movement);
        }
        if (holdingReel)
        {
            joint.distance -= reelSpeed * Time.deltaTime * Mathf.Pow(joint.distance * 0.2f, 2);
        }
        if (holdingSlack || activeBlockers.Count > 0)
        {
            joint.distance = currentDistance;
        }
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = 0;
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
			joint.enabled = false;
		}
		else
		{
            holdingReel = false;
			joint.enabled = activeBlockers.Count == 0;
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

	private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 relPos = joint.connectedBody.transform.position - transform.position;
        float angle = Vector3.Angle(collision.GetContact(0).normal, relPos);
        if (Mathf.Abs(angle) > 120)
        {
            activeBlockers.Add(collision);
            joint.enabled = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        activeBlockers.Remove(collision);
        joint.enabled = true;
    }

    public float DesiredDistance { get { return joint.distance; } }
}
