using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    public int screenResolution;

    public bool hasPostProcessing;
    public bool isFullScreen;

    public InputActionReference jumpKey;
    public InputActionReference castKey;
    public InputActionReference reelKey;
    public InputActionReference pauseKey;
    public InputActionReference releaseKey;
    public InputActionReference walkLeftKey;
    public InputActionReference interactKey;
    public InputActionReference walkRightKey;
}
