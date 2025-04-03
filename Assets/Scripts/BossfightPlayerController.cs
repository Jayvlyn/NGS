using System.Collections.Generic;
using System.Xml.Serialization;
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
    private Rigidbody2D body;
    private DistanceJoint2D joint;
    private List<Collision2D> activeBlockers = new List<Collision2D>();

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
        }
        if(Input.GetKey(reelKey))
        {
            joint.distance -= reelSpeed * Time.deltaTime * Mathf.Pow(joint.distance * 0.2f, 2);
        }
        if(Input.GetKey(slackKey) || activeBlockers.Count > 0)
        {
            joint.enabled = false;
            joint.distance = currentDistance;
        }
        if(Input.GetKeyUp(slackKey))
        {
            joint.enabled = true;
        }
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 relPos = joint.connectedBody.transform.position - transform.position;
        float angle = Vector3.Angle(collision.GetContact(0).normal, relPos);
        if (Mathf.Abs(angle) > 120)
        {
            activeBlockers.Add(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        activeBlockers.Remove(collision);
        joint.enabled = true;
    }

    public float DesiredDistance { get { return joint.distance; } }
}
