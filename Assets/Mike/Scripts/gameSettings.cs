using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    public int screenResolution;

    public bool hasPostProcessing;
    public bool isFullScreen;

    [Header("Platformer input binds")]
    public List<KeyBindingSaveData> platformerKeys;//9

    [Header("Minigame input binds")]
    public List<KeyBindingSaveData> minigameKeys;//4

    [Header("Bossgame input binds")]
    public List<KeyBindingSaveData> bossGameKeys;//6
}
