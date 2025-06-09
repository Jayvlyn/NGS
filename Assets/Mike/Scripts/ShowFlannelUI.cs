using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowFlannelUI : MonoBehaviour
{
    public Material flannel;
    [SerializeField] private TMP_Text FlannelName;

    void Start()
    {
        Material mat = new Material(flannel);
        print(FlannelName.text.Split(":")[0]);
        mat.SetTexture("_Swap", Resources.Load<Texture>($"flannels/{FlannelName.text.Split(":")[0]}"));
        gameObject.GetComponent<Image>().material = mat;
    }
}
