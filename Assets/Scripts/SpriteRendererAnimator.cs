using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SpriteRendererAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
	[SerializeField] Sprite[] sprites;
	[SerializeField] float frameInterval = 0.3f;
	[SerializeField] bool randomizeStart = false;
    float swapTimer;
	int index = 0;

	private void Start()
	{
		if (randomizeStart)
        {
            index = Random.Range(0, sprites.Length);
        }
        else
        {
            index = 0;
        }
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
