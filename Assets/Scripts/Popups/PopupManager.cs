using GameEvents;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PopupManager : Singleton<PopupManager>
{
    [SerializeField] private GameObject worldSpeechPrefab;
    [SerializeField] private GameObject screenSpeechPrefab;
    [SerializeField] private GameObject worldInteractPrefab;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private GameObject fishConfirmationPopupPrefab;
    #region World Statement
    public GameObject CreateWorldStatementPopup(Transform location, string statement = "", float lifetime = 10, string stater = "", PopupAppearanceData appearance = new PopupAppearanceData(), PopupAppearanceData disappearence = new PopupAppearanceData())
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
        switch (appearance.AppearanceType)
        {
            case AppearanceType.ZoomIn:
                go.AddComponent<Zoomer>().time = appearance.Time;
                break;
            case AppearanceType.FadeIn:
                go.AddComponent<Fader>().time = appearance.Time;
                break;
        }
        popup.closeBehavior = disappearence;
        return go;
    }
    public GameObject CreateWorldStatementPopup(Transform location, float lifetime, string stater = "", PopupAppearanceData appearance = new PopupAppearanceData(), PopupAppearanceData disappearence = new PopupAppearanceData())
    {
        return CreateWorldStatementPopup(location, "", lifetime, stater, appearance, disappearence);
    }
    public GameObject CreateWorldStatementPopup(Transform location, string statement, string stater, PopupAppearanceData appearance = new PopupAppearanceData(), PopupAppearanceData disappearence = new PopupAppearanceData())
    {
        return CreateWorldStatementPopup(location, statement, 10, stater, appearance, disappearence);
    }
    public GameObject CreateWorldStatementPopup(string stater, Transform location, PopupAppearanceData appearance = new PopupAppearanceData(), PopupAppearanceData disappearence = new PopupAppearanceData())
    {
        return CreateWorldStatementPopup(location, "", stater, appearance, disappearence);
    }
    #endregion World Statement
    #region Screen Statement
    public GameObject CreateScreenStatementPopup(string statement, float lifetime, string stater = "", PopupAppearanceData appearance = new PopupAppearanceData(), PopupAppearanceData disappearence = new PopupAppearanceData())
    {
        GameObject go = Instantiate(screenSpeechPrefab);
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
        switch(appearance.AppearanceType)
        {
            case AppearanceType.ZoomIn:
                go.AddComponent<Zoomer>().time = appearance.Time;
                break;
            case AppearanceType.FadeIn:
                go.AddComponent<Fader>().time = appearance.Time;
                break;
            case AppearanceType.FromBottom:
            case AppearanceType.FromTop:
                appearance.Offset *= Screen.height; 
                Flyer component = go.AddComponent<Flyer>();
                component.fromDirection = (Direction)(int)(appearance.AppearanceType - 3);
                component.offset = appearance.Offset;
                component.time = appearance.Time;
                break;
            case AppearanceType.FromRight:
            case AppearanceType.FromLeft:
                appearance.Offset *= Screen.width;
                component = go.AddComponent<Flyer>();
                component.fromDirection = (Direction)(int)(appearance.AppearanceType - 3);
                component.offset = appearance.Offset;
                component.time = appearance.Time;
                break;
        }
        popup.closeBehavior = disappearence;
        return go;
    }
    public GameObject CreateScreenStatementPopup(float lifetime, string stater = "", PopupAppearanceData appearance = new PopupAppearanceData(), PopupAppearanceData disappearence = new PopupAppearanceData())
    {
        return CreateScreenStatementPopup("", lifetime, stater, appearance, disappearence);
    }
    public GameObject CreateScreenStatementPopup(string statement, string speakerName, PopupAppearanceData appearance = new PopupAppearanceData(), PopupAppearanceData disappearence = new PopupAppearanceData())
    {
        return CreateScreenStatementPopup(statement, 10, speakerName, appearance, disappearence);
    }
    public GameObject CreateScreenStatementPopup(string text, bool isSpeaker, PopupAppearanceData appearance = new PopupAppearanceData(), PopupAppearanceData disappearence = new PopupAppearanceData())
    {
        return CreateScreenStatementPopup(isSpeaker ? string.Empty : text, isSpeaker ? text : string.Empty, appearance, disappearence);
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

    public GameObject CreateStatementPopup(PopupData popupData)
    {
        return popupData.IsWorldPopup ? 
            CreateWorldStatementPopup(popupData.WorldLocation, popupData.Statement, 
            popupData.Lifetime, popupData.Name, popupData.Appearance, popupData.Disappearance) : 
            CreateScreenStatementPopup(popupData.Statement, popupData.Lifetime, popupData.Name, 
            popupData.Appearance, popupData.Disappearance);
    }

    public GameObject CreateFishConfirmationPopup(IGameEventListener<bool> listener, Sprite sprite, string fishName, float fishLength, string receiverName, bool selling = false, PopupAppearanceData appearence = new PopupAppearanceData(), PopupAppearanceData disappearence = new PopupAppearanceData())
    {
        GameObject go = Instantiate(fishConfirmationPopupPrefab);
        ConfirmationBoolPopup popup = go.GetComponentInChildren<ConfirmationBoolPopup>();
        popup.Event.RegisterListener(listener);
        popup.FishImage.sprite = sprite;
        popup.FishNameText.text = fishName;
        popup.FishLengthText.text = fishLength.ToString() + " cm";
        popup.QuestionText.text = $"{(selling ? "Sell" : "Give")} this fish to {receiverName}?";
        return go;
    }

}
