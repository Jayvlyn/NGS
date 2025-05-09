using UnityEngine;
using UnityEngine.UI;

public class MinigameObstacle : MonoBehaviour
{
    [Header("Obstacle references")]
    [SerializeField] float speed;
    [SerializeField] Sprite image;
    [SerializeField] GameObject imageObject;
    private float currentSpeed = 0.0f;

    [SerializeField] public FishMinigame fishMinigame { set; get; }

    [Header("Minigame effects")]
    [SerializeField] float speedPenalty;
    [SerializeField] float reduceAmount;


    void Start()
    {
        imageObject.GetComponent<Image>().sprite = image;
        currentSpeed = speed;
        transform.Rotate(0f, 0f, 20f);
    }


    void Update()
    {
        MoveObstacle();
    }

    #region SwimLogic

    void MoveObstacle()
    {
        // Checks

        // Keep the fish within bounds of the minigame
        if ((transform.localPosition.x >= 500 || transform.localPosition.x <= -500))
        {
            BounceOff();
            return;
        }

        if ((transform.localPosition.y >= 300 || transform.localPosition.y <= -450))
        {
            BounceOff(true);
            return;
        }

        imageObject.transform.Rotate(0f,0f, 45f * Time.deltaTime);

        Vector2 direction = new Vector2(transform.right.x, transform.right.y);

        direction.Normalize();
        direction.x *= currentSpeed * Time.deltaTime;
        direction.y *= currentSpeed * Time.deltaTime;

        Vector2 newPosition = new Vector2(transform.localPosition.x + direction.x, transform.localPosition.y + direction.y);

        transform.localPosition = newPosition;
    }

    void BounceOff(bool top = false)
    {
        Vector3 newAngle = transform.localEulerAngles;
        newAngle.z *= -1;
        transform.localEulerAngles = newAngle;

        // Ensure fish goes back into bounds
        if (top)
        {
            Vector2 tempPos = transform.localPosition;
            if (tempPos.y > 0) tempPos.y = 299;
            else tempPos.y = -449;
            transform.localPosition = tempPos;
        }
        else
        {
            Vector2 tempPos = transform.localPosition;
            if (tempPos.x > 0) tempPos.x = 499;
            else tempPos.x = -499;
            transform.localPosition = tempPos;
            
            // Switch Direction
            currentSpeed *= -1;
        }

        //Debug.Log("Bounce off bounds");
    }

    #endregion

    HookBehavior hb;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Hook")
        {
            //fishMinigame.ReduceProgress(reduceAmount);
            if(hb == null) hb = collision.GetComponent<HookBehavior>();
            hb.ChangeSpeed(hb.hookResistanceVal * 2);
        }
    }

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Hook")
		{
            if(hb == null) hb = collision.GetComponent<HookBehavior>();
            hb.ResetSpeed();
		}
	}

	private void OnDisable()
	{
		Destroy(gameObject);
	}

}
