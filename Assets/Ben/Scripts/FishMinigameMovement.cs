using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class FishMinigameMovement : MonoBehaviour
{
    [SerializeField] Sprite fishSprite;
    [SerializeField] Slider catchProgBar;
    
    //[SerializeField] ProgressBar catchProgBar;

    [SerializeField] float swimSpeed = 5.0f;
    [SerializeField] float panicMulti = 1.0f;
    [SerializeField] float catchMulti = 1.0f;
    [SerializeField] float wadeSpeed = 0.005f;

    private float swimDir = 1f;

    private float currentWadeSpeed;
    private float swimAngle;

    public bool isCaught = false;

    private float catchProgress = 0.0f; // 100 is win-condition.
    private float currentSpeed;
    private bool hooked;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpeed = swimSpeed;
        currentWadeSpeed = wadeSpeed;
        hooked = false;

        catchProgBar.value = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        MoveFish();
        UpdateCatchProg();
        CheckIfComplete();
    }

    void MoveFish()
    {
        // Wade 'Animation'
        if(swimAngle > 0.5f || swimAngle < -0.5f)
        {
            currentWadeSpeed *= -1;
        }
        swimAngle += currentWadeSpeed;

        // Rotate
        Vector3 newEuler = new Vector3(0, 0, swimAngle);
        transform.Rotate(newEuler);


        Vector2 direction = new Vector2(transform.right.x, transform.right.y);
        direction.Normalize();
        direction.Scale(new Vector2(swimSpeed, swimSpeed));

        Vector2 newPosition = new Vector2(transform.position.x + direction.x, transform.position.y + direction.y );

        gameObject.GetComponent<Rigidbody2D>().MovePosition(newPosition);
    }

    void FlipFish()
    {
        // Flip Sprite
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // May need to flip collision, but should be fine

        // Switch Direction
        swimDir *= -1;
    }



    void UpdateCatchProg()
    {
        if (hooked)
        {
            catchProgress += 0.01f * catchMulti;
            catchProgBar.value = catchProgress;
        }
        else if(catchProgBar.value > 0)
        {
            catchProgress -= 0.01f * catchMulti;
            catchProgBar.value = catchProgress;
        }
    }
    void CheckIfComplete()
    {
        if (catchProgress >= 100.0f) isCaught = true; // Leave minigame WITH reward

        if (catchProgress <= 0.0f) isCaught = false; // Leave minigame without reward
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Hook")
        {
            hooked = true;
            currentSpeed = swimSpeed * panicMulti;
            currentWadeSpeed = wadeSpeed * panicMulti;
        }else if(collision.tag == "FishBorder")
        {
            FlipFish();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Hook")
        {
            hooked = false;
            currentSpeed = swimSpeed;
            currentWadeSpeed = wadeSpeed;
        }
    }
}
