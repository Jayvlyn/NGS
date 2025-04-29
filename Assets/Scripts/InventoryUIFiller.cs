using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIFiller : Singleton<InventoryUIFiller>
{
    [SerializeField] public GameObject fishInventoryUIPrefab;
    [SerializeField] public List<Sprite> starSprites;
    [SerializeField] public float randomRotationRange = 45f;
    [SerializeField] public GameObject fishInventoryUI;
    [SerializeField] public List<Fish> fishList;
    [SerializeField] public List<Fish> fishListToRemove;
    [SerializeField] public List<GameObject> currentInventoryUIElements;

    //needs to be set to true in the inspector. Will be set to false after start is ran. 
    private void Start()
    {
        var inventoryData = Inventory.Instance.GetData().Item1;
        foreach (var item in inventoryData)
        {
            string fishName = item.Key;
            FishData fishData = item.Value;

            foreach (Fish fish in fishData.currentFish)
            {
                AddFishToUI(fish);
            }
        }  
        //Set the UI to false after it loads data.
        fishInventoryUI.SetActive(false);
    }
    //add fish to the list of fish to be added to the UI
    public void AddFishToInventoryUI(Fish fish)
    {
        fishList.Add(fish);
    }
    public void RemoveFishFromInventoryUI(Fish fish)
    { 
        fishListToRemove.Add(fish);
    }
    //Method to add fish to the UI
    private void AddFishToUI(Fish fish)
    {
        //create prefab and get the components 
        GameObject newPrefab = Instantiate<GameObject>(fishInventoryUIPrefab, this.transform);
        newPrefab.GetComponent<FishInventoryItem>().fish = fish;
        Image fishImg = newPrefab.GetComponentsInChildren<Image>()[0];
        Image rarityImg = newPrefab.GetComponentsInChildren<Image>()[2];
        fishImg.sprite = fish.sprite;
        rarityImg.sprite = getStarsFromRarity(fish.rarity);

        //random rotation
        float randomRotation = Random.Range(-randomRotationRange, randomRotationRange);
        fishImg.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0,0,randomRotation);
        newPrefab.GetComponentsInChildren<Image>()[1].GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, 0, randomRotation);
        currentInventoryUIElements.Add(newPrefab); //add to the list of current UI elements
    }
    private void removeFishFromUI(Fish fish)
    { 
        foreach (GameObject fishUI in currentInventoryUIElements)
        {
            if (fishUI.GetComponent<FishInventoryItem>().fish == fish)
            {
                Destroy(fishUI);
                currentInventoryUIElements.Remove(fishUI);
                break;
            }
        }
    }
    private void FixedUpdate()
    {
        //when inventory is set active then add the UI elements 
        if (fishList.Count > 0 && this.isActiveAndEnabled)
        {
            foreach (Fish fish in fishList)
            {
                AddFishToUI(fish);
                
            }   
            fishList.Clear(); //clear after the list is added.
        }
        if (fishListToRemove.Count > 0 && this.isActiveAndEnabled)
        {
            foreach (Fish fish in fishListToRemove)
            {
                removeFishFromUI(fish);
            }
            fishListToRemove.Clear();
        }
    }
    //get correct star image based on rarity
    public Sprite getStarsFromRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return starSprites[1];
            case Rarity.Uncommon:
                return starSprites[2];
            case Rarity.Rare:
                return starSprites[3];
            case Rarity.SuperRare:
                return starSprites[4];
            case Rarity.Legendary:
                return starSprites[5];

        }
        return starSprites[0];

    }
}