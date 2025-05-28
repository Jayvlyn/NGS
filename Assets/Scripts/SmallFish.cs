using UnityEngine;

public class SmallFish : MonoBehaviour
{
	public float moveSpeed = 2f;
	public float waveAmplitude = 15f; // degrees
	public float waveFrequency = 2f;  // waves per second

	private float time;

	void Update()
	{
		// Move forward constantly in the right direction
		transform.position += (transform.right * transform.localScale.x) * moveSpeed * Time.deltaTime;

		// Update the timer
		time += Time.deltaTime * waveFrequency;

		// Apply sine wave rotation around Z-axis for a wavy motion
		float angle = Mathf.Sin(time * Mathf.PI * 2f) * waveAmplitude;
		transform.rotation = Quaternion.Euler(0f, 0f, angle);
	}
}
