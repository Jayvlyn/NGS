using System;
using UnityEngine;
[Serializable]
public class UpgradeData
{
    public int Id;
    public string name;
    public double currentCost;
    public double costIncrease;
    public bool isMultiplicativeIncrease;
    public Sprite sprite;
}
