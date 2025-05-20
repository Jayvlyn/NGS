using UnityEngine;

public class LifetimeObject : MonoBehaviour
{
    public float remainingLife = 0;
    public int layersToTravelUpBeforeDestruction = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        remainingLife -= Time.deltaTime;
        if(remainingLife <= 0)
        {
            GameObject go = gameObject;
            for(int i = 0; i < layersToTravelUpBeforeDestruction && go.transform.parent != null; i++)
            {
                go = go.transform.parent.gameObject;
            }
            Destroy(go);
        }
    }
}
