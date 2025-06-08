using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static string sceneToLoad;
    public GameSettings settings;
    public Slider progressBar;

    public static bool loadSave = false;

    void Start()
    {
        if (!loadSave)
        {
            if (sceneToLoad == "Desert" && settings.location.currentLocation == "GameScene") settings.currentPos = settings.desertToForest;
            else if (sceneToLoad == "Desert" && settings.location.currentLocation == "Snow") settings.currentPos = settings.desertToSnow;
            else if (sceneToLoad == "GameScene" && settings.location.currentLocation == "Desert") settings.currentPos = settings.forestToDesert;
            else if (sceneToLoad == "Snow" && settings.location.currentLocation == "Desert") settings.currentPos = settings.snowToDesert;
        }
        else loadSave = false;

        if (sceneToLoad != "MainMenu" && sceneToLoad != "BossfightScene") settings.location.currentLocation = sceneToLoad;
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        if (progressBar == null) yield break;
        yield return Fade.Instance.FadeOut(1, hasPlayer:false);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (progressBar.value < 0.9f)
        {
            progressBar.value = Mathf.Clamp01(operation.progress / 1f);
            yield return null;
        }

        float timer = 0f;
        float duration = 0.5f;
        float startValue = progressBar.value;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            progressBar.value = Mathf.Lerp(startValue, 1f, t);
            yield return null;
        }

        progressBar.value = 1f;

        yield return new WaitForSeconds(0.2f);
        yield return Fade.Instance.FadeIn(0.4f, hasPlayer: false);
        operation.allowSceneActivation = true;
    }

    public static IEnumerator LoadScene(string sceneName, bool hasPlayer = false)
    {
        yield return Fade.Instance.FadeIn(0.6f, hasPlayer: hasPlayer);

        if(GlobalAudioManager.Instance.IsLoopingSourcePlaying()) GlobalAudioManager.Instance.StopLoopingAudioSource();
        sceneToLoad = sceneName;

        if (sceneToLoad == "MainMenu" || sceneToLoad == "BossfightScene") GameUI.loadScreens = false;
        else GameUI.loadScreens = true;

        SceneManager.LoadScene("LoadingScene");
    }
}
