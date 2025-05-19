using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fader : PearanceHandler
{
    private Image[] images;
    private TMP_Text[] texts;
    private SpriteRenderer[] spriteRenderers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        images = gameObject.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, killObject != null ? 1 : 0);
        }
        texts = gameObject.GetComponentsInChildren<TMP_Text>();
        foreach(TMP_Text text in texts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, killObject != null ? 1 : 0);
        }
        spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, killObject != null ? 1 : 0);
        }
        if(images.Length == 0 &&  texts.Length == 0 && spriteRenderers.Length == 0)
        {
            if(killObject != null)
            {
                Destroy(killObject);
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
        if (killObject == null)
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
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, completion);
                }
            }
        }
        else
        {
            if (currentTime > time)
            {
                Destroy(killObject);
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
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, completion);
                }
            }
        }
    }
}
