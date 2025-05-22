using UnityEngine;
[System.Serializable]
public struct BasicLandmarkData
{
    public string[] landmarkDescriptions;
    public bool screenPopup;
    public float lifetime;
    public bool standardDescriptionsLoop;
    public PopupAppearanceData appearanceData;
    public PopupAppearanceData closingData;
}
