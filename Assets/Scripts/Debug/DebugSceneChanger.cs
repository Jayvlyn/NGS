using UnityEngine;

public class DebugSceneChanger : MonoBehaviour
{
//#if UNITY_EDITOR
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			StartCoroutine(SceneLoader.LoadScene("GameScene"));
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			StartCoroutine(SceneLoader.LoadScene("Desert"));
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			StartCoroutine(SceneLoader.LoadScene("Snow"));
		}
	}
//#endif
}
