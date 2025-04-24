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
        //fish outlines for fish that havent been caught
        if (fishData.amountCaught == 0)
        {
            //get images
            Image bgImg = collectionPrefab.GetComponentsInChildren<Image>()[0];
            Image polaroid = collectionPrefab.GetComponentsInChildren<Image>()[1];
            Image fishImg = collectionPrefab.GetComponentsInChildren<Image>()[3];
            Image rarityImg = collectionPrefab.GetComponentsInChildren<Image>()[2];

            //set image properties
            fishImg.sprite = fish.sprite;
            fishImg.color = Color.black; // Set the image to black if no fish caught
            bgImg.color = Color.clear;
            rarityImg.sprite = starSpriteEmpty;
            rarityImg.color = Color.clear;
            polaroid.color = Color.clear;
            string questionmarks = string.Empty;
            foreach (char a in fish.fishName)
            { 
                questionmarks += "?"; // Create a string of question marks equal to the length of the fish name 
            }
            //set text
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[0].text = questionmarks;
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[1].text = "Largest Caught: ????";
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[2].text = "Amount Caught: " + fishData.amountCaught;

            //rotation
            float randomRotation = Random.Range(-randomRotationRange, randomRotationRange);
            fishImg.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, 0, randomRotation);
            polaroid.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, 0, randomRotation);
            bgImg.GetComponentsInChildren<Image>()[0].GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, 0, randomRotation);
        }
        //fish that have been caught updated.
        else {
            //get images
            Image bgImg = collectionPrefab.GetComponentsInChildren<Image>()[0];
            Image polaroid = collectionPrefab.GetComponentsInChildren<Image>()[1];
            Image fishImg = collectionPrefab.GetComponentsInChildren<Image>()[3];
            Image rarityImg = collectionPrefab.GetComponentsInChildren<Image>()[2];

            //set image properties
            
            bgImg.color = new Color(.96f, .68f, .18f);
            fishImg.sprite = fish.sprite;
            fishImg.color = Color.white;
            polaroid.color = Color.white;
            rarityImg.sprite = InventoryUIFiller.Instance.getStarsFromRarity(fishData.highestRarity);

            //set text
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[0].text = fish.fishName;
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[1].text = "Largest Caught: " + fishData.largestCaught.ToString("0.00") + "cm";
            collectionPrefab.GetComponentsInChildren<TMP_Text>()[2].text = "Amount Caught: " + fishData.amountCaught;

            //random rotation
            float randomRotation = Random.Range(-randomRotationRange, randomRotationRange);
            fishImg.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, 0, randomRotation);
            bgImg.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, 0, randomRotation);
            polaroid.GetComponentInParent<Transform>().rotation = Quaternion.Euler(0, 0, randomRotation);
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
