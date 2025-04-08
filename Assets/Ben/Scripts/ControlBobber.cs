using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ControlBobber : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5.0f;
    [SerializeField] public float reelSpeed = 0.5f;

    [SerializeField] public GameObject hookObject;
    [SerializeField] public float lineLength = 500.0f;
    [SerializeField] public float maxLength = 850.0f;


    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //hookObject.transform.position = new Vector2(transform.position.x, transform.position.y - lineLength);
    }

    // Update is called once per frame
    void Update()
    {
        // Temp inputs because input manager is weird

        if (Input.GetKey(KeyCode.A) && transform.localPosition.x >= -500)
        {
            rb.MovePosition(new Vector2(rb.position.x - moveSpeed, rb.position.y));
        }

        if (Input.GetKey(KeyCode.D) && transform.localPosition.x <= 500)
        {
            rb.MovePosition(new Vector2(rb.position.x + moveSpeed, rb.position.y));
        }

        if (Input.GetKey(KeyCode.S) && hookObject.transform.position.y > -850.0f)
        {
            lineLength += reelSpeed;
            hookObject.GetComponent<Rigidbody2D>().MovePosition(new Vector2(hookObject.transform.position.x, hookObject.transform.position.y - reelSpeed));

        }
        if (Input.GetKey(KeyCode.W) && hookObject.transform.position.y < transform.position.y)
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
