using System.Collections;
using UnityEngine;

public class Comic : MonoBehaviour
{
    [SerializeField] float heightPerPanel = 500;
    [SerializeField] float transitionTime = 0.5f;
    [SerializeField] float timePerPanel = 3;
    [SerializeField] int panelCount = 5;

    void OnEnable()
    {
        Debug.Log("On enable");
        StartCoroutine(ComicScroll());
    }

    IEnumerator ComicScroll()
    {
        Debug.Log("start co");

        yield return Fade.Instance.FadeOut(2f);

        for (int i = 0; i < panelCount; i++)
        {
            Debug.Log("Before wait");
            yield return new WaitForSeconds(timePerPanel);
            Debug.Log("After wait");

            float initalYPos = transform.position.y;
            float targetYPos = transform.position.y + heightPerPanel;

            float t = 0;
            while(t < transitionTime)
            {
                Debug.Log(t);
                t += Time.deltaTime;
                float yPos = transform.position.y;
                yPos = Mathf.Lerp(initalYPos, targetYPos, t / transitionTime);
                transform.position = new Vector2(transform.position.x, yPos);
                yield return null;
            }

            
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
