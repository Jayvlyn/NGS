using UnityEngine;
using UnityEngine.UI;

public class Bubble : MonoBehaviour
{
	[SerializeField] private Image im;
	[SerializeField] private Sprite[] possibleSprites;

	[SerializeField] private float floatSpeed = 1f;
	[SerializeField] private float waveAmplitude = 0.5f;
	[SerializeField] private float waveFrequency = 1f;

	private float startX;
	private float timeOffset;

	private void OnEnable()
	{
		im.sprite = possibleSprites[Random.Range(0, possibleSprites.Length - 1)];
		startX = transform.position.x;
		timeOffset = Random.Range(0f, 100f); // Prevent identical wave motion for all bubbles
	}

	private void Update()
	{
		float y = transform.position.y + floatSpeed * Time.deltaTime;
		float x = startX + Mathf.Sin((Time.time + timeOffset) * waveFrequency) * waveAmplitude;
		transform.position = new Vector3(x, y, transform.position.z);
	}
}
