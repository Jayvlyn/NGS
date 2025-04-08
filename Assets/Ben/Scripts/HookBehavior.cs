using Unity.VisualScripting;
using UnityEngine;

public class HookBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] public Transform hookParent;
    [SerializeField] public GameObject catchTarget;
    [SerializeField] float hookFollowSpeed = 75.0f;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HookOutOfBoundsCheck();
    }

    public void HookOutOfBoundsCheck()
    {
        if (hookParent.position.x <= transform.position.x - 10)
        {
            KeepHookUnderBobber(-1, Vector2.Distance(transform.position, new Vector2(hookParent.position.x, transform.position.y)));
        }

        if (hookParent.position.x >= transform.position.x + 10)
        {
            KeepHookUnderBobber(1, Vector2.Distance(transform.position, new Vector2(hookParent.position.x, transform.position.y)));
        }
    }

    void KeepHookUnderBobber(int directionToMove, float distanceToBobber)
    {
        float moveDistance = distanceToBobber / hookFollowSpeed;
        float moveFinal = moveDistance * directionToMove;

        rb.MovePosition(new Vector2(transform.position.x + moveFinal, transform.position.y));

    }
}
