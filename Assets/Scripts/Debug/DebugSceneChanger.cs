using UnityEngine;

public class DebugSceneChanger : MonoBehaviour
{
//#if UNITY_EDITOR
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SceneLoader.LoadScene("GameScene");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SceneLoader.LoadScene("Desert");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SceneLoader.LoadScene("Snow");
		}
	}
//#endif
}
