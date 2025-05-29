using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadData : MonoBehaviour
{
    private SaveData data;
    public Material flannel;
    [SerializeField] private TMP_Text text;
    public void ApplyData(SaveData data)
    {
        this.data = data;
        text.text = data.id.ToString();
        Material mat = new Material(flannel);
        mat.SetTexture("_Swap", Resources.Load<Texture>($"flannels/{data.flannel}"));
        this.transform.Find("Image").GetComponent<Image>().material = mat;
    }

    public SaveData Data { get { return data; } }

}
