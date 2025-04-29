using TMPro;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private GameObject worldDialoguePrefab;
    [SerializeField] private GameObject screenDialoguePrefab;
    public GameObject CreateDialogue(Transform location, string dialogue = "", float lifetime = 10, string speakerName = "")
    {
        GameObject go = Instantiate(worldDialoguePrefab, location);
        DialogueVoidPopup popup = go.GetComponentInChildren<DialogueVoidPopup>();
        popup.Lifetime = lifetime;
        if (dialogue != string.Empty)
        {
            popup.dialogueText.text = dialogue;
        }
        else
        {
            Destroy(popup.dialogueText.transform.parent.gameObject);
        }
        if (speakerName != string.Empty)
        {
            popup.nameText.text = speakerName;
        }
        else
        {
            Destroy(popup.nameText.transform.parent.gameObject);
        }
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
    public GameObject CreateDialogue(string dialogue = "", float lifetime = 10, string speakerName = "")
    {
        GameObject go = Instantiate(worldDialoguePrefab);
        DialogueVoidPopup popup = go.GetComponentInChildren<DialogueVoidPopup>();
        popup.Lifetime = lifetime;
        if (dialogue != string.Empty)
        {
            popup.dialogueText.text = dialogue;
        }
        else
        {
            Destroy(popup.dialogueText.transform.parent.gameObject);
        }
        if (speakerName != string.Empty)
        {
            popup.nameText.text = speakerName;
        }
        else
        {
            Destroy(popup.nameText.transform.parent.gameObject);
        }
        go.GetComponent<Canvas>().worldCamera = Camera.current;
        return go;
    }
    public GameObject CreateDialogue(float lifetime, string speakerName = "")
    {
        return CreateDialogue("", lifetime, speakerName);
    }
    public GameObject CreateDialogue(string dialogue, string speakerName)
    {
        return CreateDialogue(dialogue, 10, speakerName);
    }
    public GameObject CreateDialogue(string speakerName)
    {
        return CreateDialogue("", speakerName);
    }

    public void Start()
    {
        //Debug only
        //CreateDialogue(transform, "This is a test dialogue created to verify that everything is working properly", "Tester");
    }
}
