using UnityEngine;

public class VisibilityEnabler : MonoBehaviour
{
    public float lifetime;
    // Update is called once per frame
    void Update()
    {
        MapManager.Instance.UpdateVisibility(transform.position);
        if (lifetime != -1)
        {
            lifetime -= Time.deltaTime;
            if(lifetime <= 0)
            {
                Destroy(this);
            }
        }
    }
}
