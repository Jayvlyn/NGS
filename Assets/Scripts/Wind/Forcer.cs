using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Forcer : MonoBehaviour
{
    public Vector2 force;
    public float movementModifier = 0;
    public bool checkForFalling = false;
    public bool destroyOnFall = false;
    [SerializeField] private Transform fallCheckPosition;
    [SerializeField] private LayerMask fallCheckLayers;
    [SerializeField] private float fallCheckDistance;
    [SerializeField] private Rigidbody2D body;

    private void Start()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        if(checkForFalling && IsAboutToFall())
        {
            if (destroyOnFall)
            {
                Destroy(this);
            }
            else
            {
                force *= -1;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                if(IsAboutToFall())
                {
                    movementModifier = 0;
                }
                else
                {
                    Move();
                }
            }
        }
        else
        {
            Move();
        }
    }

    private void Move()
    {
        body.AddForce(force * movementModifier * Time.deltaTime);
    }

    private bool IsAboutToFall()
    {
        return !Physics2D.Raycast(fallCheckPosition.position, Vector2.down, fallCheckDistance, fallCheckLayers);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null && collision.attachedRigidbody != null)
        {
            collision.attachedRigidbody.AddForce(force * Time.deltaTime, ForceMode2D.Force);
        }
    }

    private void OnDrawGizmos()
    {
        if(fallCheckPosition != null && fallCheckDistance > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(fallCheckPosition.position, new Vector3(fallCheckPosition.position.x, fallCheckPosition.position.y - fallCheckDistance, fallCheckPosition.position.z));
        }
    }
}
