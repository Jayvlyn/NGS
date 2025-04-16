using UnityEngine;

public class MouseTrack : MonoBehaviour
{
	private void Update()
	{
		Vector3 mousePosition = Input.mousePosition;

		mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Screen.width);
		mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Screen.height);

		mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);

		Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
		transform.position = worldPosition;
	}
}
