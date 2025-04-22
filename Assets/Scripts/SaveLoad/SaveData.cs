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
    public bool hasPostProcessing;
    public bool isFullScreen;
    public double money;
    public string id;

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
    public string bindingId; // To uniquely identify the binding within the action
    public string bindingPath;
}