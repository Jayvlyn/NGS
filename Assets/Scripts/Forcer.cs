using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Forcer : MonoBehaviour
{
    [SerializeField] private Vector2 force;
    [SerializeField] private float movementModifier;
    [SerializeField] private bool checkForFalling;
    [SerializeField] private bool destroyOnFall;
    [SerializeField] private Transform fallCheckPosition;
    [SerializeField] private LayerMask fallCheckLayers;
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
                transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
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
        return !Physics2D.Raycast(fallCheckPosition.position, Vector2.down, fallCheckPosition.position.y - transform.position.y, fallCheckLayers);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.attachedRigidbody?.AddForce(force * Time.deltaTime);
    }
}
