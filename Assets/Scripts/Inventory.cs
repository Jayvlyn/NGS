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

    private void Start()
    {
        if (testFish != null)
        {
            AddFish(testFish);
        }
        Debug.Log(JsonUtility.ToJson(GetData()));
    }

    public void AddFish(Fish fish)
    {
        if(!currentFish.ContainsKey(fish.name))
        {
            FishData fishData = new()
            {
                currentFish = new()
            };
            currentFish.Add(fish.name, fishData);
        }
        FishData data = currentFish[fish.name];
        data.amountCaught++;
        data.largestCaught = Mathf.Max(fish.length, data.largestCaught);
        data.currentFish.Add(fish);
        currentFish[fish.name] = data;
    }

    public void RemoveFish(Fish fish)
    {
        if(currentFish.ContainsKey(fish.name))
        {
            currentFish[fish.name].currentFish.Remove(fish);
        }
    }
    public FishData GetFishData(string name)
    {
        return currentFish.ContainsKey(name) ? currentFish[name] : new();
    }

    public SerializedDictionary<string, FishData> GetData()
    {
        return currentFish;
    }

    public void ApplyData(SerializedDictionary<string, FishData> data)
    {
        currentFish.Clear();
        foreach(string key in data.Keys)
        {
            currentFish.Add(key, data[key]);
        }
    }
}
