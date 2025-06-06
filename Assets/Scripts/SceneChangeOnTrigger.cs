using UnityEngine;

public class SceneChangeOnTrigger : MonoBehaviour
{
    public string newSceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("changing");
        StartCoroutine(SceneLoader.LoadScene(newSceneName));
    }
}
