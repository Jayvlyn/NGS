using UnityEngine;
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(DistanceJoint2D))]

public class BossfightPlayerController : MonoBehaviour
{
    //Input will be changed to event system after basic functionality is confirmed
    [SerializeField] private KeyCode upKey = KeyCode.W;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [SerializeField] private KeyCode reelKey = KeyCode.E;
    [SerializeField] private KeyCode slackKey = KeyCode.Q;
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float reelSpeed = 1.0f;
    [SerializeField] private BossFishController boss;
    private bool colliding = false;
    private Rigidbody2D body;
    private DistanceJoint2D joint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        joint = GetComponent<DistanceJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentDistance = Vector3.Distance(boss.transform.position, transform.position);
        Vector3 movement = new Vector3(0, 0);
        if (Input.GetKey(rightKey))
        {
            movement.x++;
        }
        if (Input.GetKey(leftKey))
        {
            movement.x--;
        }
        if (Input.GetKey(upKey))
        {
            movement.y++;
        }
        if(Input.GetKey(downKey))
        {
            movement.y--;
        }
        if(movement.magnitude > 0)
        {
            movement = movement.normalized * movementSpeed * Time.deltaTime;
            body.MovePosition(transform.position + movement);
            Vector3 relpos = joint.connectedBody.transform.position - transform.position;
            float angle = Mathf.Abs(Vector3.Angle(relpos, movement));
            joint.distance += movement.magnitude * ((90 - angle) / 90);
        }
        if(Input.GetKey(reelKey))
        {
            joint.distance -= reelSpeed * Time.deltaTime;
        }
        if(Input.GetKey(slackKey) || colliding)
        {
            joint.enabled = false;
            joint.distance = currentDistance;
        }
        if(Input.GetKeyUp(slackKey))
        {
            joint.enabled = true;
        }
    }
}
