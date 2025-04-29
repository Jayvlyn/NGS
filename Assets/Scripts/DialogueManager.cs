using TMPro;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private GameObject dialoguePrefab;
    public GameObject CreateDialogue(Transform location, string dialogue = "", float lifetime = 10, string speakerName = "")
    {
        GameObject go = Instantiate(dialoguePrefab, location);
        DialogueVoidPopup popup = go.GetComponentInChildren<DialogueVoidPopup>();
        popup.Lifetime = lifetime;
        popup.dialogueText.text = dialogue;
        popup.nameText.text = speakerName;
        go.GetComponent<Canvas>().worldCamera = Camera.current;
        return go;
    }
    public GameObject CreateDialogue(Transform location, float lifetime, string speakerName = "")
    {
        return CreateDialogue(location, "", lifetime, speakerName);
    }
    public GameObject CreateDialogue(Transform location, string dialogue, string speakerName)
    {
        return CreateDialogue(location, dialogue, 10, speakerName);
    }
    public GameObject CreateDialogue(string speakerName, Transform location)
    {
        return CreateDialogue(location, "", speakerName);
    }

    public void Start()
    {
        //Debug only
        //CreateDialogue(transform, "This is a test dialogue created to verify that everything is working properly", "Tester");
    }
}
