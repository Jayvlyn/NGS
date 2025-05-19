using System.Collections;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource loopingAudioSource;
    [SerializeField] AudioSource[] oneShotAudioSources;

    [Header("Config")]
    [SerializeField] float startingWallSlidePitch = 1.0f;
    [SerializeField] float startingReelPitch = 1.0f;
    [SerializeField] float startingSlackPitch = 1.0f;

    [Header("Movement")]
    [SerializeField] AudioClip runLoop;
    [SerializeField] AudioClip walkLoop; // for hooked movement
    [SerializeField] AudioClip wallSlideLoop;
    [SerializeField] AudioClip jump;
    [SerializeField] AudioClip[] lands;
    [SerializeField] AudioClip waterSplash;

    [Header("Fishing Rod")]
    [SerializeField] AudioClip reelLoop;
    [SerializeField] AudioClip[] casts;

    private void Start()
    {
        // LOOPING AUDIO SOURCE SHOULD HAVE 'PLAY ON AWAKE' CHECKED ON
        StopLoopingAudioSource();
    }

    public void PlayJumpSound()
    {
        PlayOneShotAudio(jump);
    }

    public void StopJumpSound()
    {
        CancelOneShotAudio(jump);
    }

    public void PlayCastSound()
    {
        PickNPlayOneShotAudio(casts);
    }    

    public void PlayLandSound()
    {
        PickNPlayOneShotAudio(lands, 0.5f);
    }

    public void PlaySplashSound()
    {
        PlayOneShotAudio(waterSplash);
    }

    public void StartRunSound()
    {
        if(runLoop == null) return;
        UpdateLoopingVolume(1);
        UpdateLoopingPitch(1);
        if(stopRun != null) StopCoroutine(stopRun);
        StartLoopingAudioSource(runLoop);
    }

    public void StopRunSound()
    {
        if (loopingAudioSource.enabled)
        {
            stopRun = StartCoroutine(SmoothStop(loopingAudioSource));
        }
    }

    public void StartWalkSound()
    {
        if(walkLoop == null) return;
        UpdateLoopingVolume(1);
        UpdateLoopingPitch(1);
        StartLoopingAudioSource(walkLoop);
    }

    public void StopWalkSound()
    {
        StopLoopingAudioSource();
    }

    public void StartWallSlideSound()
    {
        if(wallSlideLoop == null) return;
        UpdateLoopingVolume(0);
        UpdateLoopingPitch(0);
        StartLoopingAudioSource(wallSlideLoop);
    }

    public void StopWallSlideSound()
    {
        StopLoopingAudioSource();
    }

    public void StartReelSound()
    {
        if (reelLoop == null) return;
        UpdateLoopingVolume(0);
        UpdateLoopingPitch(0);
        loopingAudioSource.pitch = startingReelPitch;
        StartLoopingAudioSource(reelLoop);
    }

    public void StopReelSound()
    {
        StopLoopingAudioSource();
    }

    public bool IsReelSoundPlaying()
    {
        return loopingAudioSource.resource == reelLoop && loopingAudioSource.isPlaying;
    }

    public void UpdateWallSlideSound(float playerGravityScale)
    {
        UpdateLoopingPitch(playerGravityScale);
        UpdateLoopingVolume(Mathf.Clamp01(playerGravityScale / 2));
    }


    public int reelLowEnd = 2;
    public int reelHighEnd = 10;
    public void UpdateReelSound(float playerSpeed)
    {
        Debug.Log("PlayerSpeed: " + playerSpeed);

        float t = Mathf.InverseLerp(2, 10, playerSpeed) + 1;
        Debug.Log("t: " + t);
        //Debug.Log("t clamped: " + t);

        UpdateLoopingPitch(t);
        UpdateLoopingVolume(1);
    }

    public void UpdatSlackSound(float playerSpeed)
    {
        float t = Mathf.InverseLerp(2, 10, playerSpeed);
        t = Mathf.Clamp01(t);

        UpdateLoopingPitch(1 / t);
        UpdateLoopingVolume(1);
    }

    private void UpdateLoopingPitch(float pitch)
    {
        loopingAudioSource.pitch = pitch;
    }

    private void UpdateLoopingVolume(float volume)
    {
        loopingAudioSource.volume = volume;
    }

    private void PlayOneShotAudio(AudioClip clip, float volume = 1 , float pitch = 1)
    {
        if (clip == null) return;
        foreach (AudioSource oneShotAudioSource in oneShotAudioSources)
        {
            if (!oneShotAudioSource.isPlaying)
            {
                oneShotAudioSource.volume = volume;
                oneShotAudioSource.pitch = pitch;
                oneShotAudioSource.resource = clip;
                oneShotAudioSource.Play();
                break;
            }
        }
    }

    private void CancelOneShotAudio(AudioClip clip)
    {
        if(clip == null) return;
        foreach (AudioSource oneShotAudioSource in oneShotAudioSources)
        {
            if (oneShotAudioSource.isPlaying && oneShotAudioSource.resource == clip)
            {
                StartCoroutine(SmoothStop(oneShotAudioSource));
                break;
            }
        }
    }

    private void PickNPlayOneShotAudio(AudioClip[] clips, float volume = 1, float pitch = 1)
    {
        if (clips == null || clips.Length < 1) return;
        PlayOneShotAudio(clips[Random.Range(0, clips.Length - 1)], volume, pitch);
    }

    private void StopLoopingAudioSource()
    {
        if(loopingAudioSource.enabled) StartCoroutine(SmoothStop(loopingAudioSource));
    }

    private void StartLoopingAudioSource(AudioClip clip)
    {
        if (stopRun != null)
        {
            StopCoroutine(stopRun);
            stopRun = null;
        }
        if (loopingAudioSource.isPlaying) loopingAudioSource.Stop();
        loopingAudioSource.resource = clip;
        loopingAudioSource.Play();
    }

    private Coroutine stopRun;
    private IEnumerator SmoothStop(AudioSource audioSource, float time = 0.15f)
    {
        float t = 0f;
        float initialVolume = audioSource.volume;
        while(t < time)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(initialVolume, 0, t / time);
            yield return null;
        }
        if (stopRun != null) stopRun = null;
        audioSource.Stop();
        audioSource.volume = 1;
    }

}
