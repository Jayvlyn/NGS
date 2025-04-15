using GameEvents;
using System.Collections.Generic;
using UnityEngine;

public class Water : InteractableObject
{
    
    [SerializeField] FishEvent onBite;
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
            onBite.Raise(fish);
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
        Debug.Log("Rarity: " + n);
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
        switch (fish.rarity)
        {
            case Rarity.Common:
                return Random.Range(fish.minLength, Mathf.Lerp(fish.minLength,fish.maxLength,.2f));
            case Rarity.Uncommon:
                return Random.Range(Mathf.Lerp(fish.minLength, fish.maxLength, .2f), Mathf.Lerp(fish.minLength, fish.maxLength, .4f));
            case Rarity.Rare:
                return Random.Range(Mathf.Lerp(fish.minLength, fish.maxLength, .4f), Mathf.Lerp(fish.minLength, fish.maxLength, .6f));
            case Rarity.SuperRare:
                return Random.Range(Mathf.Lerp(fish.minLength, fish.maxLength, .6f), Mathf.Lerp(fish.minLength, fish.maxLength, .8f));
            case Rarity.Legendary:
                return Random.Range(Mathf.Lerp(fish.minLength, fish.maxLength, .8f), fish.maxLength);
        }
        return fish.minLength;
    }

    protected override void Interact(InteractionPair pair)
    {
        if (!fishing)
        {
            player = pair.actor.transform;
            startPos = player.position;
            onCast.Raise(transform);            
        }
    }

    public void OnCastComplete()
    {
        if (player != null)
        {
            fishing = true;
            randomWaitAddon = Random.Range(0, maxFishingTime - 1.0f);
            fishingWaitTimer = 1f + randomWaitAddon;
        }
	}
}
