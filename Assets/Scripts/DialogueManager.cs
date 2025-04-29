using TMPro;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private GameObject dialoguePrefab;
    [SerializeField] private int nameIndex;
    [SerializeField] private int dialogueIndex;
    public GameObject CreateDialogue(Transform location, string dialogue = "", float lifetime = 30, string speakerName = "")
    {
        GameObject go = Instantiate(dialoguePrefab, location);
        go.GetComponentInChildren<VoidPopup>().Lifetime = lifetime;
        TMP_Text[] texts = go.GetComponentsInChildren<TMP_Text>();
        texts[dialogueIndex].text = dialogue;
        texts[nameIndex].text = speakerName;
        go.GetComponent<Canvas>().worldCamera = Camera.current;
        return go;
    }
    public GameObject CreateDialogue(Transform location, float lifetime, string speakerName = "")
    {
        return CreateDialogue(location, "", lifetime, speakerName);
    }
    public GameObject CreateDialogue(Transform location, string dialogue, string speakerName)
    {
        return CreateDialogue(location, dialogue, 30, speakerName);
    }
    public GameObject CreateDialogue(string speakerName, Transform location)
    {
        return CreateDialogue(location, "", speakerName);
    }

    public void Start()
    {
        //Debug only
        CreateDialogue(transform, "This is a test dialogue created to verify that everything is working properly", "Tester");
    }
}
