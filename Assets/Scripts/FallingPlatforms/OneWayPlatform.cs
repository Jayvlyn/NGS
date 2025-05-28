using UnityEngine;
[RequireComponent(typeof(Collider2D))]

public class OneWayPlatform : MonoBehaviour
{
    public Collider2D colliderToDisable;
    private int currentObjects = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentObjects++;
        colliderToDisable.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentObjects--;
        if(currentObjects == 0)
        {
            colliderToDisable.enabled = true;
        }
    }
}
