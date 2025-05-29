using UnityEngine;

public class VisibilityEnabler : MonoBehaviour
{
    public float lifetime;
    [SerializeField] private float updateDelay = 0.25f;
    private float currentTimer = 0;
    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;
        if(currentTimer > updateDelay)
        {
            MapManager.Instance.UpdateVisibility(transform.position);
            if (lifetime != -1)
            {
                lifetime -= Time.deltaTime;
                if (lifetime <= 0)
                {
                    Destroy(this);
                }
            }
            currentTimer -= updateDelay;
        }
    }
}
