using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class ModifySettings : MonoBehaviour
{
    public GameSettings settings;
    public AudioMixer mixer;

    public TMP_Dropdown resolutionDropdown;

    public Volume postProcessing;

    Resolution[] resolutions;

    void Start()
    {
        //TODO: Switch to scriptible object later on

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            float aspect = (float)resolutions[i].width / resolutions[i].height;
            if (Mathf.Abs(aspect - (16f / 9f)) > 0.01f) continue;

            string option = resolutions[i].width + "x" + resolutions[i].height;

            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetMasterVolume(float volume)
    {
        if (volume == -45) volume = -80;
        mixer.SetFloat("MasterVol", volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume == -45) volume = -80;
        mixer.SetFloat("MusicVol", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (volume == -45) volume = -80;
        mixer.SetFloat("SFXVol", volume);
    }

    public void SetResolution(int resIndex)
    {
        Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetPostProcessing(bool isPostProcessingOn)
    {
        postProcessing.enabled = isPostProcessingOn;
    }
}
