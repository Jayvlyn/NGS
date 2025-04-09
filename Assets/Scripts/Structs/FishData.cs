using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct FishData
{
    public int amountCaught;
    public float largestCaught;
    public List<Fish> currentFish;
}
