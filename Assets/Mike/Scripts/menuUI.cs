using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject characterCreation;
    [SerializeField] GameObject lodingScreen;
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
    public PlayerInput pi;

    private GameSettings gameSettings;
    private ModifySettings modifySettings;


    void Start()
    {
        if(newGameBtn != null) newGameBtn.onClick.AddListener(() => newGameClicked());
        if(loadGameBtn != null) loadGameBtn.onClick.AddListener(() => loadGameClicked());
        if(settingsBtn != null) settingsBtn.onClick.AddListener(() => settingsClicked());
        if(keyBindBtn != null) keyBindBtn.onClick.AddListener(() => keyBindsClicked());
        if(quitBtn != null) quitBtn.onClick.AddListener(() => quitClicked());
        if(saveBtn != null) saveBtn.onClick.AddListener(() => saveClicked());
        if(backBtn != null) backBtn.onClick.AddListener(() => backClicked());
        if(createBtn != null) createBtn.onClick.AddListener(() => createClicked());

        modifySettings = GetComponent<ModifySettings>();

		gameSettings = modifySettings.settings;
	}

    void newGameClicked()
    {
        StartCoroutine(UIAnimations.PlayUIAnim("SlideUp", characterCreation));
    }

    void loadGameClicked()
    {
        StartCoroutine(UIAnimations.PlayUIAnim("SlideRight", loadMenu, true));
    }

    void settingsClicked()
    {
        StartCoroutine(UIAnimations.PlayUIAnim("SlideDown", settings));
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
        if (keyBinds.activeSelf) SaveKeyBinds();
        else modifySettings.SaveSettings();
    }

    void createClicked()
    {
        bool created = false;

        if (!loadMenu.activeSelf) loadMenu.SetActive(true);
        created = SaveLoadManager.Instance.Save(characterName.text);
        gameSettings.id = characterName.text;

        if (created)
        {
            loadMenu.SetActive(false);
            characterName.text.Remove(0);
            SceneManager.LoadScene("TestGame");
            //play loading screen while switching to game
        }
        else
        {
            characterName.text.Remove(0);
            characterCreation.transform.Find("ErrorMsg").gameObject.SetActive(true);
        }
    }

    public void ExitCreateCharacter()
    {
        loadMenu.SetActive(false);
        StartCoroutine(UIAnimations.PlayUIAnim("SlideUp", characterCreation, true));
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
}
