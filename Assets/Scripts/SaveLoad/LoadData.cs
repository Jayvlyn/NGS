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

    public SaveData Data { get { return data; } }

}
