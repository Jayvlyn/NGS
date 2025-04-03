using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    public int screenResolution;

    public bool hasPostProcessing;
    public bool isFullScreen;

    public KeyCode jumpKey;
    public KeyCode castKey;
    public KeyCode reelKey;
    public KeyCode pauseKey;
    public KeyCode releaseKey;
    public KeyCode walkLeftKey;
    public KeyCode interactKey;
    public KeyCode walkRightKey;
}
