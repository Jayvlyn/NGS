using GameEvents;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(FishRotator))]
public class FishMinigame : MonoBehaviour
{
    private Fish hookedFish;
    [SerializeField] Slider catchProgBar;
    [SerializeField] Image fishImage;
    [SerializeField] GameObject minigameUI;
    [SerializeField] HookingEffect[] hookingEffects;

    [SerializeField] float swimSpeed = 5.0f;
    [SerializeField] float panicMulti = 1.0f;
    [SerializeField] float catchMulti = 1.0f; // Multiplied Directly to catchProgress increment per update
    [SerializeField] float depleteMulti = 1.0f; // Multiplied Directly to catchProgress increment per update
    [SerializeField] float maxCatchProgress = 100f; // Max catch progress.
    [SerializeField] float wadeSpeed = 0.005f;

	[SerializeField] BoolEvent minigameEvent;
	[SerializeField] FishEvent caughtFishEvent;

    [SerializeField] float boundsx = 500.0f; // X Bounds
    [SerializeField] float boundsy = 300.0f; // Y Bounds
    [SerializeField] float boundsyoffset = 150.0f; // Y Bounds offset

    [SerializeField] Transform bobberT;
    [SerializeField] Transform hookT;
    [SerializeField] Transform fishT;
    private Vector2 initialBobberPos;
    private Vector2 initialHookPos;
    private Vector2 initialFishPos;

    //public float currentYBias;
    //private float currentWadeSpeed;
    private float swimAngle = 0;
    private float goalAngle = 0;

    public bool isCaught = false;
    private bool isFinishing = false;

    [SerializeField] private float startCatchProgress = 10f;
    private float catchProgress; // 100 is win-condition.
    private Vector2 velocity = new Vector3(1, 0, 0);
    private bool hooked;

    private Rigidbody2D rb;
    [SerializeField] private FishRotator rotator;

	private void Start()
	{
		initialBobberPos = bobberT.localPosition;
		initialHookPos = hookT.localPosition;
		initialFishPos = fishT.localPosition;
	}

	private void OnEnable()
	{
		GameUI.Instance.pi.SwitchCurrentActionMap("Minigame");
		//currentYBias = 0.0f;
		//currentSpeed = swimSpeed;
		//currentWadeSpeed = wadeSpeed;
		hooked = false;
        isCaught = false;
        isFinishing = false;

        catchProgress = startCatchProgress;
        catchProgBar.value = catchProgress;

		fishImage.sprite = hookedFish.sprite;
        UpdateDesiredAngle();
        rb = GetComponent<Rigidbody2D>();
	}

	private void OnDisable()
	{
        catchProgress = startCatchProgress;
        bobberT.localPosition = initialBobberPos;
        hookT.localPosition = initialHookPos;
        fishT.localPosition = initialFishPos;
	}

	void FixedUpdate()
    {
        MoveFish();
        KeepUpright();
        UpdateCatchProg();
    }

	private void Update()
	{
        if(!isCaught && !isFinishing) CheckIfComplete();
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
        float panicModifier = hooked ? panicMulti : 1;
        swimAngle = Mathf.Lerp(swimAngle, goalAngle, Time.deltaTime * panicModifier);
        if(swimAngle - goalAngle > -5f && swimAngle - goalAngle < 5f)
        {
            UpdateDesiredAngle();
        }
        velocity = panicModifier * swimSpeed * (Quaternion.Euler(0, 0, swimAngle) * Vector3.right);
        rotator.BaseRotation = Quaternion.Euler(0, 0, swimAngle);
        rb.MovePosition(new Vector2(transform.position.x + velocity.x, transform.position.y + velocity.y));
        
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
            catchProgress -= 0.01f * depleteMulti;
            catchProgBar.value = catchProgress / maxCatchProgress;
        }


		foreach (HookingEffect hookingEffect in hookingEffects)
		{
			hookingEffect.SetTargetScale(catchProgress / maxCatchProgress);
		}
    }
    void CheckIfComplete()
    {
        if (catchProgress >= maxCatchProgress)
        {
            isFinishing = true;
            isCaught = true; // Leave minigame WITH reward (Raise Win Event Here)

			caughtFishEvent.Raise(hookedFish);
            Inventory.Instance.AddFish(hookedFish);
			//Debug.Log(Inventory.Instance.ToString());
			OnFinish();
        }
        else if (catchProgress <= 0.0f || Input.GetKeyDown(KeyCode.Escape))
        {
            isFinishing = true;
            isCaught = false; // Leave minigame without reward (Raise Loss Event Here)

            OnFinish();
        }
    }

	private void OnFinish()
	{
		minigameEvent.Raise(isCaught);
		GameUI.Instance.pi.SwitchCurrentActionMap("Platformer");
        GameUI.Instance.LoadMinigame(GameUI.Instance.transform.Find("Minigame").gameObject);
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
            foreach(HookingEffect hookingEffect in hookingEffects)
            {
                hookingEffect.OnHooked();
            }
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
			foreach (HookingEffect hookingEffect in hookingEffects)
			{
				hookingEffect.OnUnhooked();
			}
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
