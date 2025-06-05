using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MinigameProgressBar : MonoBehaviour
{
    [SerializeField] Slider progressBar;
    [SerializeField] Image progressBarFill;

    [SerializeField] float upperThreshold = 0.75f;
    [SerializeField] float lowerThreshold = 0.2f;

    private float flashTime = 0.2f;
    private bool isFlashing = false;

    private Color highColor = Color.green;
    private Color midColor = Color.yellow;
    private Color lowColor = Color.red;

    private float value = 0;

    void Update()
    {
        value = progressBar.value - progressBar.minValue;
        if (value > upperThreshold)
        {
            StopFlashing();
            float t = Mathf.InverseLerp(progressBar.maxValue, upperThreshold, value);
            progressBarFill.color = Color.Lerp(highColor, midColor, t);
        }
        else if (value > lowerThreshold)
        {
            StopFlashing();
            float t = Mathf.InverseLerp(upperThreshold, lowerThreshold, value);
            progressBarFill.color = Color.Lerp(midColor, lowColor, t);
        }
        else
        {
            StartFlashing();
        }
    }

    private void StartFlashing()
    {
        if(!isFlashing)
        {
            isFlashing = true;
            StartCoroutine(Flash());
        }
    }

    private void StopFlashing()
    {
        if (isFlashing)
        {
            StopCoroutine(Flash());
            progressBarFill.color = lowColor;
            isFlashing = false;
        }
    }

    private IEnumerator Flash()
    {
        while (isFlashing)
        {
            progressBarFill.color = Color.white;
            yield return new WaitForSeconds(flashTime);
            progressBarFill.color = lowColor;
            yield return new WaitForSeconds(flashTime + 0.1f);
        }
    }

    public void Start()
    {
        upperThreshold *= progressBar.maxValue - progressBar.minValue;
        lowerThreshold *= progressBar.maxValue - progressBar.minValue;
    }
}
