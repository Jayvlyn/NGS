using GameEvents;
using UnityEngine;
using UnityEngine.UI;

public class FishMinigame : MonoBehaviour
{
    [SerializeField] MenuUI menu;
    private Fish hookedFish;
    [SerializeField] Slider catchProgBar;
    [SerializeField] Image fishImage;
    [SerializeField] GameObject minigameUI;

    [SerializeField] float swimSpeed = 5.0f;
    [SerializeField] float panicMulti = 1.0f;
    [SerializeField] float catchMulti = 1.0f; // Multiplied Directly to catchProgress increment per update
    [SerializeField] float wadeSpeed = 0.005f;

	[SerializeField] BoolEvent minigameEvent;

    [SerializeField] float boundsx = 500.0f; // X Bounds
    [SerializeField] float boundsy = 300.0f; // Y Bounds
    [SerializeField] float boundsyoffset = 150.0f; // Y Bounds offset

    public float currentYBias;
    private float currentWadeSpeed;
    private float swimAngle;

    public bool isCaught = false;

    private float catchProgress = 20f; // 100 is win-condition.
    private float currentSpeed = 0.0f;
    private bool hooked;

	private void OnEnable()
	{
		menu.pi.SwitchCurrentActionMap("Minigame");
		currentYBias = 0.0f;
		currentSpeed = swimSpeed;
		currentWadeSpeed = wadeSpeed;
		hooked = false;

		catchProgBar.value = 25.0f;

		fishImage.sprite = hookedFish.sprite;
	}

	void FixedUpdate()
    {
        MoveFish();
        KeepUpright();
        UpdateCatchProg();
    }

	private void Update()
	{
        CheckIfComplete();
	}

	void MoveFish()
    {
        // Keep the fish within bounds of the minigame
        if  ((transform.localPosition.x >= boundsx || transform.localPosition.x <= -boundsx))
        {
            FlipFish();
            return;
        }

        if ((transform.localPosition.y >= boundsy || transform.localPosition.y <= -boundsy-boundsyoffset))
        {
            BottomTopBounce();
            return;
        }

        // Flip the current Wading speed if it reaches one of the bounds
        if(swimAngle > (0.75f + currentYBias) || swimAngle < (-0.75f + currentYBias))
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

        RandomizeDirectionBias();

        // Ensure fish goes back into bounds
        Vector2 tempPos = transform.localPosition;
        if (tempPos.x > 0) tempPos.x = boundsx-1;
        else tempPos.x = -boundsx+1;
        transform.localPosition = tempPos;

        // Switch Direction
        currentSpeed *= -1;

    }

    void RandomizeDirectionBias()
    {
        // Random Y Bias
        currentYBias = Random.Range(-0.25f, 0.25f);
    }

    void KeepUpright()
    {
        // Decides based on the scale, when to flip the Y scale, to keep the sprite upright
        if ((transform.localEulerAngles.z > 90 && transform.localEulerAngles.z < 270) && transform.localScale.y == 1)
        {
            FlipYScale();
        }
        else if ((transform.localEulerAngles.z < 90 || transform.localEulerAngles.z > 270) && transform.localScale.y == -1)
        {
            FlipYScale();
        }
    }

    void FlipYScale()
    {
        Vector3 tempScale = transform.localScale;
        tempScale.y *= -1;
        transform.localScale = tempScale;
    }

    void BottomTopBounce()
    {
        // Reflect Angle for bounce
        Vector3 newAngle = transform.localEulerAngles;
        newAngle.z *= -1;
        transform.localEulerAngles = newAngle;

        // Ensure fish goes back into bounds
        Vector2 tempPos = transform.localPosition;
        if (tempPos.y > 0) tempPos.y = boundsy-1;
        else tempPos.y = -boundsy-boundsyoffset+1;
        transform.localPosition = tempPos;
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

            Inventory.Instance.AddFish(hookedFish);
            Debug.Log(Inventory.Instance.ToString());
            OnFinish();
        }
        else if (catchProgress <= 0.0f || Input.GetKeyDown(KeyCode.Escape))
        {
            isCaught = false; // Leave minigame without reward (Raise Loss Event Here)

            OnFinish();
        }
    }

	private void OnFinish()
	{
		minigameEvent.Raise(isCaught);
		menu.pi.SwitchCurrentActionMap("Platformer");
		catchProgress = 20f;
		minigameUI.SetActive(false);
	}

	public void ReduceProgress(float amount)
    {
        catchProgBar.value -= amount;
        catchProgress -= amount;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Hook")
        {
            hooked = true;
            currentSpeed = (currentSpeed > 0) ? swimSpeed * panicMulti : -swimSpeed * panicMulti;
            currentWadeSpeed = wadeSpeed * panicMulti;
            RandomizeDirectionBias();
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

    public void SetHookedFish(Fish fish)
    {
        hookedFish = fish;
    }
}
