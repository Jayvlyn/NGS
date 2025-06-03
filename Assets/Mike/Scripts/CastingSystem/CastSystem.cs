using GameEvents;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CastSystem : MonoBehaviour
{
    [SerializeField] WaterEvent onCastFinished;

	[Header("Pop-up & cast bar")]
    [SerializeField] GameObject castScreen;
    [SerializeField] Scrollbar castBar;

    [Header("Cast quality outcome events")]
    [SerializeField] UnityEvent bestOutcome;
    [SerializeField] UnityEvent normalOutcome;
    [SerializeField] UnityEvent worstOutcome;

    [Header("Audio")]
    [SerializeField] AudioClip castBarTone;
    [SerializeField] AudioClip bestHit;
    [SerializeField] AudioClip normalHit;
    [SerializeField] AudioClip badHit;

    private float increment = 0;
    private float increase = 0;
    private float speed = 0;
    private float startDelay;
    private bool moving = false;

    private Water water;

    [SerializeField] AnimationCurve speedCurve;

	private void OnEnable()
	{
		GameUI.Instance.pi.SwitchCurrentActionMap("Minigame");
		increment = 0.01f;
		increase = 0;
        startDelay = 0.2f;
        moving = true;
        GlobalAudioManager.Instance.StartLoopingAudioSource(castBarTone);
        GlobalAudioManager.Instance.UpdateLoopingVolume(0.5f);
	}

	private void OnDisable()
	{
        moving = false;
        GlobalAudioManager.Instance.StopLoopingAudioSource();
        //GameUI.Instance.pi.SwitchCurrentActionMap("Platformer");
	}

	void Update()
    {
        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
        }
        else if (moving)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
				GameUI.Instance.pi.SwitchCurrentActionMap("Platformer");
                ResetCast();
			}
            else if (Input.anyKeyDown)
            {
                CheckCast(castBar.value);
		    }
        }

        if(moving) SpeedVariance(castBar.value);

        Rebound(castBar.value);

        float minPitch = 0.8f;
        float maxPitch = 1.2f;
        float foldedValue = 1f - Mathf.Abs(castBar.value - 0.5f) * 2f;
        GlobalAudioManager.Instance.UpdateLoopingPitch(Mathf.Lerp(minPitch, maxPitch, foldedValue));

        if(moving) castBar.value += increment * (Time.deltaTime * (speed + increase));
    }

    private void CheckCast(float value)
    {
        GlobalAudioManager.Instance.StopLoopingAudioSource();
        if (value >= 0.48f && value <= 0.52f)
        {
            GlobalAudioManager.Instance.PlayOneShotAudio(bestHit);
            bestOutcome.Invoke();
        }
        else if (value >= 0.31f && value <= 0.69f)
        {
            GlobalAudioManager.Instance.PlayOneShotAudio(normalHit);
            normalOutcome.Invoke();
        }
        else
        {
            GlobalAudioManager.Instance.PlayOneShotAudio(badHit);
            worstOutcome.Invoke();
        }

        onCastFinished.Raise(water);

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
        StartCoroutine(ResetOnDelay());
        GameUI.Instance.LoadMinigame(GameUI.Instance.transform.Find("CastUI").gameObject);
		//castScreen.SetActive(false);
	}

    private IEnumerator ResetOnDelay()
    {
        moving = false;
        speed = 0;
        yield return new WaitForSeconds(3);
		increment = 0.01f;
		increase = 0;
		speed = 50;

		castBar.value = 0;
	}

    public void SetWater(Water water)
    {
        this.water = water;
    }

}
