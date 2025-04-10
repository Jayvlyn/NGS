using UnityEngine;


public class ControlBobber : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5.0f;
    [SerializeField] public float reelSpeed = 0.5f;

    [SerializeField] public GameObject hookObject;
    [SerializeField] public float lineLength = 500.0f;
    [SerializeField] public float maxLength = 850.0f;

    public float sideLength;
    public float depthLength;

    private HookBehavior hookBehavior;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hookBehavior = hookObject.GetComponent<HookBehavior>();
        //hookObject.transform.position = new Vector2(transform.position.x, transform.position.y - lineLength);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Temp inputs because input manager is weird

        if (Input.GetKey(KeyCode.A) && transform.localPosition.x >= -sideLength)
        {
            rb.MovePosition(new Vector2(rb.position.x - moveSpeed, rb.position.y));
        }

        if (Input.GetKey(KeyCode.D) && transform.localPosition.x <= sideLength)
        {
            rb.MovePosition(new Vector2(rb.position.x + moveSpeed, rb.position.y));
        }

        if (Input.GetKey(KeyCode.S) && hookObject.transform.localPosition.y > -depthLength)
        {
            lineLength += reelSpeed;

            float moveDistance = Vector2.Distance(hookObject.transform.position, new Vector2(transform.position.x, hookObject.transform.position.y)) / hookBehavior.hookResistanceVal;
            float moveFinal = moveDistance * hookBehavior.hookDirection;

            Vector2 movement = new Vector2(hookObject.transform.position.x + moveFinal, hookObject.transform.position.y - reelSpeed);

            hookObject.GetComponent<Rigidbody2D>().MovePosition(movement);
        }
        if (Input.GetKey(KeyCode.W) && hookObject.transform.localPosition.y < depthLength)
        {
            lineLength -= reelSpeed;
            Vector2 direction = transform.position - hookObject.transform.position;
            direction.Normalize();
            direction.Scale(new Vector2(reelSpeed, reelSpeed));
            hookObject.GetComponent<Rigidbody2D>().MovePosition(direction + new Vector2(hookObject.transform.position.x, hookObject.transform.position.y));
        }
    }

    //public void BobberMove(InputAction.CallbackContext value)
    //{

    //}

    //public void BobberReelLine(InputValue value)
    //{

    //}
}
