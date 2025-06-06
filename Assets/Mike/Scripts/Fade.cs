using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade:Singleton<Fade>
{
    [SerializeField] private Image fadeImage;

    public IEnumerator StartFade(float duration, float holdBlackScreen = 0, bool half = false, bool hasPlayer = true)
    {
        float halfDuration = duration / 2f;
        Color color = fadeImage.color;
        string currentActionMap = "";

        if(hasPlayer)
        {
            currentActionMap = GameUI.Instance.pi.currentActionMap.name;
            GameUI.Instance.pi.SwitchCurrentActionMap("RebindKeys");
        }

        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / halfDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 1);

        if(half)
        {
            if (hasPlayer) GameUI.Instance.pi.SwitchCurrentActionMap(currentActionMap);
            yield break;
        }

        if(holdBlackScreen > 0) yield return new WaitForSeconds(holdBlackScreen);

        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / halfDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 0);
        if (hasPlayer) GameUI.Instance.pi.SwitchCurrentActionMap(currentActionMap);
    }
}
