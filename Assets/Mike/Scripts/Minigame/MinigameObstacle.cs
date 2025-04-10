using UnityEngine;

public class MinigameObstacle : MonoBehaviour
{
    [Header("Obstacle properties")]
    [SerializeField] Sprite image;
    [SerializeField] float floatSpeed;
    [SerializeField] float rotationSpeed;
    private float direction;

    [Header("Minigame effects")]
    [SerializeField] float timeCaught;
    [SerializeField] float speedPenalty;
    [SerializeField] float reduceAmount;


    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
