using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string Name;
    public string Description;
    public bool completeable;
    public bool fishQuest = true;
    public float reward;
    public Fish fish;
    public float minLength;
    public string destinationName;
    public int remainingCompletions;
    [HideInInspector] public bool disabled;
}
