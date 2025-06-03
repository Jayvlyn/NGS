using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class MinigameLineUpdater : MonoBehaviour
{
	public RectTransform startPoint;
	public RectTransform endPoint;

	private RectTransform lineRect;

	void Awake()
	{
		lineRect = GetComponent<RectTransform>();
	}

	void Update()
	{
		if (startPoint == null || endPoint == null || lineRect == null)
			return;

		Vector3 startWorldPos = startPoint.position;
		Vector3 endWorldPos = endPoint.position;

		// Set position to midpoint
		lineRect.position = (startWorldPos + endWorldPos) / 2f;

		// Calculate angle and apply rotation
		Vector3 direction = endWorldPos - startWorldPos;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		lineRect.rotation = Quaternion.Euler(0, 0, angle + 90);

		// Set height (length) to match distance, keep width as is
		float distance = direction.magnitude;
		Vector2 sizeDelta = lineRect.sizeDelta;
		sizeDelta.y = distance;
		lineRect.sizeDelta = sizeDelta;
	}
}
