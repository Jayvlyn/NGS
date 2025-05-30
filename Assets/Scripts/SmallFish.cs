using UnityEngine;

public class SmallFish : MonoBehaviour
{
	public float minMoveSpeed = 2f;
	public float maxMoveSpeed = 4f;
	public float minWaveAmplitude = 15f; // degrees
	public float maxWaveAmplitude = 20f; // degrees
	public float minWaveFrequency = 2f;  // waves per second
	public float maxWaveFrequency = 4f;  // waves per second

	private float time;

	private float moveSpeed;
	private float waveAmplitude;
	private float waveFrequency;

	private void Start()
	{
		moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
		waveAmplitude = Random.Range(minWaveAmplitude, maxWaveAmplitude);
		waveFrequency = Random.Range(minWaveFrequency, maxWaveFrequency);
	}

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
