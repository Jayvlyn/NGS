using UnityEngine;

public class FishMinigameMovement : MonoBehaviour
{
    [SerializeField] Sprite fishSprite;
    [SerializeField] float swimSpeed = 5.0f;
    [SerializeField] float panicMulti = 1.0f;
    [SerializeField] float catchMulti = 1.0f;

    private float catchProgress = 0.0f; // 100 is win-condition.
    private float currentSpeed;
    private bool hooked;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpeed = swimSpeed;
        hooked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hooked)
        {
            UpdateCatchProg();
        }else
        {
            catchProgress -= 0.1f;
        }
    }

    void UpdateCatchProg()
    {
        catchProgress += 0.1f * catchMulti;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Hook")
        {
            hooked = true;
            currentSpeed = swimSpeed * panicMulti;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Hook")
        {
            hooked = false;
            currentSpeed = swimSpeed;
        }
    }
}
