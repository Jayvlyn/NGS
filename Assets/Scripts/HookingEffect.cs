using UnityEngine;

public class HookingEffect : MonoBehaviour
{
    public float growInSpeed = 1f;
    public float shrinkOutSpeed = 1f;
    public float rotateSpeed = 100f;
    public float initialOffset = 0f;
    public float scaleScaler = 1;
    private float t;
    private Vector3 initialScale;
    private Vector3 targetScale;
    [SerializeField] AudioClip reelAudio;

    public enum State
    {
        GROWING_IN,
        SHRINKING_OUT,
        OFF,
        ON
    }
    public State currentState;

	private void OnEnable()
	{
		ChangeState(State.OFF);
        transform.rotation = Quaternion.Euler(0, 0, initialOffset);
	}

    private void OnDisable()
    {
        if (reelAudio != null) GlobalAudioManager.Instance.StopLoopingAudioSource();    
    }

    public void ChangeState(State state)
    {
        switch (state)
        {
            case State.GROWING_IN:
                if(reelAudio!=null && currentState == State.OFF) GlobalAudioManager.Instance.StartLoopingAudioSource(reelAudio);
                t = 0;
                initialScale = transform.localScale;
                break;

            case State.SHRINKING_OUT:
                t = 0;
                initialScale = transform.localScale;
                break;

            case State.OFF:
                if (reelAudio != null) GlobalAudioManager.Instance.StopLoopingAudioSource();
                transform.localScale = Vector3.zero;
                break;

            case State.ON:
                break;
        }
        currentState = state;
    }

	private void Update()
	{
		switch (currentState)
		{
			case State.GROWING_IN:
                ProcessGrowingInState();
                if (reelAudio != null) GlobalAudioManager.Instance.UpdateLoopingPitch(Mathf.Lerp(0.8f, 1.2f, transform.localScale.x));
                break;
			case State.SHRINKING_OUT:
                ProcessShrinkingOutState();
                if (reelAudio != null) GlobalAudioManager.Instance.UpdateLoopingPitch(Mathf.Lerp(0.8f, 1.2f, transform.localScale.x));
                break;
			case State.OFF:
				break;
			case State.ON:
                if (reelAudio != null) GlobalAudioManager.Instance.UpdateLoopingPitch(Mathf.Lerp(0.8f, 1.2f, transform.localScale.x));
                transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
                transform.localScale = targetScale;
				break;
		}
	}

    /// <summary>
    /// Set the target size for the effect based on the cast progress
    /// </summary>
    /// <param name="catchProgressPercent">0-1 (0.5 = halfway)</param>
    public void SetTargetScale(float catchProgressPercent)
    {
        float scale = catchProgressPercent * scaleScaler;

		targetScale = new Vector3(scale, scale, scale);
    }

	public void ProcessGrowingInState()
    {
		if(t < 1)
		{
			transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
			t += Time.deltaTime * growInSpeed;
			transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
		}
        else
        {
            ChangeState(State.ON);
        }
	}

    public void ProcessShrinkingOutState() 
    {
        if(t < 1)
        {
            transform.Rotate(Vector3.forward, -rotateSpeed * Time.deltaTime);
            t += Time.deltaTime * shrinkOutSpeed;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
        }
        else
        {
            ChangeState(State.OFF);
        }
             
    }

    public void OnHooked()
    {
        ChangeState(State.GROWING_IN);
    }

    public void OnUnhooked()
    {
        ChangeState(State.SHRINKING_OUT);
    }
}
