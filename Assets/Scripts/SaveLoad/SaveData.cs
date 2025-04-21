using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
[System.Serializable]
public class SaveData
{
    public SerializedDictionary<string, FishData> inventory;
    public List<string> platformerKeybinds;
    public List<string> minigameKeybinds;
    public List<string> bossGameKeybinds;
    public bool hasPostProcessing;
    public bool isFullScreen;
    public double money;
    public string id;

    public SaveData(string id)
    {
        this.id = id;
    }
}
