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

    [Header("")]
    [SerializeField] DayNightCycle timeCycle;

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

        HUD.SetActive(loadScreens);
        //if(!GameUI.loadScreens) currentTime = settings.position.currentTime;

        if (pi != null)
        {
            SavePosition(gameStart);
            LoadPosition();
            gameStart = false;
            loadScreens = false;
        }

        if (SaveLoadManager.selected != -1)
        {
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
    }

    void Update()
    {
        if (pauseAction.action.triggered && !Shop.activeSelf) pauseClicked();
    }

    private void FixedUpdate()
    {
        if (pi != null)
        {
            gameSettings.position.x = pi.transform.localPosition.x;
            gameSettings.position.y = pi.transform.localPosition.y;
        }
    }

    public void MainMenuClicked()
    {
        pauseClicked();
        gameStart = true;
        HUD.SetActive(loadScreens);
        loadGame.SetActive(false);
        Inventory.Instance.RestInventory();
        SceneLoader.LoadScene("MainMenu");
    }

    void settingsClicked()
    {
        StartCoroutine(UIAnimations.PlayUIAnim("SlideDown", settings));
    }

    void pauseClicked()
    {
        Time.timeScale = (!pause.activeSelf) ? 0 : 1;
        StartCoroutine(UIAnimations.PlayUIAnim("SlideDown", pause, true));
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
        oldPosition = new Vector3(gameSettings.position.x, gameSettings.position.y, 0f);
        pi.transform.localPosition = oldPosition;
        inventoryMenu.gameObject.SetActive(true);
        timeCycle.currentTime = gameSettings.position.currentTime;
        if (loadGame.activeSelf) loadGame.SetActive(false);
    }

    public void AutoSave()
    {
        loadGame.SetActive(true);
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
        else oldPosition = new Vector3(gameSettings.position.x, gameSettings.position.y, 0f);
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
}
