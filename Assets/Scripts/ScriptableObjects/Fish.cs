using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Fish", menuName = "Scriptable Objects/Fish")]
public class Fish : ScriptableObject
{
    public string fishName;
    public Sprite sprite;
	[HideInInspector] public Rarity rarity;
    [HideInInspector] public float length;
    public float minLength;
    public float maxLength;
}
