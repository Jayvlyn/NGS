using UnityEngine;

public class GlobalWindSpawner : MonoBehaviour
{
    [SerializeField] private float minLifetime;
    [SerializeField] private float maxLifetime;
    [SerializeField] private float minTimeGap;
    [SerializeField] private float maxTimeGap;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField, Range(-1, 1)] private int direction;
    [SerializeField] private bool alwaysSwapDirection;
    [SerializeField] private bool timeGapIsLifetime;
    [SerializeField] private GameObject windPrefab;
    [SerializeField] private float remainingTimeBeforeSpawn = 0;

    private void Update()
    {
        remainingTimeBeforeSpawn -= Time.deltaTime;
        if(remainingTimeBeforeSpawn < 0)
        {
            SpawnWind();
        }
    }

    private void SpawnWind()
    {
        GameObject go = Instantiate(windPrefab);
        int dir = (int)(direction == 0 ? Mathf.Sign(Random.Range(-1, 1)) : direction);
        go.transform.localScale = new Vector3(dir * go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
        go.GetComponentInChildren<Forcer>().force = new Vector2(Random.Range(minSpeed, maxSpeed) * dir, 0);
        remainingTimeBeforeSpawn = Random.Range(minTimeGap, maxTimeGap);
        go.AddComponent<LifetimeObject>().remainingLife = timeGapIsLifetime ? remainingTimeBeforeSpawn : Mathf.Min(remainingTimeBeforeSpawn, Random.Range(minLifetime, maxLifetime));
        if(alwaysSwapDirection)
        {
            direction = -direction;
        }
    }

    private void Start()
    {
        if(alwaysSwapDirection && direction == 0)
        {
            direction = (int)Mathf.Sign(Random.Range(-1, 1));
        }
    }
}
