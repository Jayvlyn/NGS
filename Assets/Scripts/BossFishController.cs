using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PathFollower))]
public class BossFishController : MonoBehaviour
{
    [SerializeField] private BossfightPlayerController player;
    [SerializeField] private Camera cam;
    [SerializeField] private float baseMovementSpeed;
    private float baseDistance = -1f;
    private Rigidbody2D body;
    private PathFollower pathFollower;
    private float speedMultiplier = 1f;
    private float cameraStartingZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraStartingZ = cam.transform.position.z;
        pathFollower = GetComponent<PathFollower>();
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(baseDistance == -1f)
        {
            baseDistance = player.DesiredDistance;
        }
        (Vector3, Quaternion) newTransform = pathFollower.GetNewTransform(baseMovementSpeed * speedMultiplier * Time.deltaTime);
        body.MovePositionAndRotation(newTransform.Item1, newTransform.Item2);
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cameraStartingZ);
        float distanceDifference = baseDistance - player.DesiredDistance;
        if(distanceDifference > 0)
        {
            speedMultiplier = Mathf.Max(speedMultiplier, Mathf.Pow(distanceDifference, 2));
        }
    }
}
