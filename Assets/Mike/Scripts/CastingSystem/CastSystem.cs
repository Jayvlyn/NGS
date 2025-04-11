using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CastSystem : MonoBehaviour
{
    [Header("Pop-up & cast bar")]
    [SerializeField] GameObject castScreen;
    [SerializeField] Scrollbar castBar;

    [Header("Cast quality outcome events")]
    [SerializeField] UnityEvent bestOutcome;
    [SerializeField] UnityEvent normalOutcome;
    [SerializeField] UnityEvent worstOutcome;

    private float increment = 0;
    private float increase = 0;
    private float speed = 0;

    [SerializeField] AnimationCurve speedCurve;

    void Start()
    {
        increment = 0.01f;
        increase = 0;
		SpeedVariance(castBar.value);
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ResetCast();
        else if (Input.anyKeyDown) CheckCast(castBar.value);

        SpeedVariance(castBar.value);

        Rebound(castBar.value);

        castBar.value += increment * (Time.deltaTime * (speed + increase));
    }

    private void CheckCast(float value)
    {
        if (value >= 0.48f && value <= 0.52f) bestOutcome.Invoke();
        else if (value >= 0.31f && value <= 0.69f) normalOutcome.Invoke();
        else worstOutcome.Invoke();

        ResetCast();
    }

    private void Rebound(float value)
    {
        if (value >= 1)
        {
            increment = -0.01f;
            increase += (increase != 150) ? 5 : 0;
        }
        else if (value <= 0)
        {
            increment = 0.01f;
            increase += (increase != 150) ? 5 : 0;
        }
    }

    private void SpeedVariance(float value)
    {
        //if (value >= 0.25f && value <= 0.75f) speed = 100;
        //else speed = 50;

        speed = speedCurve.Evaluate(value);
    }

    private void ResetCast()
    {
        increment = 0.01f;
        increase = 0;
        speed = 50;

        castBar.value = 0;
        castScreen.SetActive(false);
    }

}
