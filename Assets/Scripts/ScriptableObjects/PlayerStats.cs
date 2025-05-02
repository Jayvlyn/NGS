using System;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public float castMaxSpeed = 1;
    public float fishSizeModifier = 1;
    public float biteSpeed = 1;
    public float catchSpeed = 1;
    public float hookStrength = 1;
    public float bossLineLength = 1;
    public float bossReelSpeed = 1;
    public float grappleMaxCastSpeed = 0.2f;
    public float platformingLineLength = 3.3f;
    public float platformingReelSpeed = 70;
}
