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

    public char jumpKey;
    public char castKey;
    public char reelKey;
    public char pauseKey;
    public char releaseKey;
    public char walkLeftKey;
    public char interactKey;
    public char walkRightKey;
}
