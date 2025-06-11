using System;
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
    public List<UpgradeData> upgrades;
    public List<int> unlockedFlannels;
    public List<QuestSaveData> quests;
    public locationData location;
    public positionData position;
    public VolumeData volumeData;
    public ToggleData toggleData;
    public int screenResolution;
    public StatsData stats;
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
    public float x;
    public float y;
}

[System.Serializable]
public struct locationData
{
    public string currentLocation;
    public float currentTime;
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

[System.Serializable]
public struct StatsData
{
    public float castMaxSpeed;
    public float fishSizeModifier;
    public float biteSpeed;
    public float catchSpeed;
    public float hookStrength;
    public float bossLineLength;
    public float bossReelSpeed;
    public float grappleMaxCastSpeed;
    public float platformingLineLength;
    public float platformingReelSpeed;
    public float obstacleCount;
}

[Serializable]
public struct QuestSaveData
{
    public string questName;
    public bool completeable;
    public bool disabled;
    public int remainingCompletions;
    public bool active;
    public bool completed;
}
