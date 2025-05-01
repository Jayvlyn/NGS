using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ModifySettings : MonoBehaviour
{
    public GameSettings settings;
    public AudioMixer mixer;

    public TMP_Dropdown resolutionDropdown;

    public Volume postProcessing;

    public Toggle mouseModeMiniGame;
    public Toggle mouseModeBossGame;

    List<Resolution> res;

    void Start()
    {
        res = Screen.resolutions.ToList();
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        List<Resolution> newRes = new List<Resolution>();
        int currentResIndex = 0;
        string prev = "";

        for (int i = 0; i < res.Count; i++)
        {
            string check = res[i].width + "x" + res[i].height;
            float aspect = (float)res[i].width / res[i].height;
            if (check == prev || Mathf.Abs(aspect - (16f / 9f)) > 0.2f) continue;

            string option = check;
            prev = check;
            options.Add(option);
            newRes.Add(res[i]);
            if (res[i].width == Screen.currentResolution.width && res[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        res = newRes;
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
        Screen.SetResolution(res[resIndex].width, res[resIndex].height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetPostProcessing(bool isPostProcessingOn)
    {
        postProcessing.enabled = isPostProcessingOn;
    }

    public void SetMouseModeMini(bool isMouseMode)
    {
        mouseModeBossGame.isOn = isMouseMode;
    }

    public void SetMouseMode(bool isMouseMode)
    {
        mouseModeMiniGame.isOn = isMouseMode;
    }

    public void SaveSettings()
    {
        mixer.GetFloat("MasterVol", out settings.masterVolume);
        mixer.GetFloat("MusicVol", out settings.musicVolume);
        mixer.GetFloat("SFXVol", out settings.sfxVolume);
        settings.toggleData.isFullScreen = Screen.fullScreen;
        settings.toggleData.hasPostProcessing = postProcessing.enabled;
        settings.toggleData.isMouseModeMinigame = mouseModeMiniGame.isOn;
        settings.toggleData.isMouseModeBossgame = mouseModeBossGame.isOn;
        settings.screenResolution = resolutionDropdown.value;
    }

    public void ApplyData()
    {
        //load in the sound data
        foreach(var slider in transform.Find("Settings").GetComponentsInChildren<Slider>())
        {
            if (slider.name == "MasterVolume") slider.value = settings.masterVolume;
            else if (slider.name == "MusicVolume") slider.value = settings.musicVolume;
            else slider.value = settings.sfxVolume;
        }

        //load in the toggle data
        foreach (var toggle in transform.Find("Settings").GetComponentsInChildren<Toggle>())
        {
            if (toggle.name == "PostProcessing") toggle.isOn = settings.toggleData.hasPostProcessing;
            else toggle.isOn = settings.toggleData.isFullScreen;
        }
        mouseModeMiniGame.isOn = settings.toggleData.isMouseModeMinigame;
        mouseModeBossGame.isOn = settings.toggleData.isMouseModeBossgame;

        //load in screen resolution
        resolutionDropdown.value = settings.screenResolution;
    }
}
