using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fader : PearanceHandler
{
    private Image[] images;
    private TMP_Text[] texts;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        images = gameObject.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, closing ? 1 : 0);
        }
        texts = gameObject.GetComponentsInChildren<TMP_Text>();
        foreach(TMP_Text text in texts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, closing ? 1 : 0);
        }
        if(images.Length == 0 &&  texts.Length == 0)
        {
            if(closing)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }
        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (!closing)
        {
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
                float completion = currentTime / time;
                foreach (TMP_Text text in texts)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, completion);
                }
                foreach (Image image in images)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, completion);
                }
            }
        }
        else
        {
            if (currentTime > time)
            {
                Destroy(gameObject);
            }
            else
            {
                float completion = 1 - currentTime / time;
                foreach (TMP_Text text in texts)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, completion);
                }
                foreach (Image image in images)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, completion);
                }
            }
        }
    }
}
