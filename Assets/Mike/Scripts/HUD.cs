using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text time;

    [Header("Game Settings")]
    [SerializeField] public GameSettings settings;

    public void UpdateMoney()
    {
        money.text = "x" + Inventory.Instance.GetData().Item2.ToString("F0");
    }

    public void UpdateTime()
    {
        float hoursInDay = 24f;
        float hours = (settings.position.currentTime / 250f) * hoursInDay;
        int hourInt = Mathf.FloorToInt(hours);
        int minutes = Mathf.FloorToInt((hours - hourInt) * 60);

        //int minutes = Mathf.FloorToInt(settings.position.currentTime / 60);
        //int seconds = Mathf.FloorToInt(settings.position.currentTime % 60);

        string formattedTime = string.Format("{0:00}:{1:00}", hourInt, minutes);
        time.text = formattedTime;
    }

    private void FixedUpdate()
    {
        UpdateMoney();
        UpdateTime();
    }
}
