using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HookBehavior : MonoBehaviour
{
    [SerializeField] public Transform hookParent;
    [SerializeField] public GameObject catchTarget;
    [SerializeField] public float hookResistanceVal = 25.0f; // The higher, the slower
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
            Debug.Log("Hook should move left");
            hookDirection = -1;
            KeepHookUnderBobber(Vector2.Distance(transform.position, new Vector2(hookParent.position.x, transform.position.y)));
            return;
        }

        if (hookParent.position.x >= transform.position.x + 10)
        {
            Debug.Log("Hook should move right");
            hookDirection = 1;
            KeepHookUnderBobber(Vector2.Distance(transform.position, new Vector2(hookParent.position.x, transform.position.y)));
            return;
        }

        hookDirection = 0;
    }

    void KeepHookUnderBobber(float distanceToBobber)
    {
        Debug.Log("Move Hook");
        float moveDistance = distanceToBobber / hookResistanceVal;
        float moveFinal = moveDistance * hookDirection;

        rb.MovePosition(new Vector2(transform.position.x + moveFinal, transform.position.y));
    }

    void RotateHookToBobber()
    {
        Vector2 dir = hookParent.transform.position - transform.position;
        Vector3 rotation = transform.localEulerAngles;

        float angle = -dir.x / 10;

        rotation.z = angle;

        transform.localEulerAngles = rotation;

    }
}
