using System.Collections.Generic;
using UnityEngine;

public class ShopData : InteractableObject
{
    [SerializeField] private List<FishSellData> fishData;
    [SerializeField] private List<UpgradeData> upgradeData;
    [SerializeField] private PlayerStats playerStats;

	private void Start()
	{
        base.Start();
        exitInteractionRangeEvent.Subscribe(OnExitInteractRange);
	}

	protected override void Interact(InteractionPair pair)
    {
        if(pair.obj.Id == Id)
        {
            ShopManager.Instance.Open();
        }
    }

    public void SellFish(Fish fish)
    {
        foreach(FishSellData fishPrice in fishData)
        {
            if(fishPrice.fish.fishName == fish.fishName)
            {
                Inventory.Instance.AddMoney(fishPrice.pricePerCM * fish.length);
                Inventory.Instance.RemoveFish(fish);
                break;
            }
        }
    }

    public int GetFishPrice(Fish fish)
    {
        double price = 0;
        foreach (FishSellData fishPrice in fishData)
        {
            if (fishPrice.fish.fishName == fish.fishName)
            {
                price = fishPrice.pricePerCM * fish.length;
                break;
            }
        }
        return (int)price;
    }

    public List<UpgradeData> GetUpgrades()
    {
        return upgradeData;
    }

    public void PurchaseUpgrade(int upgradeId)
    {
        foreach(UpgradeData upgrade in upgradeData)
        {
            if (upgrade.Id == upgradeId)
            {
                if (Inventory.Instance.CanAfford(upgrade.currentCost))
                {
                    Inventory.Instance.AddMoney(-upgrade.currentCost);
                    upgrade.currentCost = upgrade.isMultiplicativeIncrease ? upgrade.currentCost * upgrade.costIncrease : upgrade.currentCost + upgrade.costIncrease;
                    playerStats.Upgrade(upgrade);
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
            results.Add((data.fish.fishName, data.fish.sprite));
        }
        return results;
    }

    public bool BuysFish(string fishName)
    {
        bool result = false;
        foreach(FishSellData data in fishData)
        {
            if(data.fish.fishName == fishName)
            {
                result = true;
                break;
            }
        }
        return result;
    }

    private void OnExitInteractRange(InteractionPair pair)
    {
        if (pair.obj.Id == Id)
        {
            ShopManager.Instance.Close();
        }
    }
}
