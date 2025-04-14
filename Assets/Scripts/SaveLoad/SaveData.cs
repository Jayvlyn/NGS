using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
[System.Serializable]
public class SaveData
{
    //Temporary tracker to verify functionality
    private static int tempTracker = -1;
    public SerializedDictionary<string, FishData> inventory;
    public double money;
    public int id;

    public SaveData()
    {
        id = tempTracker;
        tempTracker++;
    }

    public SaveData(int id)
    {
        this.id = id;
    }
}
