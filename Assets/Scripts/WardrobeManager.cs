using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WardrobeManager : MonoBehaviour
{
    public Button Leftbutton;
    public Button Rightbutton;
    public Button Closebutton;
    public Button Confirmbutton;
    public TMP_Text WardrobeName;
    public Texture2D[] WardrobeTextures;

    public string[] WardrobeNames;
    public Material WardrobeMaterial;
    public List<int> unlockedFlannels = new();
    public List<int> shopFlannels = new();
    public int CurrentIndex = 0;

    public void ConfirmbuttonPressed()
    {
        GameUI.Instance.SaveFlannel(WardrobeTextures[unlockedFlannels[CurrentIndex]].name);
        WardrobeMaterial.SetTexture("_Swap", WardrobeTextures[unlockedFlannels[CurrentIndex]]);
        CloseWardrobe();
    }
    public void OpenWardrobe()
    {
        this.gameObject.SetActive(true);
    }
    public void CloseWardrobe()
    {
        this.gameObject.SetActive(false);
    }
    public void IterateWardrobe(int direction)
    {
        CurrentIndex += direction;
        if (CurrentIndex >= unlockedFlannels.Count)
        {
            CurrentIndex = 0;
        }
        if (CurrentIndex < 0)
        {
            CurrentIndex = unlockedFlannels.Count - 1;
        }
        WardrobeMaterial.SetTexture("_Swap", WardrobeTextures[unlockedFlannels[CurrentIndex]]);
        WardrobeName.text = WardrobeNames[unlockedFlannels[CurrentIndex]];
    }

    public void UnlockFlannel(int index)
    {
        if(!unlockedFlannels.Contains(index))
        {
            unlockedFlannels.Add(index);
        }
    }
}
