using System.Collections;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    public void DestroyObj(GameObject go)
    {
        StartCoroutine(destroyRoutine(go));
    }

    private IEnumerator destroyRoutine(GameObject go)
    {
        yield return new WaitForSeconds(3);
        yield return Fade.Instance.FadeIn(0.3f);
        Destroy(go);
        yield return Fade.Instance.FadeOut(0.3f);
    }
}
