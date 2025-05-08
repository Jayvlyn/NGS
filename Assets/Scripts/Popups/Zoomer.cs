using UnityEngine;

public class Zoomer : PearanceHandler
{
    Transform trans;
    Vector3 originalScale;
    private void Start()
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        if (!closing)
        {
            if (canvas != null)
            {
                trans = canvas.transform;
                originalScale = canvas.transform.localScale;
                trans.localScale = Vector3.zero;
            }
            else
            {
                Destroy(this);
            }
        }
        else
        {
            if (canvas != null)
            {
                trans = canvas.transform;
                originalScale = trans.localScale;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (!closing)
        {
            if (currentTime > time)
            {
                trans.localScale = originalScale;
                Destroy(this);
            }
            else
            {
                float completion = currentTime / time;
                trans.localScale = originalScale * completion;
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
                float completion = 1 - (currentTime / time);
                trans.localScale = originalScale * completion;
            }
        }
    }
}
