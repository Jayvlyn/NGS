using UnityEngine;
using UnityEngine.UI;

public class MinigameObstacle : MonoBehaviour
{
    [Header("Obstacle references")]
    [SerializeField] float speed;
    [SerializeField] float rotateSpeed = 45;
    [SerializeField] GameObject imageObject;
    [SerializeField] GameObject spawnOnDestroy;
    [SerializeField] bool randomInitialRotation = true;
    private float currentSpeed = 0.0f;
    private float currentRotateSpeed = 0.0f;

    [SerializeField] public FishMinigame fishMinigame { set; get; }

    [Header("Minigame effects")]
    [SerializeField] float speedPenalty;
    [SerializeField] float explosionForce;
    [SerializeField] PlayerStats playerStats;

    void Start()
    {
        currentSpeed = speed;
        if(currentRotateSpeed > 0)
        {
            currentSpeed += Random.Range(-5, 6);
            currentSpeed *= Random.Range(0, 2) == 0 ? 1 : -1;
        }    

        currentRotateSpeed = rotateSpeed;
        if(rotateSpeed > 0)
        {
            currentRotateSpeed += Random.Range(-5, 6);
		    currentRotateSpeed *= Random.Range(0, 2) == 0 ? 1 : -1;
        }

		if(randomInitialRotation) transform.Rotate(0f, 0f, Random.Range(-20, 20));
    }


    void Update()
    {
        if(currentSpeed != 0) MoveObstacle();
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

        transform.Rotate(0f,0f, currentRotateSpeed * Time.deltaTime);

        Vector2 direction = Vector2.right;

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
            if(hb == null) hb = collision.GetComponent<HookBehavior>();
            if (explosionForce > 0)
            {
                if(collision.gameObject.TryGetComponent(out HookBehavior hb))
                {
                    ExplodeObstacle(hb);
                }
            }
            if(speedPenalty > 0) hb.ChangeSpeed(hb.hookResistanceVal * speedPenalty * playerStats.hookStrength);
        }
    }

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Hook")
		{
            if(hb == null) hb = collision.GetComponent<HookBehavior>();
            if(speedPenalty > 0) hb.ResetSpeed();
		}
	}

    private void OnDisable()
    {
        Destroy(gameObject);
    }

    private void ExplodeObstacle(HookBehavior hb)
    {
        // do particle effect
        // do forces
        Vector2 explosionDir = (hb.transform.position - transform.position).normalized;
        Vector2 forceVector = explosionDir * explosionForce * playerStats.hookStrength;
        hb.ApplyImpulse(forceVector);

        if(spawnOnDestroy != null) Instantiate(spawnOnDestroy, transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }
}
