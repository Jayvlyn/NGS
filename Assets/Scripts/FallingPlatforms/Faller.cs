using UnityEngine;

public class Faller : MonoBehaviour
{
    public float lifetime;
    public Vector3 velocity;
    public bool destroyGameObject = true;

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime < 0)
        {
            if(destroyGameObject)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.localPosition = Vector3.zero;
                Destroy(this);
            }
        }
        else
        {
            transform.localPosition += velocity * Time.deltaTime;
        }    
    }
}
