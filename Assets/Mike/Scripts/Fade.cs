using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade:Singleton<Fade>
{
    [SerializeField] private Image fadeImage;

    public IEnumerator StartFade(float duration, float holdBlackScreen = 0, bool hasPlayer = true)
    {
        float halfDuration = duration / 2f;
        string currentActionMap = "";

        switchActionMap(ref currentActionMap, hasPlayer);

        yield return FadeIn(halfDuration, soloFade: false);

        if(holdBlackScreen > 0) yield return new WaitForSeconds(holdBlackScreen);

        yield return FadeOut(halfDuration, soloFade: false);

        switchActionMap(ref currentActionMap, hasPlayer);
    }

    public IEnumerator FadeIn(float duration, string currentActionMap = "", bool hasPlayer = true, bool soloFade = true)
    {
        Color color = fadeImage.color;
        if (fadeImage.color.a < 1) fadeImage.color = new Color(color.r, color.g, color.b, 1);

        if (soloFade) switchActionMap(ref currentActionMap, hasPlayer);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / duration);
            color = new Color(color.r, color.g, color.b, alpha);
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 1);
        if (soloFade) switchActionMap(ref currentActionMap, hasPlayer);
    }

    public IEnumerator FadeOut(float duration, string currentActionMap = "", bool hasPlayer = true, bool soloFade = true)
    {
        Color color = fadeImage.color;
        if(fadeImage.color.a < 1) fadeImage.color = new Color(color.r, color.g, color.b, 1);

        if (soloFade) switchActionMap(ref currentActionMap, hasPlayer);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / duration);
            color = new Color(color.r, color.g, color.b, alpha);
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 0);
        if (soloFade) switchActionMap(ref currentActionMap, hasPlayer);
    }

    private void switchActionMap(ref string currentActionMap, bool hasPlayer)
    {
        if(!hasPlayer) return;

        if (currentActionMap == "")
        {
            currentActionMap = GameUI.Instance.pi.currentActionMap.name;
            GameUI.Instance.pi.SwitchCurrentActionMap("RebindKeys");
        }
        else if (currentActionMap != "")
        {
            GameUI.Instance.pi.SwitchCurrentActionMap(currentActionMap);
        }
    }
}
