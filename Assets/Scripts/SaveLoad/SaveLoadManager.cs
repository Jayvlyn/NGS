using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject savePrefab;
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private int columns = 3;
    private readonly List<GameObject> options = new();
    private readonly List<SaveData> saveList = new();
    [HideInInspector] public static int selected = -1;
    private string id = "";
    private GridLayoutGroup layout;
    public bool Save(string name, bool newGame = true)
    {
        if(newGame) foreach (var save in saveList) if (save.id.ToLower() == name.ToLower()) return false;

        SaveData data = new(name);
        (SerializedDictionary<string, FishData>, double) inventoryData= Inventory.Instance.GetData();
        data.inventory = inventoryData.Item1;
        data.money = inventoryData.Item2;
        data.platformerKeybinds = gameSettings.platformerKeys;
        data.minigameKeybinds = gameSettings.minigameKeys;
        data.bossGameKeybinds = gameSettings.bossGameKeys;
        data.flannel = gameSettings.flannel;
        id = data.id;
        saveList.Add(data);
        string path = Path.Combine(Application.dataPath, "Saves");
        //Ensures that the saves folder actually exists
        Directory.CreateDirectory(path);
        path = Path.Combine(path, $"{data.id}.json");
        string dataString = JsonUtility.ToJson(data);
        StreamWriter sw = new(path);
        sw.Write(dataString);
        sw.Close();
        UpdateDisplay();

        return true;
    }

    public void autoSave()
    {
        if(id == "") id = gameSettings.id;
        SaveData data = saveList[0];
        foreach (var save in saveList) if (save.id.ToLower() == id.ToLower()) data = save;
        (SerializedDictionary<string, FishData>, double) inventoryData = Inventory.Instance.GetData();

        //save player data
        data.inventory = inventoryData.Item1;
        data.money = inventoryData.Item2;
        data.position = gameSettings.position;

        //copy current fish held
        foreach (var key in data.inventory.Keys)
        {
            foreach (var f in data.inventory[key].currentFish)
            {
                caughtFish fish = new caughtFish();
                fish.description = f.description;
                fish.fishName = f.fishName;
                fish.rarity = f.rarity;
                fish.length = f.length;
                fish.isBoss = f.isBoss;
                data.inventory[key].fishHeld.Add(fish);
            }

        }

        //save keybinds
        data.platformerKeybinds = gameSettings.platformerKeys;
        data.minigameKeybinds = gameSettings.minigameKeys;
        data.bossGameKeybinds = gameSettings.bossGameKeys;

        //save settings
        data.toggleData.hasPostProcessing = gameSettings.toggleData.hasPostProcessing;
        data.toggleData.isFullScreen = gameSettings.toggleData.isFullScreen;
        data.toggleData.isMouseModeMinigame = gameSettings.toggleData.isMouseModeMinigame;
        data.toggleData.isMouseModeBossgame = gameSettings.toggleData.isMouseModeBossgame;
        data.screenResolution = gameSettings.screenResolution;

        //Save Volume
        data.volumeData.master = gameSettings.masterVolume;
        data.volumeData.music = gameSettings.musicVolume;
        data.volumeData.sfx = gameSettings.sfxVolume;

        data.flannel = gameSettings.flannel;

        string path = Path.Combine(Application.dataPath, "Saves", $"{data.id}.json");
        if (File.Exists(path))
        {
            string dataString = JsonUtility.ToJson(data);
            StreamWriter sw = new(path);
            sw.Write(dataString);
            sw.Close();
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if(!layout.enabled) layout.enabled = true;
        while (options.Count > 0)
        {
            Destroy(options[0]);
            options.RemoveAt(0);
        }
        float rows = saveList.Count / columns;
        int actualRows = (rows - ((int) rows) > 0) ? (int)rows + 1 : (int)rows;
        content.sizeDelta = new Vector2(content.sizeDelta.x, actualRows * 118 + 50);
        for(int i = 0; i < saveList.Count; i++)
        {
            GameObject go = Instantiate(savePrefab, content);
            go.GetComponent<LoadData>().ApplyData(saveList[i]);
            RectTransform rect = go.GetComponent<RectTransform>();
            //rect.anchoredPosition = new Vector2(i % columns * 120 + 20, (int)(-i / columns) * 120 - 20);
            int j = i;
            foreach(var comp in go.GetComponentsInChildren<Button>())
            {
                comp.onClick.AddListener(() => Select(j));
                if(comp.name == "LoadBtn") comp.onClick.AddListener(() => LoadSelected());
                else comp.onClick.AddListener(() => Delete());
            }
            options.Add(go);
        }
    }

    public void LoadSelected()
    {
        SceneLoader.LoadScene(saveList[selected].position.currentLocation);
    }

    public void Select(int save)
    {
        selected = save;
    }

    public void Load()
    {
        var save = saveList[selected];
        string path = Path.Combine(Application.dataPath, "Saves");
        path = Path.Combine(path, $"{save.id}.json");

        //load data
        if (File.Exists(path))
        {
            //load inventory & money
            (SerializedDictionary<string, FishData>, double) inventoryData = Inventory.Instance.GetData();
            Inventory.Instance.ApplyData(save.inventory, save.money);

            //copy saved inventory
            foreach (var inv in save.inventory)
            {
                inv.Value.currentFish.Clear();
                foreach (var fish in inv.Value.fishHeld)
                {
                    Fish newFish = new Fish();
                    newFish.fishName = fish.fishName;
                    newFish.rarity = fish.rarity;
                    newFish.length = fish.length;
                    newFish.description = fish.description;
                    newFish.isBoss = fish.isBoss;
                    inv.Value.currentFish.Add(newFish);
                }
                inv.Value.fishHeld.Clear();
            }

            //load key binds
            gameSettings.platformerKeys = save.platformerKeybinds;
            gameSettings.minigameKeys = save.minigameKeybinds;
            gameSettings.bossGameKeys = save.bossGameKeybinds;

            //load settings
            gameSettings.toggleData.hasPostProcessing = save.toggleData.hasPostProcessing;
            gameSettings.toggleData.isFullScreen = save.toggleData.isFullScreen;
            gameSettings.toggleData.isMouseModeMinigame = save.toggleData.isMouseModeMinigame;
            gameSettings.toggleData.isMouseModeBossgame = save.toggleData.isMouseModeBossgame;
            gameSettings.screenResolution = save.screenResolution;

            //load player
            gameSettings.flannel = save.flannel;
            gameSettings.position = save.position;
            gameSettings.id = save.id;
            id = save.id;
        }

        //Apply loaded data & unselect
        print("Apply the loaded data");
        GameUI.Instance.LoadSaveGame();
        selected = -1;
    }

    public void Delete()
    {
        string path = Path.Combine(Application.dataPath, "Saves");
        path = Path.Combine(path, $"{saveList[selected].id}.json");

        if(File.Exists(path))
        {
            saveList.Remove(saveList[selected]);
            File.Delete(path);
            layout.enabled = false;
            StartCoroutine(UpdateLoad(options[selected].GetComponent<Animator>(), this));
            
        }
        selected = -1;
    }

    private void OnEnable()
    {
        if(layout == null) layout = content.GetComponent<GridLayoutGroup>();
        if (saveList.Count == 0)
        {
            string path = Path.Combine(Application.dataPath, "Saves");
            foreach (string file in Directory.GetFiles(path))
            {
                if (file.EndsWith(".json"))
                {
                    StreamReader sr = new(Path.Combine(path, file));
                    saveList.Add(JsonUtility.FromJson<SaveData>(sr.ReadToEnd()));
                    sr.Close();
                }
            }
        }
        UpdateDisplay();
    }

    public static IEnumerator UpdateLoad(Animator anim, SaveLoadManager self)
    {
        anim.enabled = true;
        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);
        self.UpdateDisplay();
    }
}
