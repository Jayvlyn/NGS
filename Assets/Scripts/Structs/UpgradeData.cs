using OdinSerializer;
using System;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class UpgradeData : SerializedScriptableObject
{
    public int Id;
    public string upgradeName;
    public double currentCost;
    public double costIncrease;
    public bool isMultiplicativeIncrease;
    public double maxCostBeforeDelete;
    public Color upgradeColor;
}
