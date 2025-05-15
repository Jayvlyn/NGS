using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static string sceneToLoad;
    public GameSettings persistSettings;
    public Slider progressBar;

    void Start()
    {
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

    public static void LoadScene(string sceneName)
    {
        sceneToLoad = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }
}
