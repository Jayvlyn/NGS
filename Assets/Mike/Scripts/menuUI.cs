using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class menuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject keyBinds;
    [SerializeField] GameObject pause;

    [Header("Buttons")]
    [SerializeField] Button newGameBtn;
    [SerializeField] Button loadGameBtn;
    [SerializeField] Button settingsBtn;
    [SerializeField] Button quitBtn;
    [SerializeField] Button keyBindBtn;
    [SerializeField] Button saveBtn;
    [SerializeField] Button backBtn;


    void Start()
    {
        if(newGameBtn != null) newGameBtn.onClick.AddListener(() => newGameClicked());
        if(loadGameBtn != null) loadGameBtn.onClick.AddListener(() => loadGameClicked());
        if(settingsBtn != null) settingsBtn.onClick.AddListener(() => settingsClicked());
        if(keyBindBtn != null) keyBindBtn.onClick.AddListener(() => keyBindsClicked());
        if(quitBtn != null) quitBtn.onClick.AddListener(() => quitClicked());
        if(saveBtn != null) saveBtn.onClick.AddListener(() => saveClicked());
        if(backBtn != null) backBtn.onClick.AddListener(() => backClicked());
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) pauseClicked();
    }

    void newGameClicked()
    {
        print("New Game Created");
        startMenu.SetActive(false);
    }
    void loadGameClicked()
    {
        print("loading Game");
        startMenu.SetActive(false);
    }
    void settingsClicked()
    {
        print("settings clicked");
        settings.SetActive(true);
    }

    void pauseClicked()
    {
        pause.SetActive(!pause.activeSelf);
        print("GamePaused");
    }

    void keyBindsClicked()
    {
        print("Back");
        keyBinds.SetActive(true);
    }

    void quitClicked()
    {
        print("quiting application");

        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }

    void backClicked()
    {
        print("Back");
        if (keyBinds.activeSelf) keyBinds.SetActive(false);
        else settings.SetActive(false);
    }

    void saveClicked()
    {
        print("Save");
        //add save functions later on
        if (keyBinds.activeSelf) keyBinds.SetActive(false);
        else settings.SetActive(false);
    }
}
