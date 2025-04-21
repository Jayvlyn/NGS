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

    [Header("Platformer input binds")]
    public InputActionReference jumpKey;
    public InputActionReference castKey;
    public InputActionReference reelKey;
    public InputActionReference slackKey;
    public InputActionReference pauseKey;
    public InputActionReference walkLeftKey;
    public InputActionReference interactKey;
    public InputActionReference inventoryKey; // tabkey
    public InputActionReference walkRightKey;

    [Header("Minigame input binds")]
    public InputActionReference boberLeftKey;
    public InputActionReference boberRightKey;
    public InputActionReference hookUpKey;
    public InputActionReference hookDownKey;

    [Header("Bossgame input binds")]
    public InputActionReference swimUpKey;
    public InputActionReference swimDownKey;
    public InputActionReference swimLeftKey;
    public InputActionReference swimRightKey;
    public InputActionReference reelInKey;
    public InputActionReference slackOutKey;
}
