using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
[System.Serializable]
public class SaveData
{
    public SerializedDictionary<string, FishData> inventory;
    public List<GameObject> k;
    public double money;
    public string id;

    public SaveData(string id)
    {
        this.id = id;
    }
}
