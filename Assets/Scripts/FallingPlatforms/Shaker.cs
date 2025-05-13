using UnityEngine;

public class Shaker : MonoBehaviour
{
    public float shakeAggression;
    public float lifetime;
    public bool destroyGameobject = false;

    private void Update()
    {
        transform.localPosition = new Vector3(Random.Range(-shakeAggression, shakeAggression), Random.Range(-shakeAggression, shakeAggression));
        lifetime -= Time.deltaTime;
        if(lifetime <= 0)
        {
            if(destroyGameobject)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.localPosition = Vector3.zero;
                Destroy(this);
            }
        }    
    }
}
