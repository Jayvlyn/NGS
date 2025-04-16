using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField, Tooltip("Prefab where a button to select fish type will be, one will be generated for each type of fish the attached shop is willing to buy")]
    private GameObject sellFishUIPrefab;
    [SerializeField] private Vector2 sellFishUIPrefabMarginData;
    [SerializeField] private Vector2 sellFishUIPrefabSizeData;
    [SerializeField] private int expectedSellFishColumns = 3;
    [SerializeField, Tooltip("Prefab where a button to sell a fish will be, one will be generated for each fish of the selected type that you can sell")]
    private GameObject selectFishUIPrefab;
    [SerializeField] private Vector2 selectFishUIPrefabMarginData;
    [SerializeField] private Vector2 selectFishUIPrefabSizeData;
    [SerializeField] private int expectedSelectFishColumns = 3;
    [SerializeField, Tooltip("Prefab where a button to buy an upgrade will be, one will be generated for each upgrade that you can buy at the attached shop")]
    private GameObject buyUpgradeUIPrefab;
    [SerializeField] private Vector2 buyUpgradeUIPrefabMarginData;
    [SerializeField] private Vector2 buyUpgradeUIPrefabSizeData;
    [SerializeField] private int expectedBuyUpgradeColumns = 3;

    private ShopData currentShop;

    [SerializeField, Tooltip("Main shop menu where you can select if you want to buy upgrades or sell fish")]
    private Canvas mainMenuWindow;
    [SerializeField, Tooltip("Canvas where the types of fish you can sell at the attached shop will be listed")]
    private Canvas selectFishWindow;
    [SerializeField] private RectTransform selectFishDisplayArea;
    [SerializeField, Tooltip("Canvas where fish you can sell will be placed")]
    private Canvas sellFishWindow;
    [SerializeField] private RectTransform sellFishDisplayArea;
    [SerializeField, Tooltip("Canvas containing all of the different upgrades that can be bought at the attached shop")]
    private Canvas buyUpgradeWindow;
    [SerializeField] private RectTransform buyUpgradeDisplayArea;

    private ShopState state = ShopState.Closed;
    private List<GameObject> pastFishTiles = new();
    private List<GameObject> pastSelectTiles = new();
    private List<GameObject> pastUpgradeTiles = new();
    private string previousFishType = string.Empty;

    [SerializeField] private Toggle sellAllExcludeLargest;
    [SerializeField] private Toggle sellAllOfTypeExcludeLargest;
    private bool overrideSellAllOfType = false;


    //Testing only, remove later
    [SerializeField, Tooltip("This shold be removed before build as it is only for testing purposes")] private ShopData testingShopData;

    public void SelectSell()
    {
        mainMenuWindow.enabled = false;
        selectFishWindow.enabled = true;
        if (pastSelectTiles.Count == 0)
        {
            for(int current = 0; current < currentShop.GetAvailableFish().Count; current++)
            {
                (string, Sprite) data = currentShop.GetAvailableFish()[current];
                GameObject go = Instantiate(selectFishUIPrefab, selectFishDisplayArea);
                int row = current / expectedSelectFishColumns;
                int column = current % expectedSelectFishColumns;
                go.transform.localPosition = 
                    new Vector3(selectFishUIPrefabMarginData.x * (column + 1) + selectFishUIPrefabSizeData.x * column - 7.5f, 
                    selectFishUIPrefabMarginData.y * -(row + 1) + selectFishUIPrefabSizeData.y * -row);
                go.GetComponentsInChildren<Image>()[1].sprite = data.Item2;
                go.GetComponentInChildren<TMP_Text>().text = data.Item1;
                go.GetComponentInChildren<Button>().onClick.AddListener(delegate { SelectFish(data.Item1); });
                pastSelectTiles.Add(go);
            }
        }
    }

    public void Close()
    {
        mainMenuWindow.enabled = false;
        state = ShopState.Closed;
    }

    public void SellAll()
    {
        overrideSellAllOfType = true;
        foreach (string fishType in Inventory.Instance.GetData().Item1.Keys)
        {
            if(currentShop.BuysFish(fishType))
            {
                SellAllOfType(fishType);
            }
        }
        overrideSellAllOfType = false;
    }

    public void SellAllOfType(string type)
    {
        if(type == string.Empty && previousFishType != string.Empty)
        {
            type = string.Empty;
        }
        if(Inventory.Instance.GetData().Item1[type].currentFish.Count > 0)
        {
            while (Inventory.Instance.GetData().Item1[type].currentFish.Count > 1)
            {
                int removeAt = Inventory.Instance.GetData().Item1[type].currentFish[0].length >
                    Inventory.Instance.GetData().Item1[type].currentFish[1].length ? 1 : 0;
                SellFish(Inventory.Instance.GetData().Item1[type].currentFish[removeAt]);
            }
            if ((!overrideSellAllOfType && !sellAllOfTypeExcludeLargest) || 
                (!sellAllExcludeLargest.isOn && overrideSellAllOfType))
            {
                SellFish(Inventory.Instance.GetData().Item1[type].currentFish[0]);
            }
        }
    }

    public void SellFish(Fish fish)
    {
        Inventory.Instance.RemoveFish(fish);
        Inventory.Instance.AddMoney(currentShop.GetFishPrice(fish));
        if (fish.name == previousFishType)
        {
            while (pastFishTiles.Count > 0)
            {
                Destroy(pastFishTiles[0]);
                pastFishTiles.RemoveAt(0);
            }
            if(state == ShopState.SellFish)
            {
                GenerateFishTiles(previousFishType);
            }
            else
            {
                previousFishType = string.Empty;
            }
        }
    }

    private void GenerateFishTiles(string fishType)
    {
        if(previousFishType != fishType)
        {
            if(previousFishType != string.Empty)
            {
                while(pastFishTiles.Count > 0)
                {
                    Destroy(pastFishTiles[0]);
                    pastFishTiles.RemoveAt(0);
                }
            }
            List<Fish> currentFish = Inventory.Instance.GetFishData(fishType).currentFish;
            for (int current = 0; current < currentFish.Count; current++)
            {
                GameObject go = Instantiate(sellFishUIPrefab, sellFishDisplayArea);
                int column = current % expectedSellFishColumns;
                int row = current / expectedSellFishColumns;
                go.transform.localPosition =
                    new Vector3(sellFishUIPrefabMarginData.x * (column + 1) + column * sellFishUIPrefabSizeData.x - 7.5f,
                    sellFishUIPrefabMarginData.y * -(row + 1) + -row * sellFishUIPrefabSizeData.y);
                go.GetComponentInChildren<TMP_Text>().text = 
                    $"{currentFish[current].length:2F} cm long " +
                    $"{currentFish[current].name}: " +
                    $"{currentShop.GetFishPrice(currentFish[current]):2F} Gold";
                go.GetComponentsInChildren<Image>()[1].sprite = currentFish[current].sprite;
                go.GetComponentInChildren<Button>().onClick.AddListener(delegate { SellFish(currentFish[current]); });
                pastFishTiles.Add(go);
            }
        }
    }

    public void Open(ShopData shopData)
    {
        if (currentShop != null)
        {
            if (currentShop.Id != shopData.Id)
            {
                currentShop = shopData;
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
        }
        else
        {
            currentShop = shopData;
        }
        mainMenuWindow.enabled = true;
        state = ShopState.MainMenu;
    }

    public void SelectFish(string selectedFish)
    {
        state = ShopState.SellFish;
        sellFishWindow.enabled = true;
        selectFishWindow.enabled = false;
        GenerateFishTiles(selectedFish);
    }

    public void CloseFish()
    {
        sellFishWindow.enabled = false;
        selectFishWindow.enabled = true;
        state = ShopState.SelectFish;
    }

    public void CloseSelect()
    {
        selectFishWindow.enabled = false;
        mainMenuWindow.enabled = true;
        state = ShopState.MainMenu;
    }

    public void CloseUpgrade()
    {
        buyUpgradeWindow.enabled = false;
        mainMenuWindow.enabled = true;
        state = ShopState.MainMenu;
    }

    public void OpenUpgrade()
    {
        state = ShopState.BuyUpgrade;
        buyUpgradeWindow.enabled = true;
        mainMenuWindow.enabled = false;
        if (pastUpgradeTiles.Count == 0)
        {
            for (int current = 0; current < currentShop.GetUpgrades().Count; current++)
            {
                UpgradeData data = currentShop.GetUpgrades()[current];
                int column = current % expectedBuyUpgradeColumns;
                int row = current / expectedBuyUpgradeColumns;
                GameObject go = Instantiate(buyUpgradeUIPrefab, buyUpgradeDisplayArea);
                go.transform.localPosition =
                    new Vector3(buyUpgradeUIPrefabMarginData.x * (column + 1) + buyUpgradeUIPrefabSizeData.x * column - 7.5f, 
                    buyUpgradeUIPrefabMarginData.y * -(row + 1) + buyUpgradeUIPrefabSizeData.y * -row);
                go.GetComponentsInChildren<Image>()[1].sprite = data.sprite;
                go.GetComponentInChildren<TMP_Text>().text = $"{data.name}: {(data.currentCost):2F} Gold";
                go.GetComponentInChildren<Button>().onClick.AddListener(delegate { BuyUpgrade(data.Id); });
                pastUpgradeTiles.Add(go);
            }
        }
    }

    public void BuyUpgrade(int id)
    {
        currentShop.PurchaseUpgrade(id);
    }

    public void Update()
    {
        switch(state)
        {
            case ShopState.MainMenu:
                {
                    if(Input.GetKeyDown(KeyCode.Escape))
                    {
                        Close();
                    }
                    break;
                }
            case ShopState.SelectFish:
                {
                    if(Input.GetKeyDown(KeyCode.Escape))
                    {
                        CloseSelect();
                    }
                    else if(Input.GetKeyDown(KeyCode.Return))
                    {
                        SellAll();
                    }
                    break;
                }
            case ShopState.SellFish:
                {
                    if(Input.GetKeyDown(KeyCode.Escape))
                    {
                        CloseFish();
                    }
                    else if(Input.GetKeyDown(KeyCode.Return))
                    {
                        SellAllOfType(string.Empty);
                    }
                    break;
                }
            case ShopState.BuyUpgrade:
                {
                    if(Input.GetKeyDown(KeyCode.Escape))
                    {
                        CloseUpgrade();
                    }
                    break;
                }
            default:
                break;
        }
    }

    private void Start()
    {
        //testing only, remove later
        Open(testingShopData);
    }
}
