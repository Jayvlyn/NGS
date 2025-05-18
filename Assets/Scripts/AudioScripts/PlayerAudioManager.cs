using System.Diagnostics;
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
    [SerializeField] AudioClip[] jumps;
    [SerializeField] AudioClip[] lands;
    [SerializeField] AudioClip waterSplash;

    [Header("Fishing Rod")]
    [SerializeField] AudioClip reelLoop;
    [SerializeField] AudioClip slackLoop;
    [SerializeField] AudioClip[] casts;

    private void Start()
    {
        // LOOPING AUDIO SOURCE SHOULD HAVE 'PLAY ON AWAKE' CHECKED ON
        StopLoopingAudioSource();
    }

    public void PlayJumpSound()
    {
        PickNPlayOneShotAudio(jumps);
    }

    public void PlayCastSound()
    {
        PickNPlayOneShotAudio(casts);
    }    

    public void PlayLandSound()
    {
        PickNPlayOneShotAudio(lands);
    }

    public void PlaySplashSound()
    {
        PlayOneShotAudio(waterSplash);
    }

    public void StartRunSound()
    {
        if(runLoop == null) return;
        loopingAudioSource.resource = runLoop;
        StartLoopingAudioSource();
    }

    public void StopRunSound()
    {
        StopLoopingAudioSource();
    }

    public void StartWalkSound()
    {
        if(walkLoop == null) return;
        loopingAudioSource.resource = walkLoop;
        StartLoopingAudioSource();
    }

    public void StopWalkSound()
    {
        StopLoopingAudioSource();
    }

    public void StartWallSlideSound()
    {
        if(wallSlideLoop == null) return;
        loopingAudioSource.pitch = startingWallSlidePitch;
        loopingAudioSource.resource = wallSlideLoop;
        StartLoopingAudioSource();
    }

    public void StopWallSlideSound()
    {
        StopLoopingAudioSource();
    }

    public void StartReelSound()
    {
        if (reelLoop == null) return;
        loopingAudioSource.pitch = startingReelPitch;
        loopingAudioSource.resource = reelLoop;
        StartLoopingAudioSource();
    }

    public void StopReelSound()
    {
        StopLoopingAudioSource();
    }

    public void StartSlackSound()
    {
        if (slackLoop == null) return;
        loopingAudioSource.pitch = startingSlackPitch;
        loopingAudioSource.resource = reelLoop;
        StartLoopingAudioSource();
    }

    public void StopSlackSound()
    {
        StopLoopingAudioSource();
    }

    public void UpdateWallSlidePitchAndVolume(float playerGravityScale)
    {
        UpdateLoopingPitch(playerGravityScale);
        UpdateLoopingVolume(Mathf.Clamp01(playerGravityScale));
    }

    private void UpdateLoopingPitch(float pitch)
    {
        loopingAudioSource.pitch = pitch;
    }

    private void UpdateLoopingVolume(float volume)
    {
        loopingAudioSource.volume = volume;
    }

    private void PlayOneShotAudio(AudioClip clip)
    {
        if (clip == null) return;
        foreach (AudioSource oneShotAudioSource in oneShotAudioSources)
        {
            if (!oneShotAudioSource.isPlaying)
            {
                oneShotAudioSource.resource = clip;
                oneShotAudioSource.Play();
                break;
            }
        }
    }

    private void PickNPlayOneShotAudio(AudioClip[] clips)
    {
        if (clips == null || clips.Length < 1) return;
        PlayOneShotAudio(clips[Random.Range(0, clips.Length - 1)]);
    }

    private void StopLoopingAudioSource()
    {
        loopingAudioSource.Stop();
        loopingAudioSource.enabled = false;
    }

    private void StartLoopingAudioSource()
    {
        if (loopingAudioSource.isPlaying) loopingAudioSource.Stop();
        loopingAudioSource.Play();
        loopingAudioSource.enabled = true;
    }
}
