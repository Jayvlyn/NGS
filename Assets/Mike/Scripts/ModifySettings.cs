using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class ModifySettings : MonoBehaviour
{
    public GameSettings settings;
    public AudioMixer mixer;

    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    void Start()
    {
        //TODO: when switched to scriptible object load on start

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
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
        mixer.SetFloat("MasterVol", volume);
    }

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("MusicVol", volume);
    }

    public void SetSFXVolume(float volume)
    {
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
}
