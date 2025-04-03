using UnityEngine;
[System.Serializable]
public class SaveData
{
    //Temporary tracker to verify functionality
    private static int tempTracker = -1;

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
