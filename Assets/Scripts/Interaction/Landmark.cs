using UnityEditor;
using UnityEngine;

public class Landmark : InteractableObject
{
    public Transform dialoguePopupLocation;
    [SerializeField] protected string[] questDescriptions;
    [SerializeField] protected bool questDescriptionsLoop = false;
    public string landmarkName;
    protected int currentStandard = 0;
    protected int currentQuest = 0;
    public BasicLandmarkData baseData;

    protected GameObject currentPopup = null;
    protected override void Interact(InteractionPair pair)
    {
        if(pair.obj.Id == Id)
        {
            if(QuestManager.Instance.HasQuestFor(landmarkName) && questDescriptions.Length > 0)
            {
                DoQuestPopup();
            }
            else if(baseData.landmarkDescriptions.Length > 0)
            {
                DoStandardPopup();
            }
        }
    }

    protected void DoStandardPopup()
    {
        DoPopup(baseData.landmarkDescriptions[currentStandard]);
        currentStandard++;
        if (currentStandard == baseData.landmarkDescriptions.Length)
        {
            currentStandard = baseData.standardDescriptionsLoop ? 0 : currentStandard - 1;
        }
    }

    protected void DoQuestPopup()
    {
        DoPopup(questDescriptions[currentQuest]);
        currentQuest++;
        if (currentQuest == questDescriptions.Length)
        {
            currentQuest = questDescriptionsLoop ? 0 : currentQuest - 1;
        }
    }

    protected void DoPopup(string description)
    {
        if (currentPopup == null)
        {
            currentPopup = baseData.screenPopup ? PopupManager.Instance.CreateScreenStatementPopup(description, baseData.lifetime, landmarkName, baseData.appearanceData, baseData.closingData) : PopupManager.Instance.CreateWorldStatementPopup(dialoguePopupLocation, description, baseData.lifetime, landmarkName, baseData.appearanceData, baseData.closingData);
        }
        else
        {
            currentPopup.GetComponent<VoidPopup>().ClosePopup();
        }
    }
}
