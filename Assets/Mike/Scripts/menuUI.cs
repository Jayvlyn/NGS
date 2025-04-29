using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject characterCreation;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject loadMenu;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject keyBinds;
    [SerializeField] GameObject pause;
    [SerializeField] GameObject inventoryMenu;

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
        foreach (Button btn in pause.GetComponentsInChildren<Button>())
        {
            if(btn.name == "ResumeBtn") btn.onClick.AddListener(() => pauseClicked());
            else if(btn.name == "QuitBtn")
            {
                btn.onClick.AddListener(() => SaveOnQuit());
                btn.onClick.AddListener(() => quitClicked());
            }
            else btn.onClick = settingsBtn.onClick;
        }
    }

    void Update()
    {
        if (pauseAction.action.triggered) pauseClicked();
    }

    private void FixedUpdate()
    {
        GetComponent<ModifySettings>().settings.position.x = pi.transform.localPosition.x;
        GetComponent<ModifySettings>().settings.position.y = pi.transform.localPosition.y;
    }

    void newGameClicked()
    {
        if (!loadMenu.activeSelf) loadMenu.SetActive(true);
        characterCreation.SetActive(true);
        startMenu.SetActive(false);
    }
    void loadGameClicked()
    {
        loadMenu.SetActive(!loadMenu.activeSelf);
    }
    void settingsClicked()
    {
        settings.SetActive(true);
    }

    void pauseClicked()
    {
        pause.SetActive(!pause.activeSelf);
        Time.timeScale = (pause.activeSelf) ? 0 : 1;
    }

    void keyBindsClicked()
    {
        keyBinds.SetActive(true);
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
        if (keyBinds.activeSelf) keyBinds.SetActive(false);
        else settings.SetActive(false);
    }

    void saveClicked()
    {
        if (keyBinds.activeSelf)
        {
            //keyBinds.SetActive(false);
            SaveKeyBinds();
        }
        else
        {
            GetComponent<ModifySettings>().SaveSettings();
            //settings.SetActive(false);
        }
    }

    void createClicked()
    {
        bool created = false;

        created = GetComponentInChildren<SaveLoadManager>().Save(characterName.text);

        if (created)
        {
            loadMenu.SetActive(false);
            characterCreation.SetActive(false);
        }
        else
        {
            characterCreation.transform.Find("ErrorMsg").gameObject.SetActive(true);
        }
    }

    public void OnInventory()
    { 
        if (inventoryMenu.activeSelf)
        {
            inventoryMenu.SetActive(false);
        }
        else
        {
            inventoryMenu.SetActive(true);
        }
    }

    private void SaveKeyBinds()
    {
        var settings = GetComponent<ModifySettings>().settings;
        switch (keyBinds.GetComponentInChildren<KeyRebinder>().data.actionMap)
        {
            case 0:
                settings.platformerKeys.Clear();
                break;
            case 1:
                settings.minigameKeys.Clear();
                break;
            case 2:
                settings.bossGameKeys.Clear();
                break;
        }

        foreach (var bind in keyBinds.GetComponentsInChildren<KeyRebinder>())
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
    }

    public void LoadSaveGame()
    {
        loadMenu.SetActive(false);
        startMenu.SetActive(false);
        GetComponent<ModifySettings>().ApplyData();
        foreach (var binds in keyBinds.transform.Find("PageArea").GetComponentsInChildren<Image>())
        {
            foreach (var bind in binds.GetComponentsInChildren<KeyRebinder>()) bind.ApplyData();
        }
        Vector3 oldPosition = new Vector3(GetComponent<ModifySettings>().settings.position.x, GetComponent<ModifySettings>().settings.position.y, 0f);
        pi.transform.localPosition = oldPosition;
    }

    public void SaveOnQuit()
    {
        loadMenu.SetActive(true);
        GetComponentInChildren<SaveLoadManager>().autoSave();
    }
}
