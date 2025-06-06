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
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!operation.isDone)
        {
            if (progressBar != null) progressBar.value = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }
    }

    public static IEnumerator LoadScene(string sceneName, bool GameSceneSwitch = false)
    {
        yield return Fade.Instance.StartFade(2, half:true);

        if(GlobalAudioManager.Instance.IsLoopingSourcePlaying()) GlobalAudioManager.Instance.StopLoopingAudioSource();
        sceneToLoad = sceneName;

        if (sceneToLoad == "MainMenu" || sceneToLoad == "BossfightScene") GameUI.loadScreens = false;
        else GameUI.loadScreens = true;

        if (GameSceneSwitch) GameUI.gameStart = true;

        SceneManager.LoadScene("LoadingScene");
    }
}
