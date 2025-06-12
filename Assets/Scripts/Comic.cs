using System.Collections;
using UnityEngine;
using Unity;

public class Comic : MonoBehaviour
{
    [SerializeField] float heightMod = 500;
    [SerializeField] float transitionTime = 0.5f;
    [SerializeField] float timePerPanel = 3;
    [SerializeField] int panelCount = 5;

    void OnEnable()
    {
        StartCoroutine(ComicScroll());
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            gameObject.SetActive(false);
            GameUI.Instance.pi.SwitchCurrentActionMap("Platformer");
        }
    }

    IEnumerator ComicScroll()
    {
        yield return Fade.Instance.FadeOut(2f);

        for (int i = 0; i < panelCount; i++)
        {
            yield return new WaitForSeconds(timePerPanel);

            float initalYPos = transform.position.y;
            float targetYPos = transform.position.y + (heightMod * Screen.height);

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
