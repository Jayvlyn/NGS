using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct FishData
{
    public int amountCaught;
    public float largestCaught;
    public Rarity highestRarity;
    public List<Fish> currentFish;

}
