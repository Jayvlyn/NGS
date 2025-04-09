using Unity.VisualScripting;
using UnityEngine;

public class HookBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] public Transform hookParent;
    [SerializeField] public GameObject catchTarget;
    [SerializeField] public float hookFollowSpeed = 50.0f;
    public int hookDirection = 0;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HookOutOfBoundsCheck();
        RotateHookToBobber();
    }

    public void HookOutOfBoundsCheck()
    {
        if (hookParent.position.x <= transform.position.x - 10)
        {
            hookDirection = -1;
            KeepHookUnderBobber(Vector2.Distance(transform.position, new Vector2(hookParent.position.x, transform.position.y)));
            return;
        }

        if (hookParent.position.x >= transform.position.x + 10)
        {
            hookDirection = 1;
            KeepHookUnderBobber(Vector2.Distance(transform.position, new Vector2(hookParent.position.x, transform.position.y)));
            return;
        }

        hookDirection = 0;
    }

    void KeepHookUnderBobber(float distanceToBobber)
    {

        float moveDistance = distanceToBobber / hookFollowSpeed;
        float moveFinal = moveDistance * hookDirection;

        rb.MovePosition(new Vector2(transform.position.x + moveFinal, transform.position.y));

    }

    void RotateHookToBobber()
    {
        Vector2 dir = hookParent.transform.position - transform.position;
        Vector3 rotation = transform.localEulerAngles;

        rotation.z = (-dir.x / 20);

        transform.localEulerAngles = rotation;

        //transform.LookAt(dir, Vector3.forward);
        //float angle = Mathf.Cos()
    }
}
