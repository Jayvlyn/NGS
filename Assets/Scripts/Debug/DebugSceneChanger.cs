using UnityEngine;

public class DebugSceneChanger : MonoBehaviour
{
//#if UNITY_EDITOR
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SceneLoader.LoadScene("GameScene", true);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SceneLoader.LoadScene("Desert", true);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SceneLoader.LoadScene("Snow", true);
		}
	}
//#endif
}
