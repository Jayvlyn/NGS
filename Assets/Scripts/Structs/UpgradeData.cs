using System;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public int Id;
    public string upgradeName;
    public double currentCost;
    public double costIncrease;
    public bool isMultiplicativeIncrease;
    public double maxCostBeforeDelete;
}
