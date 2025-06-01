using System.Collections;
using UnityEngine;

public class GlobalAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource loopingAudioSource;
    [SerializeField] AudioSource[] oneShotAudioSources;

    private void UpdateLoopingPitch(float pitch)
    {
        loopingAudioSource.pitch = pitch;
    }

    private void UpdateLoopingVolume(float volume)
    {
        loopingAudioSource.volume = volume;
    }

    private void PlayOneShotAudio(AudioClip clip, float volume = 1, float pitch = 1)
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
        if (clip == null) return;
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
        if (loopingAudioSource.enabled) StartCoroutine(SmoothStop(loopingAudioSource));
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
        while (t < time)
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
