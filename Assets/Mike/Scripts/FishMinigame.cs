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
    [SerializeField] float maxCatchProgress = 100f; // Max catch progress.
    [SerializeField] float wadeSpeed = 0.005f;

	[SerializeField] BoolEvent minigameEvent;

    [SerializeField] float boundsx = 500.0f; // X Bounds
    [SerializeField] float boundsy = 300.0f; // Y Bounds
    [SerializeField] float boundsyoffset = 150.0f; // Y Bounds offset

    //public float currentYBias;
    //private float currentWadeSpeed;
    private float swimAngle = 0;
    private float goalAngle = 0;

    public bool isCaught = false;

    private float catchProgress = 20f; // 100 is win-condition.
    private Vector2 velocity = new Vector3(1, 0, 0);
    private bool hooked;

	private void OnEnable()
	{
		menu.pi.SwitchCurrentActionMap("Minigame");
		//currentYBias = 0.0f;
		//currentSpeed = swimSpeed;
		//currentWadeSpeed = wadeSpeed;
		hooked = false;

		catchProgBar.value = 25.0f;

		fishImage.sprite = hookedFish.sprite;
        UpdateDesiredAngle();
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

        //// Flip the current Wading speed if it reaches one of the bounds
        //if(swimAngle > (0.75f + currentYBias) || swimAngle < (-0.75f + currentYBias))
        //{
        //    currentWadeSpeed *= -1;
        //}
        ////swimAngle += (currentWadeSpeed);
        //swimAngle += currentWadeSpeed > 0 ? Random.Range(-currentSpeed)

        //// Rotate Sprite
        //Vector3 newEuler = new Vector3(0, 0, swimAngle);
        //transform.Rotate(newEuler);

        //Vector2 direction = new Vector2(transform.right.x, transform.right.y);

        //direction.Normalize();
        //direction.x *= currentSpeed;
        //direction.y *= currentSpeed;

        //Vector2 newPosition = new Vector2(transform.position.x + direction.x, transform.position.y + direction.y );
        float panicModifier = hooked ? panicMulti : 1;
        swimAngle = Mathf.Lerp(swimAngle, goalAngle, Time.deltaTime * panicModifier);
        if(swimAngle - goalAngle > -5f && swimAngle - goalAngle < 5f)
        {
            UpdateDesiredAngle();
        }
        velocity = panicModifier * swimSpeed * (Quaternion.Euler(0, 0, swimAngle) * Vector3.right);
        //if(Mathf.Sign(velocity.x) != Mathf.Sign(transform.localScale.x))
        //{
        //    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //}
        gameObject.GetComponent<Rigidbody2D>().MovePositionAndRotation(new Vector2(transform.position.x + velocity.x, transform.position.y + velocity.y), swimAngle);
        
    }

    void FlipFish()
    {
        // Flip Sprite
        //Vector3 scale = transform.localScale;
        //scale.x *= -1;
        //transform.localScale = scale;

        //RandomizeDirectionBias();

        // Ensure fish goes back into bounds
        Vector2 tempPos = transform.localPosition;
        if (tempPos.x > 0) tempPos.x = boundsx-1;
        else tempPos.x = -boundsx+1;
        transform.localPosition = tempPos;

        // Switch Direction
        swimAngle -= 180;
        UpdateDesiredAngle(swimAngle);
    }

    //void RandomizeDirectionBias()
    //{
    //    // Random Y Bias
    //    currentYBias = Random.Range(-0.25f, 0.25f);
    //}

    void KeepUpright()
    {
        float zRotation = transform.rotation.eulerAngles.z;
        while (zRotation > 180)
        {
            zRotation -= 360;
        }
        while (zRotation < -180)
        {
            zRotation += 360;
        }
        if (transform.localScale.y > 0 &&
            (zRotation > 90 || zRotation < -90)
            || (transform.localScale.y < 0 &&
            zRotation < 90 && zRotation > -90))
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
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
        swimAngle -= 180;
        UpdateDesiredAngle(swimAngle);
    }

    void UpdateCatchProg()
    {
        if (hooked)
        {
            catchProgress += 0.01f * catchMulti;
            catchProgBar.value = catchProgress / maxCatchProgress;
        }
        else if(catchProgBar.value > 0)
        {
            catchProgress -= 0.01f * catchMulti;
            catchProgBar.value = catchProgress / maxCatchProgress;
        }
    }
    void CheckIfComplete()
    {
        if (catchProgress >= maxCatchProgress)
        {
            isCaught = true; // Leave minigame WITH reward (Raise Win Event Here)

            Inventory.Instance.AddFish(hookedFish);
            //Debug.Log(Inventory.Instance.ToString());
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
        MenuUI.Instance.LoadMinigame(MenuUI.Instance.transform.Find("Minigame").gameObject);
        //minigameUI.SetActive(false);
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
            //currentSpeed = (currentSpeed > 0) ? swimSpeed * panicMulti : -swimSpeed * panicMulti;
            //currentWadeSpeed = wadeSpeed * panicMulti;
            //RandomizeDirectionBias();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Hook")
        {
            hooked = false;
            //currentSpeed = (currentSpeed > 0) ? swimSpeed : -swimSpeed;
            //currentWadeSpeed = wadeSpeed;
        }
    }

    private void UpdateDesiredAngle()
    {
        goalAngle = Random.Range(goalAngle - wadeSpeed * 1800, goalAngle + wadeSpeed * 1800);
    }
    private void UpdateDesiredAngle(float oldAngle)
    {
        goalAngle = Random.Range(oldAngle - wadeSpeed * 1800, oldAngle + wadeSpeed * 1800);
    }

    public void SetHookedFish(Fish fish)
    {
        hookedFish = fish;
    }
}
