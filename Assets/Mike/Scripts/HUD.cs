using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text time;
    [SerializeField] private DayNightCycle timeCycle;


    public void UpdateMoney()
    {
        money.text = "x" + Inventory.Instance.GetData().Item2.ToString("F0");
    }

    public void UpdateTime()
    {
        print(timeCycle.GetTime());
        time.text = timeCycle.CurrentHour.ToString("F2");
    }

    private void FixedUpdate()
    {
        UpdateMoney();
        UpdateTime();
    }
}
