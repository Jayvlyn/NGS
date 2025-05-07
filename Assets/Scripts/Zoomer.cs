using UnityEngine;

public class Zoomer : MonoBehaviour
{
    public float time;
    public float currentTime;
    Transform trans;
    Vector3 originalScale;
    private void Start()
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        if(canvas != null)
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

    private void Update()
    {
        currentTime += Time.deltaTime;
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
}
