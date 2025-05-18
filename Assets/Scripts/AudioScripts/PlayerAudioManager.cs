using System.Diagnostics;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource loopingAudioSource;
    [SerializeField] AudioSource oneShotAudioSource;

    [Header("Config")]
    [SerializeField] float startingWallSlidePitch = 1.0f;
    [SerializeField] float startingReelPitch = 1.0f;
    [SerializeField] float startingSlackPitch = 1.0f;


    [Header("Movement")]
    [SerializeField] AudioClip runLoop;
    [SerializeField] AudioClip walkLoop; // for hooked movement
    [SerializeField] AudioClip wallSlideLoop;
    [SerializeField] AudioClip[] jumps;
    [SerializeField] AudioClip wallStick;
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
        oneShotAudioSource.resource = jumps[Random.Range(0, jumps.Length-1)];
        oneShotAudioSource.Play();
    }

    public void PlayCastSound()
    {
        if (cast == null) return;
        oneShotAudioSource.resource = cast;
        oneShotAudioSource.Play();
    }    

    public void PlayWallStickSound()
    {
        if (wallStick == null) return;
        oneShotAudioSource.resource = wallStick;
        oneShotAudioSource.Play();
    }

    public void PlaySplashSound()
    {
        if(waterSplash == null) return;
        oneShotAudioSource.resource = waterSplash;
        oneShotAudioSource.Play();
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



    private void StopLoopingAudioSource()
    {
        loopingAudioSource.enabled = false;
    }

    private void StartLoopingAudioSource()
    {
        loopingAudioSource.enabled = true;
    }

}
