using System;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
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
}
