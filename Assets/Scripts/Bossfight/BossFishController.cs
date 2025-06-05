using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PathFollower))]
[RequireComponent (typeof(FishRotator))]
public class BossFishController : MonoBehaviour
{
    public static Fish bossFish;
    public static bool caughtBoss;

    [SerializeField] private BossfightPlayerController player;
    [SerializeField] private Camera cam;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float baseMovementSpeed;
    [SerializeField] private float maxSpeedMultiplier;
    [SerializeField] private FishRotator rotator;
    private Rigidbody2D body;
    private PathFollower pathFollower;
    private float speedMultiplier = 1f;
    private float cameraStartingZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (bossFish != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = bossFish.sprite;
        }
        cameraStartingZ = cam.transform.position.z;
        pathFollower = GetComponent<PathFollower>();
        body = GetComponent<Rigidbody2D>();
        caughtBoss = false;
    }

    // Update is called once per frame
    void Update()
    {
        (Vector3, Quaternion) newTransform = pathFollower.GetNewTransform(baseMovementSpeed * speedMultiplier * Time.deltaTime);
        rotator.BaseRotation = newTransform.Item2;
        body.MovePosition(newTransform.Item1);
        float zRotation = transform.rotation.eulerAngles.z;
        while(zRotation > 180)
        {
            zRotation -= 360;
        }
        while(zRotation < -180)
        {
            zRotation += 360;
        }
        if(transform.localScale.y > 0 && 
            (zRotation > 90 || zRotation < -90)
            || (transform.localScale.y < 0 && 
            zRotation < 90 && zRotation > -90))
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        }
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cameraStartingZ);
        speedMultiplier = Mathf.Lerp(1, maxSpeedMultiplier, 1 - player.DesiredDistance / player.MaxDistance);
    }

    public float GetRemainingDistance()
    {
        return pathFollower.GetRemainingLength();
    }
}
