using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopupManager : Singleton<PopupManager>
{
    [SerializeField] private GameObject worldSpeechPrefab;
    [SerializeField] private GameObject screenSpeechPrefab;
    [SerializeField] private GameObject worldInteractPrefab;
    [SerializeField] private InputActionReference interactAction;
    #region World Statement
    public GameObject CreateWorldStatementPopup(Transform location, string statement = "", float lifetime = 10, string stater = "")
    {
        GameObject go = Instantiate(worldSpeechPrefab, location);
        DialogueVoidPopup popup = go.GetComponentInChildren<DialogueVoidPopup>();
        popup.Lifetime = lifetime;
        if (statement != string.Empty)
        {
            popup.dialogueText.text = statement;
        }
        else
        {
            Destroy(popup.dialogueText.transform.parent.gameObject);
        }
        if (stater != string.Empty)
        {
            popup.nameText.text = stater;
        }
        else
        {
            Destroy(popup.nameText.transform.parent.gameObject);
        }
        go.GetComponent<Canvas>().worldCamera = Camera.current;
        return go;
    }
    public GameObject CreateWorldStatementPopup(Transform location, float lifetime, string stater = "")
    {
        return CreateWorldStatementPopup(location, "", lifetime, stater);
    }
    public GameObject CreateWorldStatementPopup(Transform location, string statement, string stater)
    {
        return CreateWorldStatementPopup(location, statement, 10, stater);
    }
    public GameObject CreateWorldStatementPopup(string stater, Transform location)
    {
        return CreateWorldStatementPopup(location, "", stater);
    }
    #endregion World Statement
    #region Screen Statement
    public GameObject CreateScreenStatementPopup(string statement, float lifetime, string stater = "")
    {
        GameObject go = Instantiate(worldSpeechPrefab);
        DialogueVoidPopup popup = go.GetComponentInChildren<DialogueVoidPopup>();
        popup.Lifetime = lifetime;
        if (statement != string.Empty)
        {
            popup.dialogueText.text = statement;
        }
        else
        {
            Destroy(popup.dialogueText.transform.parent.gameObject);
        }
        if (stater != string.Empty)
        {
            popup.nameText.text = stater;
        }
        else
        {
            Destroy(popup.nameText.transform.parent.gameObject);
        }
        go.GetComponent<Canvas>().worldCamera = Camera.current;
        return go;
    }
    public GameObject CreateScreenStatementPopup(float lifetime, string stater = "")
    {
        return CreateScreenStatementPopup("", lifetime, stater);
    }
    public GameObject CreateScreenStatementPopup(string statement, string speakerName)
    {
        return CreateScreenStatementPopup(statement, 10, speakerName);
    }
    public GameObject CreateScreenStatementPopup(string text, bool isSpeaker)
    {
        return CreateScreenStatementPopup(isSpeaker ? string.Empty : text, isSpeaker ? text : string.Empty);
    }
    #endregion Screen Statement
    #region World Interaction Popups
    public GameObject CreateWorldInteractionPopup(Transform location)
    {
        GameObject go = Instantiate(worldInteractPrefab, location);
        go.GetComponent<InteractVoidPopup>().text.text =
            InputControlPath.ToHumanReadableString(interactAction.action.bindings[0].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        return go;
    }
    #endregion World Interaction Popups
}
