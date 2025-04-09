using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : InteractableObject
{
    [SerializeField, Tooltip("Prefab where a button to select fish type will be, one will be generated for each type of fish the attached shop is willing to buy")] 
    private GameObject sellFishUIPrefab;
    [SerializeField, Tooltip("Prefab where a button to sell a fish will be, one will be generated for each fish of the selected type that you can sell")]
    private GameObject selectFishPrefab;
    [SerializeField, Tooltip("Prefab where a button to buy an upgrade will be, one will be generated for each upgrade that you can buy at the attached shop")]
    private GameObject buyUpgradePrefab;

    [SerializeField] private ShopData shop;

    [SerializeField, Tooltip("Main shop menu where you can select if you want to buy upgrades or sell fish")] 
    private Canvas mainMenuWindow;
    [SerializeField, Tooltip("Canvas where the types of fish you can sell at the attached shop will be listed")] 
    private Canvas selectFishWindow;
    [SerializeField, Tooltip("Canvas where fish you can sell will be placed")] 
    private Canvas sellFishWindow;
    [SerializeField, Tooltip("Canvas containing all of the different upgrades that can be bought at the attached shop")]
    private Canvas buyUpgradeWindow;

    private ShopState state = ShopState.Closed;
    private List<GameObject> pastFishTiles = new();
    private List<GameObject> pastSelectTiles = new();
    private List<GameObject> pastUpgradeTiles = new();
    private string previousFishType = string.Empty;

    [SerializeField] private Toggle sellAllExcludeLargest;
    [SerializeField] private Toggle sellAllOfTypeExcludeLargest;

    protected override void Interact(InteractionPair pair)
    {
        if(pair.obj != null && pair.obj.Id == Id)
        {
            mainMenuWindow.enabled = true;
            state = ShopState.MainMenu;
        }
    }

    public void SelectSell()
    {
        mainMenuWindow.enabled = false;
        selectFishWindow.enabled = true;
        if (pastSelectTiles.Count == 0)
        {
            foreach ((string, Sprite) availableFish in shop.GetAvailableFish())
            {
                GameObject go = Instantiate(selectFishPrefab, selectFishWindow.transform);
                go.GetComponentInChildren<Image>().sprite = availableFish.Item2;
                go.GetComponentInChildren<TMP_Text>().text = availableFish.Item1;
                pastSelectTiles.Add(go);
            }
        }
    }

    public void Close()
    {
        mainMenuWindow.enabled = false;
        while (pastSelectTiles.Count > 0)
        {
            Destroy(pastSelectTiles[0]);
            pastSelectTiles.RemoveAt(0);
        }
        while (pastFishTiles.Count > 0)
        {
            Destroy(pastFishTiles[0]);
            pastFishTiles.RemoveAt(0);
        }
        previousFishType = string.Empty;
        while (pastUpgradeTiles.Count > 0)
        {
            Destroy(pastUpgradeTiles[0]);
            pastUpgradeTiles.RemoveAt(0);
        }
    }

    public void SellAll()
    {
        foreach(string fishType in Inventory.FishTypes) {
    }
}
