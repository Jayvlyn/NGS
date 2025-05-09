using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : Singleton<MenuUI>
{
    public static bool gameLoaded = false;

    [Header("Panels")]
    [SerializeField] GameObject characterCreation;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject loadMenu;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject keyBinds;

    [Header("Buttons&Inputs")]
    [SerializeField] Button newGameBtn;
    [SerializeField] Button loadGameBtn;
    [SerializeField] Button settingsBtn;
    [SerializeField] Button quitBtn;
    [SerializeField] Button keyBindBtn;
    [SerializeField] Button saveBtn;
    [SerializeField] Button backBtn;
    [SerializeField] Button createBtn;
    [SerializeField] TMP_InputField characterName;

    [Header("Player")]
    [SerializeField] InputActionReference pauseAction;
    public PlayerInput pi;

    private GameSettings gameSettings;
    private ModifySettings modifySettings;
    private Vector3 oldPosition;


    void Start()
    {
        if (gameLoaded) startMenu.SetActive(false);
        else
        {
            startMenu.SetActive(true);
			Time.timeScale = 0;
		}

        if(newGameBtn != null) newGameBtn.onClick.AddListener(() => newGameClicked());
        if(loadGameBtn != null) loadGameBtn.onClick.AddListener(() => loadGameClicked());
        if(settingsBtn != null) settingsBtn.onClick.AddListener(() => settingsClicked());
        if(keyBindBtn != null) keyBindBtn.onClick.AddListener(() => keyBindsClicked());
        if(quitBtn != null) quitBtn.onClick.AddListener(() => quitClicked());
        if(saveBtn != null) saveBtn.onClick.AddListener(() => saveClicked());
        if(backBtn != null) backBtn.onClick.AddListener(() => backClicked());
        if(createBtn != null) createBtn.onClick.AddListener(() => createClicked());
        //foreach (Button btn in pause.GetComponentsInChildren<Button>())
        //{
        //    if(btn.name == "ResumeBtn") btn.onClick.AddListener(() => pauseClicked());
        //    else if(btn.name == "QuitBtn")
        //    {
        //        btn.onClick.AddListener(() => SaveOnQuit());
        //        btn.onClick.AddListener(() => quitClicked());
        //    }
        //    else if(btn.name == "SettingsBtn") btn.onClick = settingsBtn.onClick;
        //    else btn.onClick.AddListener(() => MainMenuClicked());
        //}

        modifySettings = GetComponent<ModifySettings>();

		gameSettings = modifySettings.settings;

        if(pi != null)
        {
            SavePosition();
            LoadPosition();
        }

	}

    void Update()
    {
        //if (pauseAction.action.triggered) pauseClicked();
    }

    private void FixedUpdate()
    {
        if(pi != null)
        {
            gameSettings.position.x = pi.transform.localPosition.x;
            gameSettings.position.y = pi.transform.localPosition.y;
        }
    }

    void newGameClicked()
    {
        StartCoroutine(PlayUIAnim("SlideUp", characterCreation));
    }

    void loadGameClicked()
    {
        StartCoroutine(PlayUIAnim("SlideRight", loadMenu, true));
    }

    void settingsClicked()
    {
        StartCoroutine(PlayUIAnim("SlideDown", settings));
    }

    void keyBindsClicked()
    {
        StartCoroutine(PlayUIAnim("SlideDown", keyBinds));
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
        if (keyBinds.activeSelf)
        {
            StartCoroutine(PlayUIAnim("SlideDown", keyBinds, true));
        }
        else
        {
            StartCoroutine(PlayUIAnim("SlideDown", settings, true));
        }
    }

    void saveClicked()
    {
        if (keyBinds.activeSelf)
        {
            SaveKeyBinds();
        }
        else
        {
            modifySettings.SaveSettings();
        }
    }

    void createClicked()
    {
        bool created = false;

        if (!loadMenu.activeSelf) loadMenu.SetActive(true);
        created = GetComponentInChildren<SaveLoadManager>().Save(characterName.text);

        if (created)
        {
            SceneManager.LoadScene("GameScene");
            SavePosition(true);
            LoadPosition();
            loadMenu.SetActive(false);
            startMenu.SetActive(false);
            gameLoaded = true;
            Time.timeScale = 1;
            transform.Find("InventoryCollection").gameObject.SetActive(true);
            StartCoroutine(PlayUIAnim("SlideUp", characterCreation, true));
        }
        else
        {
            characterCreation.transform.Find("ErrorMsg").gameObject.SetActive(true);
        }
    }

    public void ExitCreateCharacter()
    {
        loadMenu.SetActive(false);
        startMenu.SetActive(true);
        StartCoroutine(PlayUIAnim("SlideUp", characterCreation, true));
    }

    //public void OnInventory()
    //{
    //    StartCoroutine(PlayUIAnim("SlideLeft", inventoryMenu, true));
    //}

    private void SaveKeyBinds()
    {
        var settings = gameSettings;

        settings.platformerKeys.Clear();
        settings.minigameKeys.Clear();
        settings.bossGameKeys.Clear();

        foreach (var bind in keyBinds.GetComponentsInChildren<KeyRebinder>(includeInactive: true))
        {
            switch (bind.data.actionMap)
            {
                case 0:
                    settings.platformerKeys.Add(bind.data);
                    break;
                case 1:
                    settings.minigameKeys.Add(bind.data);
                    break;
                case 2:
                    settings.bossGameKeys.Add(bind.data);
                    break;
            }
        }

        modifySettings.SaveMouseMode();
    }

    public void LoadSaveGame()
    {
        loadMenu.SetActive(false);
        startMenu.SetActive(false);
        gameLoaded = true;
        Time.timeScale = 1;
        modifySettings.ApplyData();

        int i = 0;
        foreach (var bind in keyBinds.GetComponentsInChildren<KeyRebinder>(includeInactive: true))
        {
            print(bind.gameObject.name);
            if(i > 12) bind.data = gameSettings.bossGameKeys[i-13];
            else if (i > 8) bind.data = gameSettings.minigameKeys[i-9];
            else bind.data = gameSettings.platformerKeys[i];

            bind.ApplyData();
            
            i++;
        }
        Vector3 oldPosition = new Vector3(gameSettings.position.x, gameSettings.position.y, 0f);
        pi.transform.localPosition = oldPosition;
        transform.Find("InventoryCollection").gameObject.SetActive(true);
    }

    public void SaveOnQuit()
    {
        loadMenu.SetActive(true);
        GetComponentInChildren<SaveLoadManager>().autoSave();
    }

    public IEnumerator PlayUIAnim(string name, GameObject menu, bool Both = false)
    {
        var anim = menu.GetComponent<Animator>();

        if (anim.GetBool(name)) anim.SetBool(name, false);
        else
        {
            menu.SetActive(true);
            anim.SetBool(name, true);
        }

        if (Both)
        {
            yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

            if (!anim.GetBool(name)) menu.SetActive(false);
        }
        else yield return new WaitForSecondsRealtime(1);
    }

    public void LoadMinigame(GameObject go)
    {
        StartCoroutine(PlayUIAnim("SlideUp", go, go.activeSelf));
    }

    public void LoadPosition()
    {
        pi.transform.localPosition = oldPosition;
    }

    public void SavePosition(bool reset = false)
    {
        if(reset) oldPosition = new Vector3(-5.3f, -4.2f, 0f);
        else oldPosition = new Vector3(gameSettings.position.x, gameSettings.position.y, 0f);
    }
}
