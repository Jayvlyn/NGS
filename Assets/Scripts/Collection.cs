using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Collection : Singleton<Collection>
{

    [SerializeField] Dictionary<string, GameObject> fishCollection = new();

    [SerializeField] public GameObject fishCollectionUIPrefab;
    [SerializeField] public float randomRotationRange = 15f;
    [SerializeField] public GameObject collectionScrollView;
    [SerializeField] public List<Fish> fishList;
    [SerializeField] public List<FishData> fishDataList;
    [SerializeField] public List<Fish> allFishToBeLoaded;
    [SerializeField] public Sprite starSpriteEmpty;

    private void Start()
    {

        foreach (Fish f in allFishToBeLoaded)
        { 
            AddFishToCollection(f, Inventory.Instance.GetFishData(f.fishName));

        }
        collectionScrollView.SetActive(false);
    }
    private void FixedUpdate()
    {
        //when inventory is set active then add the UI elements 
        if (fishList.Count > 0 && this.isActiveAndEnabled)
        {
            for (int i = 0; i < fishList.Count; i++)
            {
                Fish fish = fishList[i];
                FishData fishData = fishDataList[i];
                AddFishToCollectionUI(fishData, fish);
            }
            fishList.Clear(); //clear after the list is added.
            fishDataList.Clear(); //clear after the list is added.
        }
    }
    //adds fish to the list to be added to the UI later.
    public void AddFishToCollection(Fish fish, FishData fishData)
    {
        if (!fishList.Contains(fish))
        {
            fishList.Add(fish);
            fishDataList.Add(fishData);
        }
    }
    //adds the fish to the ui itself or updates the current one.
    private void AddFishToCollectionUI(FishData fishData, Fish fish)
    {
        
        GameObject collectionPrefab = findOrCreateCollectionPrefab(fish.fishName);
        if (fishData.amountCaught == 0)
        {
            Image fishImg = collectionPrefab.GetComponentsInChildren<Image>()[0];
            Image rarityImg = collectionPrefab.GetComponentsInChildren<Image>()[2];
            fishImg.sprite = fish.sprite;
            fishImg.color = Color.black; // Set the image to black if no fish caught
            rarityImg.sprite = starSpriteEmpty;
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[0].text = "?????????";
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[1].text = "Largest Caught: ????";
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[2].text = "Amount Caught: " + fishData.amountCaught;

            float randomRotation = Random.Range(-randomRotationRange, randomRotationRange);
            fishImg.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, 0, randomRotation);
            collectionPrefab.GetComponentsInChildren<Image>()[1].GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, 0, randomRotation);
        }
        else {
            Image fishImg = collectionPrefab.GetComponentsInChildren<Image>()[0];
            Image rarityImg = collectionPrefab.GetComponentsInChildren<Image>()[2];
            fishImg.sprite = fish.sprite;
            fishImg.color = Color.white;
            rarityImg.sprite = InventoryUIFiller.Instance.getStarsFromRarity(fishData.highestRarity);
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[0].text = fish.fishName;
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[1].text = "Largest Caught: " + fishData.largestCaught.ToString("0.00") + "cm";
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[2].text = "Amount Caught: " + fishData.amountCaught;

            float randomRotation = Random.Range(-randomRotationRange, randomRotationRange);
            fishImg.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, 0, randomRotation);
            collectionPrefab.GetComponentsInChildren<Image>()[1].GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, 0, randomRotation);
        }

        //random rotation
       
    }

    //finds the collection prefab for the fish, if it doesn't exist then it creates a new one.
    private GameObject findOrCreateCollectionPrefab(string fishName)
    {
        if (fishCollection.TryGetValue(fishName, out GameObject existing))
        {
            return existing;
        }

        GameObject collectionPrefab = Instantiate(fishCollectionUIPrefab, this.transform);
        fishCollection[fishName] = collectionPrefab;
        return collectionPrefab;
    }

}
