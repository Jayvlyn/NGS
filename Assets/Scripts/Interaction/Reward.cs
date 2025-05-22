using UnityEngine;
[System.Serializable]
public struct Reward
{
    public string flannelName;
    public float carrots;
    public string GetReward()
    {
        if(string.IsNullOrEmpty(flannelName))
        {
            return $"{carrots} carrots";
        }
        return $"{flannelName} flannel";
    }
}
