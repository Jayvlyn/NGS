using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject savePrefab;
    [SerializeField] private int columns = 3;
    private readonly List<GameObject> options = new();
    private readonly List<SaveData> saveList = new();
    private int selected = -1;
    public void Save()
    {
        SaveData data = new();
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
            go.GetComponent<Button>().onClick.AddListener(() => Select(j));
            options.Add(go);
        }
    }

    public void Select(int save)
    {
        if (selected != -1)
        {
            options[selected].GetComponent<Image>().color = Color.white;
        }
        selected = save;
    }

    public void Load()
    {
        Debug.Log($"Loaded Save {selected}");
    }

    private void Start()
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
        UpdateDisplay();
    }
}
