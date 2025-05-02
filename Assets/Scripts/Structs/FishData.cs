using OdinSerializer;
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
    [OdinSerialize, HideInInspector]
    public List<caughtFish> fishHeld;

}

[Serializable]
public struct caughtFish
{
    public string fishName;
    [HideInInspector] public Rarity rarity;
    [HideInInspector] public float length;
    public string description;
    public bool isBoss;
}