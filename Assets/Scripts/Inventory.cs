using GameEvents;
using OdinSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class Inventory : Singleton<Inventory>
{
    [SerializeField] protected SerializedDictionary<string, FishData> currentFish = new();
    [SerializeField] private Fish testFish;
    [SerializeField] protected double money;

    private void Start()
    {
        if (testFish != null)
        {
            AddFish(testFish);
        }
        //Debug.Log(JsonUtility.ToJson(GetData()));
    }

    public void AddFish(Fish fish)
    {
        GameUI.Instance.inventoryUIFiller.AddFishToInventoryUI(fish);
        if (!currentFish.ContainsKey(fish.fishName))
        {
            FishData fishData = new()
            {
                currentFish = new(),
                fishHeld = new()
            };
            currentFish.Add(fish.fishName, fishData);
            
        }
        FishData data = currentFish[fish.fishName];
        data.amountCaught++;
        data.highestRarity = fish.rarity > data.highestRarity ? fish.rarity : data.highestRarity;
        data.largestCaught = Mathf.Max(fish.length, data.largestCaught);
        data.currentFish.Add(fish);
        GameUI.Instance.collection.AddFishToCollection(fish, data);
        currentFish[fish.fishName] = data;
    }

    public void RemoveFish(Fish fish)
    {
        if(currentFish.ContainsKey(fish.fishName))
        {
            currentFish[fish.fishName].currentFish.Remove(fish);
            GameUI.Instance.inventoryUIFiller.RemoveFishFromInventoryUI(fish);
            Destroy(fish);
        }
    }

    public void RestInventory()
    {
        currentFish.Clear();
        money = 0;
    }
    
    public FishData GetFishData(string name)
    {
        return currentFish.ContainsKey(name) ? currentFish[name] : new();
    }

    public (SerializedDictionary<string, FishData>, double) GetData()
    {
        return (currentFish, money);
    }

    public void ApplyData(SerializedDictionary<string, FishData> data, double cash)
    {
        currentFish.Clear();
        foreach(string key in data.Keys)
        {
            currentFish.Add(key, data[key]);
        }
        money = cash;
    }

    public void AddMoney(double amount)
    {
        money += amount;
    }

    public bool CanAfford(double amount)
    {
        return money >= amount;
    }

    public void ApplyData(SerializedDictionary<string, FishData> data)
    {
        currentFish.Clear();
        foreach(string key in data.Keys)
        {
            currentFish.Add(key, data[key]);
        }
    }

    public override string ToString()
    {
        string str = string.Empty;

        str += "All fish";
        foreach(string key in currentFish.Keys)
        {
            str += "\n" + key + ": " + currentFish[key].amountCaught;
        }

        return str;
    }
}
