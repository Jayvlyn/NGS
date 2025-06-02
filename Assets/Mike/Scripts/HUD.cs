using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text time;

    [Header("Game Settings")]
    [SerializeField] public GameSettings settings;

    private string lastFormattedTime = "";

    public void UpdateMoney()
    {
        money.text = "x" + Inventory.Instance.GetData().Item2.ToString("F0");
    }

    public void UpdateTime()
    {
        float hoursInDay = 24f;
        float hours = (settings.position.currentTime / 250f) * hoursInDay;
        hours %= hoursInDay;

        int hourInt = Mathf.FloorToInt(hours);
        int minutes = Mathf.FloorToInt((hours - hourInt) * 60);

        if (minutes % 10 != 0) return;

        string formattedTime = string.Format("{0:00}:{1:00}", hourInt, minutes);

        if (formattedTime == lastFormattedTime) return;

        lastFormattedTime = formattedTime;
        time.text = formattedTime;
    }

    private void FixedUpdate()
    {
        UpdateMoney();
        UpdateTime();
    }
}
