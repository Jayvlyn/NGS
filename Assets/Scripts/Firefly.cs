using UnityEngine;

public class Firefly : MonoBehaviour
{
    [SerializeField] private float minMoveSpeed = 2f;
    [SerializeField] private float maxMoveSpeed = 4f;
    [SerializeField] private float noiseScale = 0.5f; // Adjust for slower or faster drift
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float maxDistanceFromHome = 5f;
    [SerializeField] private float returnForce = 0.5f; // How strongly it pulls back when far

    private Vector3 homePosition;

    private float moveSpeed;
    private float xNoiseOffset;
    private float yNoiseOffset;
    private float lastXDirection = 1f;

    void Start()
    {
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        xNoiseOffset = Random.Range(0f, 100f);
        yNoiseOffset = Random.Range(0f, 100f);
        homePosition = transform.position; // Save spawn point
    }
    void Update()
    {
        float time = Time.time * noiseScale;

        float xDir = Mathf.PerlinNoise(time + xNoiseOffset, 0f) * 2f - 1f;
        float yDir = Mathf.PerlinNoise(0f, time + yNoiseOffset) * 2f - 1f;

        // Occasionally zero out one axis
        if (Random.value < 0.05f)
        {
            if (Random.value < 0.5f) xDir = 0f;
            else yDir = 0f;
        }

        Vector2 moveDir = new Vector2(xDir, yDir).normalized;

        // Check distance from home and blend movement toward it if too far
        Vector3 toHome = (homePosition - transform.position);
        float distance = toHome.magnitude;

        if (distance > maxDistanceFromHome)
        {
            // Blend original direction with the direction back to home
            Vector2 returnDir = toHome.normalized;
            moveDir = Vector2.Lerp(moveDir, returnDir, returnForce);
        }

        transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);

        // Flip sprite if direction changes
        if (Mathf.Sign(xDir) != Mathf.Sign(lastXDirection) && Mathf.Abs(xDir) > 0.2f)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            lastXDirection = Mathf.Sign(xDir);
        }
    }
}
