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
    [SerializeField] GameObject loadMenu;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject keyBinds;
    [SerializeField] GameObject saveReaction;

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

    [Header("MainMenu")]
    [SerializeField] GameObject Title;
    [SerializeField] GameObject Bunny;

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

        modifySettings.SaveMouseMode();

        LoadBindingOnStart(true);
        LoadBindingOnStart(false);

        StartCoroutine(UIAnimations.PlayUIAnim("Bunny", Bunny, turnOff:true));
        StartCoroutine(UIAnimations.PlayUIAnim("SlideDown", Title, delay:1.7f));
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
        StartCoroutine(UIAnimations.PlaySigleAnim("Fade", saveReaction));
        if (keyBinds.activeSelf) SaveKeyBinds();
        else modifySettings.SaveSettings();
    }

    void createClicked()
    {
        bool created = false;

        if (!loadMenu.activeSelf) loadMenu.SetActive(true);
        created = loadMenu.GetComponent<SaveLoadManager>().Save(characterName.text);
        gameSettings.id = characterName.text;

        if (created)
        {
            loadMenu.SetActive(false);
            characterName.text.Remove(0);
            SceneLoader.LoadScene("GameScene");
            //GameUI.gameStart = true;
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
