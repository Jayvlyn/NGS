using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ImageAnimator : MonoBehaviour
{
    [SerializeField] Image image;
	[SerializeField] Sprite[] sprites;
	[SerializeField] float frameInterval = 0.3f;
	float swapTimer;
	int index = 0;

	private void Start()
	{
		swapTimer = frameInterval;
		image.sprite = sprites[index];
	}

	private void Update()
	{
		if(swapTimer > 0)
		{
			swapTimer -= Time.deltaTime;
		}
		else // swap image
		{
			swapTimer = frameInterval;
			if (index++ >= sprites.Length - 1)
			{
				index = 0;
			}

			image.sprite = sprites[index];

		}
	}
}
