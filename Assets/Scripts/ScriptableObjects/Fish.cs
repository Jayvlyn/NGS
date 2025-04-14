using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Fish", menuName = "Scriptable Objects/Fish")]
public class Fish : ScriptableObject
{
    public string fishName;
    public Sprite sprite;
    public Rarity rarity;
    public float length;
    public float minLength;
    public float maxLength;
}
