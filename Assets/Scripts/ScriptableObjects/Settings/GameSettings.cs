using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    public string id;
    public string flannel;
    public List<int> unlockedFlannels = new List<int>() { 0, 6};
    public List<UpgradeData> upgrades;

    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    public int screenResolution;

    public ToggleData toggleData;

    public locationData location;

    public positionData currentPos;
    public positionData forestToDesert;
    public positionData desertToForest;
    public positionData desertToSnow;
    public positionData snowToDesert;

    [Header("Platformer input binds")]
    public List<KeyBindingSaveData> platformerKeys;//9

    [Header("Minigame input binds")]
    public List<KeyBindingSaveData> minigameKeys;//4

    [Header("Bossgame input binds")]
    public List<KeyBindingSaveData> bossGameKeys;//6

}
