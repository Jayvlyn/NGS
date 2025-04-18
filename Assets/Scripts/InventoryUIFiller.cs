using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIFiller : Singleton<InventoryUIFiller>
{
    [SerializeField] public GameObject fishInventoryUIPrefab;
    [SerializeField] public List<Sprite> starSprites;
    [SerializeField] public float randomRotationRange = 45f;
    private void Start()
    {
        var inventoryData = Inventory.Instance.GetData().Item1;
        
        foreach (var item in inventoryData)
        {
            string fishName = item.Key;
            FishData fishData = item.Value;

            foreach (Fish fish in fishData.currentFish)
            {
                addFishToUI(fish);
            }

        }  
    }
    public void addFishToUI(Fish fish)
    {
        GameObject newPrefab = Instantiate<GameObject>(fishInventoryUIPrefab, this.transform);
        newPrefab.GetComponentsInChildren<TMPro.TMP_Text>()[0].text = fish.fishName;
        Image fishImg = newPrefab.GetComponentsInChildren<Image>()[0];
        Image rarityImg = newPrefab.GetComponentsInChildren<Image>()[2];

        fishImg.sprite = fish.sprite;
        rarityImg.sprite = getStarsFromRarity(fish.rarity);

        //random rotation
        float randomRotation = Random.Range(-randomRotationRange, randomRotationRange);
        fishImg.GetComponentInParent<Transform>().Rotate(0, 0, randomRotation);
        newPrefab.GetComponentsInChildren<Image>()[1].GetComponentInParent<Transform>().Rotate(0, 0, randomRotation);
    }
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