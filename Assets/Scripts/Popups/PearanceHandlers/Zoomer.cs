using UnityEngine;

public class Zoomer : PearanceHandler
{
    Transform trans;
    Vector3 originalScale;
    private void Start()
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        if (killObject == null)
        {
            if (canvas != null)
            {
                trans = canvas.transform;
            }
            else
            {
                trans = transform;
            }
            originalScale = trans.localScale;
            trans.localScale = Vector3.zero;
        }
        else
        {
            if (canvas != null)
            {
                trans = canvas.transform;
            }
            else
            {
                trans = transform;
            }
            originalScale = trans.localScale;
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (killObject == null)
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
                Destroy(killObject);
            }
            else
            {
                float completion = 1 - (currentTime / time);
                trans.localScale = originalScale * completion;
            }
        }
    }
}
