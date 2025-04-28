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
    private int selected = -1;
    private string id = "";
    public bool Save(string name, bool newGame = true)
    {
        if(newGame) foreach (var save in saveList) if (save.id.ToLower() == name.ToLower()) return false;

        SaveData data = new(name);
        (SerializedDictionary<string, FishData>, double) inventoryData= Inventory.Instance.GetData();
        data.inventory = inventoryData.Item1;
        data.money = inventoryData.Item2;
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
        SaveData data = saveList[0];
        foreach (var save in saveList) if (save.id.ToLower() == id.ToLower()) data = save;
        (SerializedDictionary<string, FishData>, double) inventoryData = Inventory.Instance.GetData();
        //save player data
        data.inventory = inventoryData.Item1;
        data.money = inventoryData.Item2;
        data.position = gameSettings.position;

        //save keybinds
        data.platformerKeybinds = gameSettings.platformerKeys;
        data.minigameKeybinds = gameSettings.minigameKeys;
        data.bossGameKeybinds = gameSettings.bossGameKeys;

        //save settings
        data.hasPostProcessing = gameSettings.hasPostProcessing;
        data.isFullScreen = gameSettings.isFullScreen;
        data.screenResolution = gameSettings.screenResolution;

        //Save Volume
        data.volumeData.master = gameSettings.masterVolume;
        data.volumeData.music = gameSettings.musicVolume;
        data.volumeData.sfx = gameSettings.sfxVolume;

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
        while(options.Count > 0)
        {
            Destroy(options[0]);
            options.RemoveAt(0);
        }
        float rows = saveList.Count / columns;
        int actualRows = (rows - ((int) rows) > 0) ? (int)rows + 1 : (int)rows;
        content.sizeDelta = new Vector2(content.sizeDelta.x, actualRows * 100 + 20);
        for(int i = 0; i < saveList.Count; i++)
        {
            GameObject go = Instantiate(savePrefab, content);
            go.GetComponent<LoadData>().ApplyData(saveList[i]);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(i % columns * 120 + 20, (int)(-i / columns) * 120 - 20);
            int j = i;
            foreach(var comp in go.GetComponentsInChildren<Button>())
            {
                comp.onClick.AddListener(() => Select(j));
                if(comp.name == "LoadBtn") comp.onClick.AddListener(() => Load());
                else comp.onClick.AddListener(() => Delete());
            }
            options.Add(go);
        }
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

            //load key binds
            gameSettings.platformerKeys = save.platformerKeybinds;
            gameSettings.minigameKeys = save.minigameKeybinds;
            gameSettings.bossGameKeys = save.bossGameKeybinds;

            //load settings
            gameSettings.hasPostProcessing = save.hasPostProcessing;
            gameSettings.screenResolution = save.screenResolution;
            gameSettings.isFullScreen = save.isFullScreen;

            //load player
            gameSettings.position = save.position;
            id = save.id;
        }

        //Apply loaded data
        print("Apply the loaded data");
        GetComponentInParent<MenuUI>().LoadSaveGame();
    }

    public void Delete()
    {
        string path = Path.Combine(Application.dataPath, "Saves");
        path = Path.Combine(path, $"{saveList[selected].id}.json");

        if(File.Exists(path))
        {
            saveList.Remove(saveList[selected]);
            File.Delete(path);
            UpdateDisplay();
        }
    }

    private void OnEnable()
    {
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
}
