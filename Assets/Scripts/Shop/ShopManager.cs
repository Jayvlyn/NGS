using System.Collections.Generic;
using System.Linq;
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

    //private ShopData currentShop;

    [SerializeField, Tooltip("Main shop menu where you can select if you want to buy upgrades or sell fish")]
    private GameObject mainMenuWindow;
    [SerializeField, Tooltip("Window where the types of fish you can sell at the attached shop will be listed")]
    private GameObject selectFishWindow;
    [SerializeField] private RectTransform selectFishDisplayArea;
    [SerializeField, Tooltip("Window where fish you can sell will be placed")]
    private GameObject sellFishWindow;
    [SerializeField] private RectTransform sellFishDisplayArea;
    [SerializeField, Tooltip("Window containing all of the different upgrades that can be bought at the attached shop")]
    private GameObject buyUpgradeWindow;
    [SerializeField] private RectTransform buyUpgradeDisplayArea;

    private ShopState state = ShopState.Closed;
    private List<GameObject> pastFishTiles = new();
    private List<GameObject> pastSelectTiles = new();
    private List<GameObject> pastUpgradeTiles = new();
    private string previousFishType = string.Empty;

    [SerializeField] private Toggle sellAllExcludeLargest;
    [SerializeField] private Toggle sellAllOfTypeExcludeLargest;
    private bool overrideSellAllOfType = false;

    private bool selectTilesOutOfDate = true;
    private bool fishTilesOutOfDate = true;

    [SerializeField] public static List<UpgradeData> upgrades;
    [SerializeField] public static UpgradeData[] baseUpgrades;
    [SerializeField] private UpgradeData[] defaultUpgrades;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerStats baseStats;

    [SerializeField] private WardrobeManager wardrobeManager;
    [SerializeField] private GameObject buyFlannelWindow;
    [SerializeField] private RectTransform buyFlannelDisplayArea;

    //Testing only, remove later
    //[SerializeField, Tooltip("This shold be removed before build as it is only for testing purposes")] private ShopData testingShopData;

    [Header("Particle System")]
    public UIParticleFX carrotParticles;


    public void Close()
    {
        if (state != ShopState.Closed)
        {
			switch (state)
			{
				case ShopState.MainMenu:
			        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", mainMenuWindow, true));
					break;
				case ShopState.SelectFish:
			        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", selectFishWindow, true));
			        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", mainMenuWindow, true));
					break;
				case ShopState.SellFish:
			        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", sellFishWindow, true));
			        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", selectFishWindow, true));
			        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", mainMenuWindow, true));
					break;
				case ShopState.BuyUpgrade:
			        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", buyUpgradeWindow, true));
			        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", mainMenuWindow, true));
					break;
			}

            QuestManager.Instance.UpdateQuests();
            state = ShopState.Closed;
		}
	}

	public void SellAll()
    {
        overrideSellAllOfType = true;
        foreach (string fishType in Inventory.Instance.GetData().Item1.Keys)
        {
            SellAllOfType(fishType, false);
        }
        DestroySelectTiles();
        GenerateSelectTiles();
        overrideSellAllOfType = false;
    }

    public void SellAllOfType(string type, bool refresh = true)
    {
        if(type == string.Empty && previousFishType != string.Empty)
        {
            type = previousFishType;
        }
        if(Inventory.Instance.GetData().Item1[type].currentFish.Count > 0)
        {
            while (Inventory.Instance.GetData().Item1[type].currentFish.Count > 1)
            {
                int removeAt = Inventory.Instance.GetData().Item1[type].currentFish[0].length >
                    Inventory.Instance.GetData().Item1[type].currentFish[1].length ? 1 : 0;
                SellFish(Inventory.Instance.GetData().Item1[type].currentFish[removeAt], false);
            }
            if ((!overrideSellAllOfType && !sellAllOfTypeExcludeLargest.isOn) || 
                (!sellAllExcludeLargest.isOn && overrideSellAllOfType))
            {
                SellFish(Inventory.Instance.GetData().Item1[type].currentFish[0], false);
            }
            if(refresh)
            {
                previousFishType = string.Empty;
                GenerateFishTiles(type);
            }
        }
    }

    public void SellFish(Fish fish, bool refreshAfterwards = true)
    {
        Inventory.Instance.RemoveFish(fish);
        var price = GetFishPrice(fish);
        PurchasedFish(price);
        Inventory.Instance.AddMoney(price);
        if (refreshAfterwards)
        {
            fishTilesOutOfDate = true;
            GenerateFishTiles(previousFishType);
            if(Inventory.Instance.GetFishData(fish.fishName).currentFish.Count == 0)
            {
                CloseFish();
                DestroySelectTiles();
                GenerateSelectTiles();
            }
        }
    }

    private void GenerateFishTiles(string fishType)
    {
        if(previousFishType != fishType || fishTilesOutOfDate)
        {
            if(previousFishType != string.Empty)
            {
                DestroyFishTiles();
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
                    $"{currentFish[current].length:F2} cm long " +
                    $"{currentFish[current].fishName}: " +
                    $"{GetFishPrice(currentFish[current])} Carrots";
                go.GetComponentsInChildren<Image>()[1].sprite = currentFish[current].sprite;
                Fish fish = currentFish[current];
                go.GetComponentInChildren<Button>().onClick.AddListener(delegate { SellFish(fish); });
                pastFishTiles.Add(go);
                previousFishType = fishType;
            }
        }
    }

    public void Open()
    {
        if(GameUI.Instance.pause.activeSelf) return;
        state = ShopState.MainMenu;
        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", mainMenuWindow));
    }

	public void SelectSell()
	{
		StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", selectFishWindow));
        state = ShopState.SelectFish;
		if (pastSelectTiles.Count == 0 || selectTilesOutOfDate)
		{
			GenerateSelectTiles();
			selectTilesOutOfDate = false;
		}
	}

	public void SelectFish(string selectedFish)
    {
        state = ShopState.SellFish;
        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", sellFishWindow));
        GenerateFishTiles(selectedFish);
    }

    public void CloseFish()
    {
        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", sellFishWindow, true));
        state = ShopState.SelectFish;
    }

    public void CloseSelect()
    {
        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", selectFishWindow, true));
        state = ShopState.MainMenu;
    }

    public void CloseUpgrade()
    {
        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", buyUpgradeWindow, true));
        state = ShopState.MainMenu;
    }

    public void OpenUpgrade()
    {
        state = ShopState.BuyUpgrade;
        StartCoroutine(UIAnimations.PlayUIAnim("SlideIn", buyUpgradeWindow));
        if (pastUpgradeTiles.Count == 0)
        {
            GenerateUpgradeTiles();
        }
    }

    public void BuyUpgrade(int id)
    {
        UpgradeData upgrade = null;
        foreach(UpgradeData data in upgrades)
        {
            if(data.Id == id)
            {
                upgrade = data;
                break;
            }
        }
        if (upgrade != null && Inventory.Instance.CanAfford(upgrade.currentCost))
        {
            Inventory.Instance.AddMoney(-upgrade.currentCost);
            upgrade.currentCost = upgrade.isMultiplicativeIncrease ? upgrade.currentCost * upgrade.costIncrease : upgrade.currentCost + upgrade.costIncrease;
            playerStats.Upgrade(upgrade);
            if(upgrade.currentCost > upgrade.maxCostBeforeDelete)
            {
                upgrades.RemoveAt(id);
            }
            DestroyUpgradeTiles();
            GenerateUpgradeTiles();
        }
    }

    private void DestroyUpgradeTiles()
    {
        while (pastUpgradeTiles.Count > 0)
        {
            Destroy(pastUpgradeTiles[0]);
            pastUpgradeTiles.RemoveAt(0);
        }
    }

    private void DestroyFishTiles()
    {
        while (pastFishTiles.Count > 0)
        {
            Destroy(pastFishTiles[0]);
            pastFishTiles.RemoveAt(0);
        }
    }

    private void DestroySelectTiles()
    {
        while (pastSelectTiles.Count > 0)
        {
            Destroy(pastSelectTiles[0]);
            pastSelectTiles.RemoveAt(0);
        }
    }

    private void GenerateSelectTiles()
    {
        var keyArr = Inventory.Instance.GetData().Item1.Keys.ToArray();
        for (int current = 0, offset = 0; current < keyArr.Length; current++)
        {
            string fishName = keyArr[current];
            if (Inventory.Instance.GetFishData(fishName).currentFish.Count == 0)
            {
                offset++;
                continue;
            }
            GameObject go = Instantiate(selectFishUIPrefab, selectFishDisplayArea);
            int row = (current - offset) / expectedSelectFishColumns;
            int column = (current - offset) % expectedSelectFishColumns;
            go.transform.localPosition =
                new Vector3(selectFishUIPrefabMarginData.x * (column + 1) + selectFishUIPrefabSizeData.x * column - 7.5f,
                selectFishUIPrefabMarginData.y * -(row + 1) + selectFishUIPrefabSizeData.y * -row);
            go.GetComponentsInChildren<Image>()[1].sprite = Inventory.Instance.GetFishData(fishName).currentFish[0].sprite;
            go.GetComponentInChildren<TMP_Text>().text = fishName;
            go.GetComponentInChildren<Button>().onClick.AddListener(delegate { SelectFish(fishName); });
            pastSelectTiles.Add(go);
        }
    }

    private void GenerateUpgradeTiles()
    {
        for (int current = 0; current < upgrades.Count; current++)
        {
            UpgradeData data = upgrades[current];
            int column = current % expectedBuyUpgradeColumns;
            int row = current / expectedBuyUpgradeColumns;
            GameObject go = Instantiate(buyUpgradeUIPrefab, buyUpgradeDisplayArea);
            RectTransform trans = go.GetComponent<RectTransform>();
            Vector2 size = trans.offsetMax - trans.offsetMin;
            trans.offsetMax += (buyUpgradeUIPrefabSizeData - size) / 2f;
            trans.offsetMin -= (buyUpgradeUIPrefabSizeData - size) / 2f;
            go.transform.localPosition =
                new Vector3(buyUpgradeUIPrefabMarginData.x * (column + 1) + buyUpgradeUIPrefabSizeData.x * column - 7.5f,
                buyUpgradeUIPrefabMarginData.y * -(row + 1) + buyUpgradeUIPrefabSizeData.y * -row);
            go.GetComponentsInChildren<Image>()[1].sprite = data.sprite;
            go.GetComponentInChildren<TMP_Text>().text = $"{data.upgradeName}: {(data.currentCost):F2} Carrots";
            go.GetComponentInChildren<Button>().onClick.AddListener(delegate { BuyUpgrade(data.Id); });
            pastUpgradeTiles.Add(go);
        }
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
        if(upgrades == null)
        {
            upgrades = new();
            if (baseUpgrades == null)
            {
                baseUpgrades = new UpgradeData[defaultUpgrades.Length];
                for(int i = 0; i < baseUpgrades.Length; i++)
                {
                    baseUpgrades[i] = Instantiate(defaultUpgrades[i]);
                }
            }
            ResetUpgrades();
        }
        ResetStats();
    }

    public void ResetUpgrades()
    {
        upgrades.Clear();
        foreach(UpgradeData upgrade in baseUpgrades)
        {
            upgrades.Add(Instantiate(upgrade));
        }
    }

    public void ResetStats()
    {
        playerStats.CopyFrom(baseStats);
    }

    public void PurchasedFish(int cost)
    {
        Vector3 screenPos = Input.mousePosition;
        carrotParticles.SpawnParticles(cost, screenPos);
    }
    public int GetFishPrice(Fish fish)
    {
        return (int)(fish.length * fish.costPerCM);
    }

    public void CaughtFishHandling(Fish fish)
    {
        selectTilesOutOfDate = true;
        if(fish.fishName == previousFishType)
        {
            fishTilesOutOfDate = true;
        }
    }

    public void OpenBuyFlannels()
    {

    }

}
