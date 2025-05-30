using UnityEngine;

public class SwimmingFish : MonoBehaviour
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

	private Quaternion initialRotation;

	private void Start()
	{
		initialRotation = transform.rotation;
		moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
		waveAmplitude = Random.Range(minWaveAmplitude, maxWaveAmplitude);
		waveFrequency = Random.Range(minWaveFrequency, maxWaveFrequency);
	}

	void Update()
	{
		transform.position += initialRotation * Vector3.right * moveSpeed * Time.deltaTime;
		time += Time.deltaTime * waveFrequency;
		float angle = Mathf.Sin(time * Mathf.PI * 2f) * waveAmplitude;
		transform.rotation = initialRotation * Quaternion.Euler(0f, 0f, angle);
	}
}
