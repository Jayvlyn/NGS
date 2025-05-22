using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
	public RectTransform rectTransform;
	public float floatRange = 5f;
	public float floatSpeed = 1f;
	public float rotationRange = 5f;
	public float rotationSpeed = 1f;
	public float manualRotationOffset = 0f;


	public Vector2 originalPosition = Vector2.zero;
	private float timeOffsetX;
	private float timeOffsetY;
	private float rotationOffset;

	void Start()
	{
		if (originalPosition == Vector2.zero)
		{
			originalPosition = rectTransform.anchoredPosition;
		}

		timeOffsetX = Random.Range(0f, 100f);
		timeOffsetY = Random.Range(0f, 100f);
		rotationOffset = Random.Range(0f, 100f);
	}

	void Update()
	{
		float offsetX = Mathf.PerlinNoise(Time.time * floatSpeed + timeOffsetX, 0f) * 2f - 1f;
		float offsetY = Mathf.PerlinNoise(Time.time * floatSpeed + timeOffsetY, 0f) * 2f - 1f;

		Vector2 newPosition = originalPosition + new Vector2(offsetX, offsetY) * floatRange;

		rectTransform.anchoredPosition = newPosition;

		float rotation = Mathf.PerlinNoise(Time.time * rotationSpeed + rotationOffset, 0f) * 2f - 1f;
		float newRotation = (rotation * rotationRange) + manualRotationOffset;

		rectTransform.localRotation = Quaternion.Euler(0f, 0f, newRotation);
	}
}
