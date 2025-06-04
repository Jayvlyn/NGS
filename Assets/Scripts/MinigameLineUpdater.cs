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

        // Convert start/end world positions to local positions relative to the line's parent
        Vector3 localStart = lineRect.parent.InverseTransformPoint(startPoint.position);
        Vector3 localEnd = lineRect.parent.InverseTransformPoint(endPoint.position);

        // Midpoint for line position
        Vector3 localMidpoint = (localStart + localEnd) * 0.5f;
        lineRect.localPosition = localMidpoint;

        // Direction and angle
        Vector3 direction = localEnd - localStart;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.localRotation = Quaternion.Euler(0, 0, angle+90);

        // Set line length (height) and preserve its width
        float distance = direction.magnitude;
        Vector2 sizeDelta = lineRect.sizeDelta;
        sizeDelta.y = distance;
        lineRect.sizeDelta = sizeDelta;
    }
}
