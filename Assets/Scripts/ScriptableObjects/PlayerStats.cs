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

    public void Upgrade(UpgradeData upgradeData)
    {
        switch(upgradeData.Id)
        {
            case 0:
                castMaxSpeed *= 0.95f;
                break;
            case 1:
                fishSizeModifier += 0.05f;
                break;
            case 2:
                biteSpeed *= 0.95f;
                break;
            case 3:
                catchSpeed += 0.1f;
                break;
            case 4:
                hookStrength *= 0.95f;
                break;
            case 5:
                bossLineLength += 0.1f;
                platformingLineLength += 0.033f;
                break;
            case 6:
                bossReelSpeed += 0.1f;
                platformingReelSpeed += 7;
                break;
            case 7:
                grappleMaxCastSpeed += 0.02f;
                break;
                
        }
    }

    public void CopyFrom(PlayerStats stats)
    {
        castMaxSpeed = stats.castMaxSpeed;
        fishSizeModifier = stats.fishSizeModifier;
        biteSpeed = stats.biteSpeed;
        catchSpeed = stats.catchSpeed;
        hookStrength = stats.hookStrength;
        bossLineLength = stats.bossLineLength;
        bossReelSpeed = stats.bossReelSpeed;
        grappleMaxCastSpeed = stats.grappleMaxCastSpeed;
        platformingLineLength = stats.platformingLineLength;
        platformingReelSpeed = stats.platformingReelSpeed;
    }
}
