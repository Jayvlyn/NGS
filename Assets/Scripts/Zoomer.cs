using UnityEngine;

public class Zoomer : MonoBehaviour
{
    public float time;
    public float currentTime;
    Transform trans;
    private void Start()
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        if(canvas != null)
        {
            trans = canvas.transform;
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
            trans.localScale = Vector3.one;
            Destroy(this);
        }
        else
        {
            float completion = currentTime / time;
            trans.localScale = new Vector3(completion, completion);
        }
    }
}
