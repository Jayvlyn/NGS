using Unity.VisualScripting;
using UnityEngine;

public class InteractionTestController : Interactor
{
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Attempted interaction");
            TryInteract();
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = new Vector3(transform.position.x + Time.deltaTime, transform.position.y);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x - Time.deltaTime, transform.position.y);
        }
    }
}
