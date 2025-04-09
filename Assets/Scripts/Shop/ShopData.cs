using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopData : MonoBehaviour
{
    [SerializeField] private List<FishSellData> fishData;
    [SerializeField] private List<UpgradeData> upgradeData;

    public void SellFish(Fish fish)
    {
        foreach(FishSellData fishPrice in fishData)
        {
            if(fishPrice.name == fish.name)
            {
                Inventory.Instance.AddMoney(fishPrice.pricePerCM * fish.length);
                Inventory.Instance.RemoveFish(fish);
                break;
            }
        }
    }

    public double GetFishPrice(Fish fish)
    {
        double price = 0;
        foreach (FishSellData fishPrice in fishData)
        {
            if (fishPrice.name == fish.name)
            {
                price = fishPrice.pricePerCM * fish.length;
                break;
            }
        }
        return price;
    }


    public void PurchaseUpgrade(int upgradeId)
    {
        foreach(UpgradeData upgrade in upgradeData)
        {
            if (upgrade.upgradeID == upgradeId)
            {
                if (Inventory.Instance.CanAfford(upgrade.currentCost))
                {
                    Inventory.Instance.AddMoney(-upgrade.currentCost);
                    upgrade.currentCost = upgrade.isMultiplicativeIncrease ? upgrade.currentCost * upgrade.costIncrease : upgrade.currentCost + upgrade.costIncrease;
                    //Requires implementation of player upgradeabiliyt, will go here later
                }
                break;
            }
        }
    }
    public List<(string, Sprite)> GetAvailableFish()
    {
        List<(string, Sprite)> results = new();
        foreach(FishSellData data in fishData)
        {
            results.Add((data.name, data.sprite));
        }
        return results;
    }
}
