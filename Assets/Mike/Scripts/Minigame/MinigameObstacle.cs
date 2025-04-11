using UnityEngine;
using UnityEngine.UI;

public class MinigameObstacle : MonoBehaviour
{
    [Header("Obstacle references")]
    [SerializeField] float speed;
    [SerializeField] Sprite image;
    [SerializeField] GameObject fishMinigame;
    private Vector2 direction;

    [Header("Minigame effects")]
    [SerializeField] float speedPenalty;
    [SerializeField] float reduceAmount;


    void Start()
    {
        this.GetComponent<Image>().sprite = image;
    }


    void Update()
    {
        float randomRotation = Random.Range(25f, 45f);
        gameObject.transform.Rotate(0f, 0f, 15 * Time.deltaTime);
        gameObject.transform.Translate(0, 50 * Time.deltaTime, 0);
        //this.transform.forward.Normalize() += speed;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Hook")
        {
            fishMinigame.GetComponent<FishMinigame>().ReduceProgress(reduceAmount);
            collision.GetComponent<HookBehavior>().ReduceSpeed(speedPenalty);
            OnDisable();
        }
    }

    private void OnDisable()
    {
        Destroy(this.gameObject);
    }
}
