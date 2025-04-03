using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] List<Fish> ListOfFish = new List<Fish>();
    [SerializeField] List<int> rarityValues = new List<int>();
    private bool fishing = false;
    private float FishingWaitTimer = 1f;
    private float randomWaitAddon = 0f;

    private void Update()
    {
        if (fishing)
        {
            StartFishing();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fishing = true;
            randomWaitAddon = Random.Range(0, 15f);
            FishingWaitTimer = 1f + randomWaitAddon;
        }
    }
    public void StartFishing()
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
        return fish;
    }
    //returns a random rarity based on the rarity values set in the inspector. 
    private Rarity generateRarity()
    {
        int n = Random.Range(0, 100);
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
}
