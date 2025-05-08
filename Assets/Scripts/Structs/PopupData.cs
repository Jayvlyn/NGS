using System;
using UnityEngine;
[Serializable]
public struct PopupData
{
    public bool IsWorldPopup;
    public Transform WorldLocation;
    public string Statement;
    public float Lifetime;
    public string Name;
    public PopupAppearanceData Appearance;
    public PopupAppearanceData Disappearance;
}
