using UnityEngine;
using UnityEngine.U2D;

public class SpriteShapeAnimator : MonoBehaviour
{
	[SerializeField] private SpriteShape profile;
	[SerializeField] private Texture2D[] textures;
	[SerializeField] private float animateInterval = 1f;
	private float timer;
	private int index;

	private void Start()
	{
		timer = animateInterval;
		index = 0;
		profile.fillTexture = textures[index];
	}

	private void Update()
	{
		if(timer > 0)
		{
			timer -= Time.deltaTime;
		}
		else
		{ // swap sprite
			timer = animateInterval;

			if(index++ >= textures.Length-1)
			{
				index = 0;
			}

			profile.fillTexture = textures[index];
		}
	}


}
