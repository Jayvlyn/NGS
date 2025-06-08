using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUI : Singleton<GameUI>
{
    [HideInInspector] public static bool gameStart = true;
    [HideInInspector] public static bool loadScreens = true;

    [Header("Panels")]
    [SerializeField] GameObject loadGame;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject keyBinds;
    [SerializeField] public GameObject pause;
    [SerializeField] GameObject Shop;
    [SerializeField] GameObject inventoryMenu;
    [SerializeField] public GameObject HUD;
    [SerializeField] GameObject saveReaction;
    [SerializeField] GameObject wardrobe;
    [SerializeField] GameObject comic;

    [Header("Buttons&Inputs")]
    [SerializeField] Button keyBindBtn;
    [SerializeField] Button saveBtn;
    [SerializeField] Button backBtn;

    [Header("Player")]
    [SerializeField] InputActionReference pauseAction;
    [SerializeField] public PlayerInput pi;
    [SerializeField] public InventoryUIFiller inventoryUIFiller;
    [SerializeField] public QuestUIFiller questUIFiller;
    [SerializeField] public Collection collection;
    [HideInInspector] public GameSettings gameSettings;
    private ModifySettings modifySettings;
    private Vector3 oldPosition;
    public Material playerOutfit;


    void Start()
    {
        if (keyBindBtn != null) keyBindBtn.onClick.AddListener(() => keyBindsClicked());
        if (saveBtn != null) saveBtn.onClick.AddListener(() => saveClicked());
        if (backBtn != null) backBtn.onClick.AddListener(() => backClicked());
        foreach (Button btn in pause.GetComponentsInChildren<Button>())
        {
            if (btn.name == "ResumeBtn") btn.onClick.AddListener(() => pauseClicked());
            else if (btn.name == "QuitBtn")
            {
                btn.onClick.AddListener(() => AutoSave());
                btn.onClick.AddListener(() => quitClicked());
            }
            else if (btn.name == "SettingsBtn") btn.onClick.AddListener(() => settingsClicked());
            else
            {
                btn.onClick.AddListener(() => AutoSave());
                btn.onClick.AddListener(() => MainMenuClicked());
            }
        }

        modifySettings = GetComponent<ModifySettings>();

        gameSettings = modifySettings.settings;

        if (loadScreens) DayNightCycle.Instance.currentTime = gameSettings.location.currentTime;

        HUD.SetActive(loadScreens);

        if(gameStart && SaveLoadManager.selected == -1)
        {
            comic.SetActive(true);
        }

        if (pi != null)
        {
            SavePosition(gameStart);
            LoadPosition();
            gameStart = false;
            loadScreens = false;
        }

        if (SaveLoadManager.selected != -1)
        {
            StartCoroutine(Fade.Instance.FadeOut(2f));
            loadGame.SetActive(true);
            loadGame.GetComponent<SaveLoadManager>().Load();
            loadGame.SetActive(false);
        }
        else
        {
            inventoryMenu.SetActive(true);
        }

        LoadBindingOnStart(true);
        LoadBindingOnStart(false);

        LoadFlannel();
    }

    void Update()
    {
        if (pauseAction.action.triggered && !Shop.activeSelf && !wardrobe.activeSelf && !comic.activeSelf) pauseClicked();
        //if (Input.GetKeyDown(KeyCode.V)) Fade.Instance.StartFade(8,2);
    }

    private void FixedUpdate()
    {
        if (pi != null)
        {
            gameSettings.currentPos.x = pi.transform.localPosition.x;
            gameSettings.currentPos.y = pi.transform.localPosition.y;
        }
    }

    public void MainMenuClicked()
    {
        pauseClicked(1);
        gameStart = true;
        HUD.SetActive(loadScreens);
        loadGame.SetActive(false);
        Inventory.Instance.RestInventory();
        StartCoroutine(SceneLoader.LoadScene("MainMenu"));
    }

    void settingsClicked()
    {
        StartCoroutine(UIAnimations.PlayUIAnim("SlideDown", settings));
    }

    void pauseClicked(int delay = 0)
    {
        Time.timeScale = (!pause.activeSelf) ? 0 : 1;
        StartCoroutine(UIAnimations.PlayUIAnim("SlideDown", pause, true, delay));
    }

    void keyBindsClicked()
    {
        StartCoroutine(UIAnimations.PlayUIAnim("SlideDown", keyBinds));
    }

    void quitClicked()
    {
        #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
        #else
                Application.Quit();
        #endif
    }

    void backClicked()
    {
        if (keyBinds.activeSelf) StartCoroutine(UIAnimations.PlayUIAnim("SlideDown", keyBinds, true));
        else StartCoroutine(UIAnimations.PlayUIAnim("SlideDown", settings, true));
    }

    void saveClicked()
    {
        StartCoroutine(UIAnimations.PlaySigleAnim("Fade", saveReaction));
        if (keyBinds.activeSelf) SaveKeyBinds();
        else modifySettings.SaveSettings();
    }


    public void OnInventory()
    {
        StartCoroutine(UIAnimations.PlayUIAnim("SlideLeft", inventoryMenu, true));
    }

    private void SaveKeyBinds()
    {
        gameSettings.platformerKeys.Clear();
        gameSettings.minigameKeys.Clear();
        gameSettings.bossGameKeys.Clear();

        foreach (var bind in keyBinds.GetComponentsInChildren<KeyRebinder>(includeInactive: true))
        {
            switch (bind.data.actionMap)
            {
                case 0:
                    gameSettings.platformerKeys.Add(bind.data);
                    break;
                case 1:
                    gameSettings.minigameKeys.Add(bind.data);
                    break;
                case 2:
                    gameSettings.bossGameKeys.Add(bind.data);
                    break;
            }
        }

        modifySettings.SaveMouseMode();
    }

    public void LoadSaveGame()
    {
        Time.timeScale = 1;
        modifySettings.ApplyData();

        int i = 0;
        foreach (var bind in keyBinds.GetComponentsInChildren<KeyRebinder>(includeInactive: true))
        {
            print(bind.gameObject.name);
            if (i > 12) bind.data = gameSettings.bossGameKeys[i - 13];
            else if (i > 8) bind.data = gameSettings.minigameKeys[i - 9];
            else bind.data = gameSettings.platformerKeys[i];

            bind.ApplyData();

            i++;
        }
        oldPosition = new Vector3(gameSettings.currentPos.x, gameSettings.currentPos.y, 0f);
        pi.transform.localPosition = oldPosition;
        inventoryMenu.gameObject.SetActive(true);
        DayNightCycle.Instance.currentTime = gameSettings.location.currentTime;
        if (loadGame.activeSelf) loadGame.SetActive(false);
    }

    public void AutoSave()
    {
        loadGame.SetActive(true);
        gameSettings.location.currentTime = DayNightCycle.Instance.currentTime;
        loadGame.GetComponent<SaveLoadManager>().autoSave();
        loadGame.SetActive(false);
    }

    public void LoadMinigame(GameObject go)
    {
        if (pause.activeSelf) return;
        StartCoroutine(UIAnimations.PlayUIAnim("SlideUp", go, go.activeSelf));
    }

    public void LoadPosition()
    {
        pi.transform.localPosition = oldPosition;
    }

    public void SavePosition(bool reset = false)
    {
        if (reset) oldPosition = new Vector3(-5.3f, -4.2f, 0f);
        else oldPosition = new Vector3(gameSettings.currentPos.x, gameSettings.currentPos.y, 0f);
    }

    public void LoadBindingOnStart(bool active)
    {
        settings.SetActive(active);
        keyBinds.SetActive(active);
        var tg = keyBinds.GetComponentInChildren<TabGroup>();

        switch (tg.selectedTab.name)
        {
            case "Tab1":
                tg.objectsToSwap[1].SetActive(active);
                tg.objectsToSwap[2].SetActive(active);
                break;
            case "Tab2":
                tg.objectsToSwap[0].SetActive(active);
                tg.objectsToSwap[2].SetActive(active);
                break;
            case "Tab3":
                tg.objectsToSwap[0].SetActive(active);
                tg.objectsToSwap[1].SetActive(active);
                break;
        }
    }

    public void SaveFlannel(string name) => gameSettings.flannel = name;

    private void LoadFlannel() => playerOutfit.SetTexture("_Swap", Resources.Load<Texture>($"flannels/{((gameSettings.flannel != "") ? gameSettings.flannel : "Hoppepalette1")}"));
}
