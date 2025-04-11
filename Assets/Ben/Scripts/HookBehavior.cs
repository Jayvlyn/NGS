using Unity.VisualScripting;
using UnityEngine;

public class HookBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] public Transform hookParent;
    [SerializeField] public GameObject catchTarget;
    [SerializeField] float hookFollowSpeed = 75.0f;

    private bool hitObstacle = false;
    private float timer = 0;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HookOutOfBoundsCheck();

        if (hitObstacle && timer > 4)
        {
            hookFollowSpeed = 75;
        }

        timer += Time.deltaTime;
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

    public void ReduceSpeed(float multiplier)
    {
        hookFollowSpeed *= multiplier;
        hitObstacle = true;
        timer = 0;
    }
}
