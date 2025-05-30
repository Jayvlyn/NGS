using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AllFish", menuName = "Scriptable Objects/AllFish")]
public class AllFish : ScriptableObject
{
    [SerializeField] public List<Fish> list;
}
