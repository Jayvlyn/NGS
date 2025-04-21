using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject savePrefab;
    [SerializeField] private GameObject gameSettings;
    [SerializeField] private int columns = 3;
    private readonly List<GameObject> options = new();
    private readonly List<SaveData> saveList = new();
    private int selected = -1;
    public bool Save(string name, bool autoSave = false)
    {

        if(!autoSave) foreach (var save in saveList) if (save.id.ToLower() == name.ToLower()) return false;

        SaveData data = (!autoSave) ? new(name) : saveList[selected];
        (SerializedDictionary<string, FishData>, double) inventoryData= Inventory.Instance.GetData();
        data.inventory = inventoryData.Item1;
        data.money = inventoryData.Item2;
        if(!autoSave)
        {
            data.platformerKeybinds = new List<string> { "space", "" };
        }
        else
        {

        }
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
        string path = Path.Combine(Application.dataPath, "Saves");
        path = Path.Combine(path, $"{saveList[selected].id}.json");
        if (File.Exists(path))
        {
            //use this saveList[selected] to fill out & load settings
            Debug.Log($"Loaded Save from {path}");
        }
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
