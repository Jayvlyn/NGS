using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadData : MonoBehaviour
{
    private SaveData data;
    [SerializeField] private TMP_Text text;
    public void ApplyData(SaveData data)
    {
        this.data = data;
        text.text = data.id.ToString();
    }

    public void Select()
    {
        gameObject.GetComponentInChildren<Image>().color = Color.red;
    }

}
