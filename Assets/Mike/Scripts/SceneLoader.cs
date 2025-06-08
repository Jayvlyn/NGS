using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static string sceneToLoad;
    public GameSettings settings;
    public Slider progressBar;

    void Start()
    {
        if (sceneToLoad != "MainMenu" && sceneToLoad != "BossfightScene") settings.location.currentLocation = sceneToLoad;
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        if (progressBar == null) yield break;
        yield return Fade.Instance.FadeOut(1, hasPlayer:false);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (progressBar.value < 1)
        {
            progressBar.value = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        yield return Fade.Instance.FadeIn(0.4f, hasPlayer: false);
        operation.allowSceneActivation = true;
    }

    public static IEnumerator LoadScene(string sceneName, bool GameSceneSwitch = false, bool hasPlayer = false)
    {
        yield return Fade.Instance.FadeIn(0.6f, hasPlayer: hasPlayer);

        if(GlobalAudioManager.Instance.IsLoopingSourcePlaying()) GlobalAudioManager.Instance.StopLoopingAudioSource();
        sceneToLoad = sceneName;

        if (sceneToLoad == "MainMenu" || sceneToLoad == "BossfightScene") GameUI.loadScreens = false;
        else GameUI.loadScreens = true;

        if (GameSceneSwitch) GameUI.gameStart = true;

        SceneManager.LoadScene("LoadingScene");
    }
}
