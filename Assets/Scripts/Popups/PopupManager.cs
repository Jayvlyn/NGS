using GameEvents;
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
    [SerializeField] private GameObject fishConfirmationPopupPrefab;
    [SerializeField] private GameObject fishCaughtPopup;
    [SerializeField] private Color genericFishPopupColor = Color.white;
    [SerializeField] private Color newLargestFishPopupColor = Color.white;
    [SerializeField] private Color newTypeFishPopupColor = Color.white;
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
        AddAppearance(popup.gameObject, appearance, true);
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
        AddAppearance(popup.gameObject, appearance);
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
        go.GetComponentInChildren<InteractVoidPopup>().text.text =
            InputControlPath.ToHumanReadableString(interactAction.action.bindings[0].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        return go;
    }
    #endregion World Interaction Popups

    public GameObject CreateFishCaughtPopup(Sprite sprite, Color fxColor, string topText = "", string bottomText = "", PopupAppearanceData appearance = new PopupAppearanceData(), PopupAppearanceData disappearance = new PopupAppearanceData())
    {
        GameObject go = Instantiate(fishCaughtPopup);
        FishPopup popup = go.GetComponentInChildren<FishPopup>();
        popup.topText.text = topText;
        popup.bottomText.text = bottomText;
        popup.image.sprite = sprite;
        popup.closeBehavior = disappearance;
        popup.effectImg.color = fxColor;
        AddAppearance(popup.gameObject, appearance);
        return go;
    }

    public GameObject CreateNewFishTypePopup(Fish fish)
    {
        PopupAppearanceData popupData = GetDefaultFishPopupData();
        return CreateFishCaughtPopup(fish.sprite, newTypeFishPopupColor,"New fish type caught!", fish.fishName, popupData, popupData);
    }

    public GameObject CreateNewLargestFishPopup(Fish fish)
    {
        PopupAppearanceData popupData = GetDefaultFishPopupData();
        return CreateFishCaughtPopup(fish.sprite, newLargestFishPopupColor, $"New largest {fish.fishName}!", $"{fish.length:F2} cm", popupData, popupData);
    }

    public GameObject CreateGenericFishPopup(Fish fish)
    {
        PopupAppearanceData popupData = GetDefaultFishPopupData();
        return CreateFishCaughtPopup(fish.sprite, genericFishPopupColor, $"Caught a {fish.fishName}.", $"{fish.length:F2} cm", popupData, popupData);
    }

    private PopupAppearanceData GetDefaultFishPopupData()
    {
        return new PopupAppearanceData()
        {
            AppearanceType = AppearanceType.ZoomIn,
            Time = 0.25f
        };
    }

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
    private void AddAppearance(GameObject go, PopupAppearanceData appearance, bool world = false)
    {
        switch (appearance.AppearanceType)
        {
            case AppearanceType.ZoomIn:
                go.AddComponent<Zoomer>().time = appearance.Time;
                break;
            case AppearanceType.FadeIn:
                go.AddComponent<Fader>().time = appearance.Time;
                break;
            case AppearanceType.FromBottom:
            case AppearanceType.FromTop:
                appearance.Offset *= world ? 1 : Screen.height;
                Flyer component = go.AddComponent<Flyer>();
                component.fromDirection = (Direction)(int)(appearance.AppearanceType - 3);
                component.offset = appearance.Offset;
                component.time = appearance.Time;
                break;
            case AppearanceType.FromRight:
            case AppearanceType.FromLeft:
                appearance.Offset *= world ? 1 : Screen.width;
                component = go.AddComponent<Flyer>();
                component.fromDirection = (Direction)(int)(appearance.AppearanceType - 3);
                component.offset = appearance.Offset;
                component.time = appearance.Time;
                break;
        }
    }
}
