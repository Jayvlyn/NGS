using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    private Image[] images;
    private TMP_Text[] texts;
    public float time;
    private float currentTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        images = gameObject.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
        texts = gameObject.GetComponentsInChildren<TMP_Text>();
        foreach(TMP_Text text in texts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > time)
        {
            foreach (TMP_Text text in texts)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
            }
            foreach (Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            }
            Destroy(this);
        }
        else
        {
            float completion = time / currentTime;
            foreach (TMP_Text text in texts)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, currentTime);
            }
            foreach (Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, currentTime);
            }
        }
    }
}
