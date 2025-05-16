using UnityEngine;

public class HookBehavior : MonoBehaviour
{
    [SerializeField] public Transform hookParent;
    public float hookResistanceVal = 25.0f; // Higher = slower
    private float originalResistanceVal;
    public int hookDirection = 0;

    private float verticalInput = 0f;
    public float verticalSpeed = 0.5f;

    private Rigidbody2D rb;

    private Vector2 externalVelocity = Vector2.zero;
    [SerializeField] private float externalDecayRate = 3f;

    [SerializeField] private float minY = -20f;
    [SerializeField] private float maxY = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Prevent falling
        originalResistanceVal = hookResistanceVal;
    }

    void FixedUpdate()
    {
        HookOutOfBoundsCheck();
        MoveHook();
        RotateHookToBobber();
    }

    public void SetVerticalInput(float input)
    {
        verticalInput = input;
    }

    void MoveHook()
    {
        float horizontalMove = 0f;

        if (hookDirection != 0)
        {
            float distanceToBobber = Vector2.Distance(transform.position, new Vector2(hookParent.position.x, transform.position.y));
            horizontalMove = (distanceToBobber / hookResistanceVal) * hookDirection;
        }

        float verticalMove = verticalInput * verticalSpeed;

        Vector2 controlMovement = new Vector2(horizontalMove, verticalMove);
        Vector2 finalMove = controlMovement + externalVelocity * Time.fixedDeltaTime;

        Vector2 targetPos = rb.position + finalMove;

        //Debug.Log("Before: " + targetPos.y);
        // Clamp the Y position within min and max bounds
        targetPos.y = Mathf.Clamp(targetPos.y, minY * Screen.height, maxY * Screen.height);
        //Debug.Log("After: " + targetPos.y);
        //Debug.Log("Max: "+ maxY * Screen.currentResolution.height);
        //Debug.Log("Min: "+ minY * Screen.currentResolution.height);

        rb.MovePosition(targetPos);

        // Smooth decay of external velocity
        externalVelocity = Vector2.Lerp(externalVelocity, Vector2.zero, externalDecayRate * Time.fixedDeltaTime);
    }

    void HookOutOfBoundsCheck()
    {
        float xDiff = hookParent.position.x - transform.position.x;

        if (Mathf.Abs(xDiff) >= 10f)
        {
            hookDirection = xDiff < 0 ? -1 : 1;
        }
        else
        {
            hookDirection = 0;
        }
    }

    void RotateHookToBobber()
    {
        Vector2 dir = hookParent.position - transform.position;
        float angle = -dir.x / 10f;
        Vector3 rotation = transform.localEulerAngles;
        rotation.z = angle;
        transform.localEulerAngles = rotation;
    }

    public void ChangeSpeed(float newVal)
    {
        hookResistanceVal = newVal;
    }

    public void ResetSpeed()
    {
        hookResistanceVal = originalResistanceVal;
    }

    public void ApplyImpulse(Vector2 force)
    {
        externalVelocity += force;
    }
}
