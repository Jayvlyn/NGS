using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text time;

    private float timeValue;

    private void Start()
    {
        timeValue = GameUI.Instance.gameSettings.position.currentTime;
    }


    public void UpdateMoney()
    {
        money.text = "x" + Inventory.Instance.GetData().Item2.ToString("F0");
    }

    public void UpdateTime()
    {
        time.text = GameUI.Instance.gameSettings.position.currentTime.ToString("F2");
    }

    private void FixedUpdate()
    {
        UpdateMoney();
        UpdateTime();
    }
}
