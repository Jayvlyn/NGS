using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SpriteRendererAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
	[SerializeField] Sprite[] sprites;
	[SerializeField] float frameInterval = 0.3f;
	float swapTimer;
	int index = 0;

	private void Start()
	{
		swapTimer = frameInterval;
		spriteRenderer.sprite = sprites[index];
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

			spriteRenderer.sprite = sprites[index];

		}
	}
}
