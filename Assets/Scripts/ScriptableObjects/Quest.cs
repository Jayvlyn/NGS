using GameEvents;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    public string description;
	[HideInInspector] public bool completeable;
    public bool fishQuest = true;
    public Reward reward;
    public Fish fish;
    public float minLength;
    public string destinationName;
    public int remainingCompletions;
    public VoidEvent onCompleteEvent;
    [HideInInspector] public bool disabled;
}
