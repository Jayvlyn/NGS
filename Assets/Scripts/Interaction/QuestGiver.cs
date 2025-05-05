using GameEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent (typeof(BoolListener))]
public class QuestGiver : InteractableObject
{
    protected bool completedAQuest = false;
    [SerializeField] protected int currentQuestIndex = -1;
    [SerializeField] protected QuestData[] potentialQuests;
    [SerializeField] protected bool givesCosmetic = true;
    [SerializeField] protected string questGiverName = "";
    //True if you have not initiated an interaction with this object yet
    protected bool canInteract = true;

    [SerializeField] protected BoolListener listener;
    [SerializeField] protected GameObject confirmationPopupPrefab;
    [SerializeField] protected Transform dialoguePopupTransform;

    private Fish lowestViable = null;

    protected GameObject currentPopup = null;
    protected override void Start()
    {
        base.Start();
        if (listener == null)
        {
            listener = GetComponent<BoolListener>();
        }
    }

    protected override void Interact(InteractionPair pair)
    {
        if(pair.obj.Id == Id && potentialQuests.Length > 0 && canInteract)
        {
            bool firstTimeDescribing = false;
            if(currentQuestIndex != -1)
            {
                lowestViable = null;
                if(Inventory.Instance.GetFishData(potentialQuests[currentQuestIndex].Fish.fishName).currentFish != null)
                {
                    foreach(Fish fish in Inventory.Instance.GetFishData(potentialQuests[currentQuestIndex].Fish.fishName).currentFish)
                    {
                        if(fish.length >= potentialQuests[currentQuestIndex].MinLength && (lowestViable == null || lowestViable.length > fish.length))
                        {
                            lowestViable = fish;
                        }
                    }
                }
                if(lowestViable != null)
                {
                    currentPopup = Instantiate(confirmationPopupPrefab);
                    ConfirmationBoolPopup popup = currentPopup.GetComponentInChildren<ConfirmationBoolPopup>();
                    popup.Event.RegisterListener(listener);
                    popup.FishImage.sprite = lowestViable.sprite;
                    popup.FishNameText.text = lowestViable.fishName;
                    popup.FishLengthText.text = lowestViable.length.ToString();
                    popup.QuestionText.text = $"Give this fish to {questGiverName}?";
                    canInteract = false;
                }
            }
            else
            {
                currentQuestIndex = Random.Range(0, potentialQuests.Length);
                firstTimeDescribing = true;
            }
            if(canInteract)
            {
                PopupManager.Instance.CreateWorldStatementPopup(dialoguePopupTransform,
                    firstTimeDescribing || potentialQuests[currentQuestIndex].RepeatDescription == string.Empty ?
                    potentialQuests[currentQuestIndex].QuestDescription :
                    potentialQuests[currentQuestIndex].RepeatDescription, questGiverName);
            }
        }
    }

    public void CloseConfirmationPopup(bool complete)
    {
        if(!canInteract)
        {
            if(complete)
            {
                Inventory.Instance.RemoveFish(lowestViable);
                lowestViable = null;
                if(completedAQuest)
                {
                    Inventory.Instance.AddMoney(potentialQuests[currentQuestIndex].Reward);
                    currentQuestIndex = -1;

                }
                else
                {
                    if (givesCosmetic)
                    {
                        completedAQuest = true;
                        //TODO: Attach system to add cosmetics
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
            Destroy(currentPopup);
            canInteract = true;
        }
    }

}
