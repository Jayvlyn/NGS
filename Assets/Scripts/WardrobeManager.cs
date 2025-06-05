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

    public List<string> WardrobeNames = new();
    public Material WardrobeMaterial;
    public int CurrentIndex = 0;
    public GameSettings gameSettings;
    public void ConfirmbuttonPressed()
    {
        GameUI.Instance.SaveFlannel(WardrobeTextures[gameSettings.unlockedFlannels[CurrentIndex]].name);
        WardrobeMaterial.SetTexture("_Swap", WardrobeTextures[gameSettings.unlockedFlannels[CurrentIndex]]);
        CloseWardrobe();
    }
    public void OpenWardrobe()
    {
        while(QuestManager.Instance.flannelsToUnlock.Count > 0)
        {
            UnlockFlannel(QuestManager.Instance.flannelsToUnlock[0]);
            QuestManager.Instance.flannelsToUnlock.RemoveAt(0);
        }
        this.gameObject.SetActive(true);
    }
    public void CloseWardrobe()
    {
        this.gameObject.SetActive(false);
    }
    public void IterateWardrobe(int direction)
    {
        CurrentIndex += direction;
        if (CurrentIndex >= gameSettings.unlockedFlannels.Count)
        {
            CurrentIndex = 0;
        }
        if (CurrentIndex < 0)
        {
            CurrentIndex = gameSettings.unlockedFlannels.Count - 1;
        }
        WardrobeMaterial.SetTexture("_Swap", WardrobeTextures[gameSettings.unlockedFlannels[CurrentIndex]]);
        WardrobeName.text = WardrobeNames[gameSettings.unlockedFlannels[CurrentIndex]];
    }

    public void UnlockFlannel(int index)
    {
        if(!gameSettings.unlockedFlannels.Contains(index))
        {
            gameSettings.unlockedFlannels.Add(index);
        }
    }

    public void UnlockFlannel(string flannel)
    {
        for(int i = 0; i < WardrobeNames.Count; i++)
        {
            if (WardrobeNames[i] == flannel)
            {
                UnlockFlannel(i);
                break;
            }
        }
    }
}
