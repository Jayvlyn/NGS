using GameEvents;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Water : InteractableObject
{
    
    [SerializeField] FishEvent onBite;
    [SerializeField] FishEvent onBossBite;
    [SerializeField] TransformEvent onCast;
    [SerializeField] VoidEvent onQuitFishing;

    //list of fish the pond has 
    [SerializeField] List<Fish> ListOfFish = new List<Fish>();

    //rarity values so 0 is legendary and 3 is uncommon, whatever is left from between the index 3 and the value 100 is common. 
    //good values to start with are 3, 7, 25, 55 
    [SerializeField] List<int> rarityValues = new List<int>() { 3,7,25,55 };

    //fishing bool to check if the player is fishing or not.
    private bool fishing = false;

    private float fishingWaitTimer = 1f;
    private float randomWaitAddon = 0f;
    //max time to wait for a fish to bite.
    [SerializeField] float maxFishingTime = 15f;
    [SerializeField] float maxDistFromStart = 2f;
    private Vector2 startPos;

    private Transform player;

	protected override void Start()
	{
        base.Start();
        player = null;
	}

	private void Update()
	{
		if (fishing)
        {
            DoFishing();
        }
	}

	public void DoFishing()
    {
        if (fishingWaitTimer <= 0)
        {
            Fish fish = GenerateFish();
            fishingWaitTimer = 1f;

            if(fish.isBoss)
            {
                onBossBite.Raise(fish);
                BossFishController.bossFish = fish;
                MenuUI.Instance.SetPosition();
                SceneManager.LoadScene("BossfightScene");
            }
            else
            {
                onBite.Raise(fish);
            }

            QuitFishing();
        }
        else
        {
            fishingWaitTimer -= Time.deltaTime;
            if (player != null)
            {
                if(Vector2.Distance(player.position, startPos) > maxDistFromStart)
                {
                    QuitFishing();
                    onQuitFishing.Raise(); // will pull in hook
                }
            }
            //Debug.Log("Fishing... " + fishingWaitTimer);
        }
    }

    public void QuitFishing()
    {
		fishing = false;
		player = null; // only reference player during cast/waiting time
	}

    //generates a fish from the list with a random rarity.
    Fish GenerateFish()
    {
        Fish fish = Instantiate(ListOfFish[Random.Range(0, ListOfFish.Count)]);
        fish.rarity = GenerateRarity();
        fish.length = GenerateLength(fish);
        return fish;
    }
    //returns a random rarity based on the rarity values set in the inspector. 
    private Rarity GenerateRarity()
    {
        int n = Random.Range(0, 100);
        //Debug.Log("Rarity: " + n);
        if (n >= 0 && n < rarityValues[0] + randomWaitAddon/3f)
        {
            return Rarity.Legendary;
        }
        else if (n >= rarityValues[0] + randomWaitAddon / 3f && n < rarityValues[1] + randomWaitAddon / 3f)
        {
            return Rarity.SuperRare;
        }
        else if (n >= rarityValues[1] && n < rarityValues[2] + randomWaitAddon / 3f)
        {
            return Rarity.Rare;
        }
        else if (n >= rarityValues[2] && n < rarityValues[3] + randomWaitAddon / 3f)
        {
            return Rarity.Uncommon;
        }
        else
        {
            return Rarity.Common;
        }
    }

	private float GenerateLength(Fish fish)
	{
		float length = fish.minLength;

		switch (fish.rarity)
		{
			case Rarity.Common:
				length = Random.Range(fish.minLength, Mathf.Lerp(fish.minLength, fish.maxLength, .2f));
				break;
			case Rarity.Uncommon:
				length = Random.Range(Mathf.Lerp(fish.minLength, fish.maxLength, .2f), Mathf.Lerp(fish.minLength, fish.maxLength, .4f));
				break;
			case Rarity.Rare:
				length = Random.Range(Mathf.Lerp(fish.minLength, fish.maxLength, .4f), Mathf.Lerp(fish.minLength, fish.maxLength, .6f));
				break;
			case Rarity.SuperRare:
				length = Random.Range(Mathf.Lerp(fish.minLength, fish.maxLength, .6f), Mathf.Lerp(fish.minLength, fish.maxLength, .8f));
				break;
			case Rarity.Legendary:
				length = Random.Range(Mathf.Lerp(fish.minLength, fish.maxLength, .8f), fish.maxLength);
				break;
		}

		return Mathf.Round(length * 10f) / 10f; // cut off after one decimal (4.123214415 -> 4.1)
	}

	protected override void Interact(InteractionPair pair)
    {
        if (pair.obj.Id == Id)
        {
			if (!fishing)
            {
                player = pair.actor.transform;
                startPos = player.position;
                onCast.Raise(transform);            
            }
        }
    }

    public void OnCastComplete()
    {
        if (player != null)
        {
            //Debug.Log("cast complete");
            fishing = true;
            randomWaitAddon = Random.Range(0, maxFishingTime - 1.0f);
            fishingWaitTimer = 1f + randomWaitAddon;
        }
	}
}
