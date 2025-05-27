using GameEvents;
using UnityEngine;
[RequireComponent(typeof(FishListener))]

public class CatchFishHandler : MonoBehaviour
{
    public void DisplayCatch(Fish fish)
    {
        FishData data = Inventory.Instance.GetFishData(fish.fishName);
        if (data.amountCaught == 1)
        {
            PopupManager.Instance.CreateNewFishTypePopup(fish);
        }
        else if(fish.length >= data.largestCaught)
        {
            PopupManager.Instance.CreateNewLargestFishPopup(fish);
        }
        else
        {
            PopupManager.Instance.CreateGenericFishPopup(fish);
        }
    }
}
