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
    [SerializeField] AudioClip cast;
    [SerializeField] AudioClip hookHit;

    private void Start()
    {
        // LOOPING AUDIO SOURCE SHOULD HAVE 'PLAY ON AWAKE' CHECKED ON
        StopLoopingAudioSource();
    }

    public void PlayJumpSound()
    {
        if (jumps == null || jumps.Length < 1) return;
        PlayOneShotAudio(jumps[Random.Range(0, jumps.Length - 1)]);
    }

    public void PlayCastSound()
    {
        if (cast == null) return;
        PlayOneShotAudio(cast);
    }    

    public void PlayLandSound()
    {
        if (lands == null || lands.Length < 1) return;
        PlayOneShotAudio(lands[Random.Range(0, lands.Length - 1)]);
    }

    public void PlaySplashSound()
    {
        if(waterSplash == null) return;
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

    private void PlayOneShotAudio(AudioClip clip)
    {
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
