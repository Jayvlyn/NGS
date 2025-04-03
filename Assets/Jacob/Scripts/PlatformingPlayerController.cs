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
    [SerializeField] private float changeXDirSharpness = 10;
    [SerializeField] private float groundFriction = 0.98f;

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

    private void Update()
    {
        onGround = isGrounded();

        Debug.Log(onGround);

        if(onGround)
        {
            rb.linearVelocityX *= groundFriction;
        }

        if(moveHeld)
        {
            float speed = onGround ? moveSpeed * 0.5f : moveSpeed; // half move speed in air
            // change dir
            
            if(onGround && ((rb.linearVelocityX > 0 && moveInput < 0) || (rb.linearVelocityX < 0 && moveInput > 0)))
            { // changing dir on ground
                speed *= changeXDirSharpness;
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

    private bool isGrounded()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            return true;
        }
        return false;
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
