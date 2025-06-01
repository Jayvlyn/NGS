using System.Collections;
using UnityEngine;

public class GlobalAudioManager : Singleton<GlobalAudioManager>
{
    [Header("Audio Sources")]
    public AudioSource loopingAudioSource;
    public AudioSource[] oneShotAudioSources;

    public void UpdateLoopingPitch(float pitch)
    {
        loopingAudioSource.pitch = pitch;
    }

    public void UpdateLoopingVolume(float volume)
    {
        loopingAudioSource.volume = volume;
    }

    public void PlayOneShotAudio(AudioClip clip, float volume = 1, float pitch = 1)
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

    public void CancelOneShotAudio(AudioClip clip)
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

    public void PickNPlayOneShotAudio(AudioClip[] clips, float volume = 1, float pitch = 1)
    {
        if (clips == null || clips.Length < 1) return;
        PlayOneShotAudio(clips[Random.Range(0, clips.Length - 1)], volume, pitch);
    }

    public void StopLoopingAudioSource()
    {
        if (loopingAudioSource.enabled) StartCoroutine(SmoothStop(loopingAudioSource));
    }

    public void StartLoopingAudioSource(AudioClip clip)
    {
        if (loopingAudioSource.isPlaying && loopingAudioSource.resource == clip) return;
        if (loopingAudioSource.isPlaying) loopingAudioSource.Stop();
        loopingAudioSource.resource = clip;
        loopingAudioSource.Play();
    }

    public bool IsLoopingSourcePlaying()
    {
        return loopingAudioSource.isPlaying;
    }

    public IEnumerator SmoothStop(AudioSource audioSource, float time = 0.15f)
    {
        float t = 0f;
        float initialVolume = audioSource.volume;
        while (t < time)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(initialVolume, 0, t / time);
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = 1;
    }

}
