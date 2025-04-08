using GameEvents;
using OdinSerializer;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    [SerializeField] protected Dictionary<string, (int, float, List<Fish>)> currentFish = new();

    private void Start()
    {
        
    }

    public void AddFish(Fish fish)
    {
        if(!currentFish.ContainsKey(fish.name))
        {
            currentFish.Add(fish.name, (0, 0, new()));
        }
        (int, float, List<Fish>) data = currentFish[fish.name];
        data.Item1++;
        data.Item2 = Mathf.Max(fish.length, data.Item2);
        data.Item3.Add(fish);
        currentFish[fish.name] = data;
    }

    public void RemoveFish(Fish fish)
    {
        if(currentFish.ContainsKey(fish.name))
        {
            currentFish[fish.name].Item3.Remove(fish);
        }
    }

    public (int, float) GetFishMetadata(string name)
    {
        return (currentFish[name].Item1, currentFish[name].Item2);
    }

    public List<Fish> GetFish(string name)
    {
        return currentFish[name].Item3;
    }
}
