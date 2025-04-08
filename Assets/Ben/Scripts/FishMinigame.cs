using GameEvents;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class FishMinigame : MonoBehaviour
{
    [SerializeField] Sprite fishSprite;
    [SerializeField] Slider catchProgBar;

    [SerializeField] float swimSpeed = 5.0f;
    [SerializeField] float panicMulti = 1.0f;
    [SerializeField] float catchMulti = 1.0f; // Multiplied Directly to catchProgress increment per update
    [SerializeField] float wadeSpeed = 0.005f;

    public BoolEvent minigameEvent;

    public float currentYBias;
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

        minigameEvent = new BoolEvent();

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
        if  ((transform.localPosition.x >= 500 || transform.localPosition.x <= -500))
        {
            FlipFish();
            return;
        }

        if ((transform.localPosition.y >= 300 || transform.localPosition.y <= -450))
        {
            BottomTopBounce();
            return;
        }

        // Flip the current Wading speed if it reaches one of the bounds
        if(swimAngle > (0.5f + currentYBias) || swimAngle < (-0.5f + currentYBias))
        {
            currentWadeSpeed *= -1;
        }
        swimAngle += (currentWadeSpeed);

        // Rotate Sprite
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

        // Random Y Bias
        currentYBias = Random.Range(-0.05f, 0.05f);

        // Ensure fish goes back into bounds
        Vector2 tempPos = transform.localPosition;
        if (tempPos.x > 0) tempPos.x = 499;
        else tempPos.x = -499;
        transform.localPosition = tempPos;

        // Switch Direction
        currentSpeed *= -1;

        Debug.Log("Flip Fish");
    }

    void BottomTopBounce()
    {
        // Reflect Angle for bounce
        Vector3 newAngle = transform.localEulerAngles;
        newAngle.z *= -1;
        transform.localEulerAngles = newAngle;

        // Ensure fish goes back into bounds
        Vector2 tempPos = transform.localPosition;
        if (tempPos.y > 0) tempPos.y = 299;
        else tempPos.y = -449;
        transform.localPosition = tempPos;

        Debug.Log("Bottom Top Bounce");
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
        if (catchProgress >= 100.0f)
        {
            isCaught = true; // Leave minigame WITH reward (Raise Win Event Here)
            minigameEvent.Raise(isCaught);
        }

        if (catchProgress <= 0.0f)
        {
            isCaught = false; // Leave minigame without reward (Raise Loss Event Here)
            minigameEvent.Raise(isCaught);
        }
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
