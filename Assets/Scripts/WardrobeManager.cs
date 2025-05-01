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

    public int CurrentIndex = 0;

    public void ConfirmbuttonPressed()
    {
        WardrobeMaterial.SetTexture("_TargetPalette", WardrobeTextures[CurrentIndex]);
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
        if (CurrentIndex >= WardrobeTextures.Length)
        {
            CurrentIndex = 0;
        }
        if (CurrentIndex < 0)
        {
            CurrentIndex = WardrobeTextures.Length - 1;
        }
        WardrobeMaterial.SetTexture("_TargetPalette", WardrobeTextures[CurrentIndex]);
        WardrobeName.text = WardrobeNames[CurrentIndex];
    }
}
