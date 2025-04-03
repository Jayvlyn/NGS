using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    //list of fish the pond has 
    [SerializeField] List<Fish> ListOfFish = new List<Fish>();

    //rarity values so 0 is legendary and 3 is uncommon, whatever is left from between the index 3 and the value 100 is common.
    [SerializeField] List<int> rarityValues = new List<int>();

    //fishing bool to check if the player is fishing or not.
    private bool fishing = false;

    private float FishingWaitTimer = 1f;
    private float randomWaitAddon = 0f;
    //max time to wait for a fish to bite.
    [SerializeField] float maxFishingTime = 15f;

    private void Update()
    {
        if (fishing)
        {
            startFishing();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fishing = true;
            randomWaitAddon = Random.Range(0, maxFishingTime - 1.0f);
            FishingWaitTimer = 1f + randomWaitAddon;
        }
    }
    public void startFishing()
    {
        if (FishingWaitTimer <= 0)
        {
            Fish fish = generateFish();
            Debug.Log("You caught a " + fish.rarity + " " + fish.fishName + " of length " + fish.length);
            FishingWaitTimer = 1f;
            fishing = false;
        }
        else
        {
            FishingWaitTimer -= Time.deltaTime;
        }
    }

    //generates a fish from the list with a random rarity.
    Fish generateFish()
    {
        Fish fish = ListOfFish[Random.Range(0, ListOfFish.Count)];
        fish.rarity = generateRarity();
        fish.length = generateLength(fish);
        return fish;
    }
    //returns a random rarity based on the rarity values set in the inspector. 
    private Rarity generateRarity()
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

    private float generateLength(Fish fish)
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
}
