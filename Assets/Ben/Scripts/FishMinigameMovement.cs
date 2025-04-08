using Unity.Hierarchy;
using Unity.VisualScripting;
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
    [SerializeField] float catchMulti = 1.0f; // Multiplied Directly to 
    [SerializeField] float wadeSpeed = 0.005f;

    [SerializeField] float YBias = 0.05f;

    private float currentYBias;
    private float currentWadeSpeed;
    private float swimAngle;

    public bool isCaught = false;

    private float catchProgress = 0.0f; // 100 is win-condition.
    private float currentSpeed = 0.0f;
    private bool hooked;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentYBias = 0.0f;
        currentSpeed = swimSpeed;
        currentWadeSpeed = wadeSpeed;
        hooked = false;

        catchProgBar.value = 25.0f;

        gameObject.GetComponent<Image>().sprite = fishSprite;
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
        // Checks

        // Keep the fish within bounds of the minigame
        if  (transform.localPosition.x > 500 || transform.localPosition.x < -500)
        {
            FlipFish();
        }

        if (transform.localPosition.y > 300 || transform.localPosition.y < -450)
        {
            BottomBounce();
        }

        // Use bias to ensure fish doesn't swim off top or bottom of screen
        //if (transform.localPosition.y > 50)
        //{
        //    currentYBias = -YBias - Random.Range(0.0f, 0.25f);
        //    Debug.Log("Negative Bias, try go down");
        //}
        //else if (transform.localPosition.y < -50)
        //{
        //    currentYBias = YBias + Random.Range(0.0f, 0.25f);
        //    Debug.Log("Positve Bias, try go up");
        //}

        // Flip the current Wading speed if it reaches one of the bounds
        if(swimAngle > (0.5f + currentYBias) || swimAngle < (-0.5f + currentYBias))
        {
            currentWadeSpeed *= -1;
        }
        swimAngle += (currentWadeSpeed);

        // Rotate
        Vector3 newEuler = new Vector3(0, 0, swimAngle);
        transform.Rotate(newEuler);

        Vector2 direction = new Vector2(transform.right.x, transform.right.y);

        direction.Normalize();
        direction.x *= currentSpeed;
        direction.y *= currentSpeed;

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
        currentSpeed *= -1;

        Debug.Log("Flip Fish");
    }

    void BottomBounce()
    {
        Vector3 newAngle = transform.localEulerAngles;
        newAngle.z *= -1;

        transform.localEulerAngles = newAngle;


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
        if (catchProgress >= 100.0f) isCaught = true; // Leave minigame WITH reward (Raise Win Event Here)

        if (catchProgress <= 0.0f) isCaught = false; // Leave minigame without reward (Raise Loss Event Here)
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Hook")
        {
            hooked = true;
            currentSpeed = (currentSpeed > 0) ? swimSpeed * panicMulti : -swimSpeed * panicMulti;
            currentWadeSpeed = wadeSpeed * panicMulti;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Hook")
        {
            hooked = false;
            currentSpeed = (currentSpeed > 0) ? swimSpeed : -swimSpeed;
            currentWadeSpeed = wadeSpeed;
        }
    }
}
