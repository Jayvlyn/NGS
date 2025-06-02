using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MaterialAnimator : MonoBehaviour
{
    [SerializeField] Material material;
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
		material.SetTexture("_MainTex", sprites[index].texture);
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

			material.SetTexture("_MainTex", sprites[index].texture);

		}
	}
}
