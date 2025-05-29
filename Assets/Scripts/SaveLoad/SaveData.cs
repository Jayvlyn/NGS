using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
[System.Serializable]
public class SaveData
{
    public SerializedDictionary<string, FishData> inventory;
    public List<KeyBindingSaveData> platformerKeybinds;
    public List<KeyBindingSaveData> minigameKeybinds;
    public List<KeyBindingSaveData> bossGameKeybinds;
    public positionData position;
    public VolumeData volumeData;
    public ToggleData toggleData;
    public int screenResolution;
    public double money;
    public string id;
    public string flannel;

    public SaveData(string id)
    {
        this.id = id;
    }
}

[System.Serializable]
public struct KeyBindingSaveData
{
    public int actionMap;
    public string actionName;
    public string bindingId;
    public string bindingPath;
}

[System.Serializable]
public struct positionData
{
    public string currentLocation;
    public float currentTime;
    public float x;
    public float y;
}

[System.Serializable]
public struct VolumeData
{
    public float master;
    public float music;
    public float sfx;
}

[System.Serializable]
public struct ToggleData
{
    public bool hasPostProcessing;
    public bool isFullScreen;
    public bool isMouseModeMinigame;
    public bool isMouseModeBossgame;
}