using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text time;


    public void UpdateMoney()
    {
        money.text = "x" + Inventory.Instance.GetData().Item2.ToString("F2");
    }

    public void UpdateTime(float change)
    {
        time.text = change.ToString();
    }

    private void FixedUpdate()
    {
        UpdateMoney();
    }
}
