using TMPro;
using UnityEngine;

public class AutoTextFontAdapter : MonoBehaviour
{
    [SerializeField] protected float desiredEffectiveFontSize = 1;
    [SerializeField] protected TMP_Text textObject;
    [SerializeField] protected Vector2 initialSize = Vector2.one;
    public void Resize()
    {
        if (textObject != null)
        {
            ((RectTransform)transform).sizeDelta = initialSize * desiredEffectiveFontSize / textObject.fontSize;
        }
    }

    private void Start()
    {
        Resize();
    }
}
