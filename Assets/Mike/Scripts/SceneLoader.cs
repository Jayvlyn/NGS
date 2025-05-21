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
        settings.position.currentLocation = sceneToLoad;
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!operation.isDone)
        {
            if (progressBar != null)
                progressBar.value = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }
    }

    public static void LoadScene(string sceneName, bool GameSceneSwitch = false)
    {
        sceneToLoad = sceneName;
        if (sceneName == "MainMenu" || sceneName == "BossfightScene") GameUI.loadScreens = false;
        else GameUI.loadScreens = true;

        if (GameSceneSwitch) GameUI.gameStart = true;

        SceneManager.LoadScene("LoadingScene");
    }
}
