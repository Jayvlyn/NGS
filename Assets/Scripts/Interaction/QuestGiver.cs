using GameEvents;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
[RequireComponent (typeof(BoolListener))]
public class QuestGiver : InteractableObject
{
    [SerializeField] protected bool completedAQuest = false;
    [SerializeField] protected int currentQuestIndex = -1;
    [SerializeField] protected bool questLine = false;
    [SerializeField] protected bool loopAfterComplete;
    protected int currentOrder = 0;
    [SerializeField] protected List<QuestData> potentialQuests;
    [SerializeField] protected bool givesCosmetic = true;
    public string questGiverName = "";
    //True if you have not initiated an interaction with this object yet
    protected bool canInteract = true;
    protected bool nextCanInteract = false;

    [SerializeField] protected BoolListener listener;
    [SerializeField] protected Transform dialoguePopupTransform;
    [SerializeField] protected PopupAppearanceData appearanceData;
    [SerializeField] protected BasicLandmarkData dataForNoMoreQuests;
    private int dialogueIndex;

    private Fish lowestViable = null;

    protected GameObject currentPopup = null;

    private Collider2D colliderStorage;
    protected override void Start()
    {
        base.Start();
        if (listener == null)
        {
            listener = GetComponent<BoolListener>();
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        colliderStorage = collision;
    }

    protected override void Interact(InteractionPair pair)
    {
        if(pair.obj.Id == Id && potentialQuests.Count > 0 && canInteract)
        {
            nextCanInteract = canInteract;
            if(currentQuestIndex != -1)
            {
                if (potentialQuests[currentQuestIndex].quest.completeable)
                {
                    if (potentialQuests[currentQuestIndex].quest.fishQuest)
                    {
                        lowestViable = Inventory.Instance.GetLowestViable(potentialQuests[currentQuestIndex].quest.fish.fishName, potentialQuests[currentQuestIndex].quest.minLength);
                        currentPopup = PopupManager.Instance.CreateFishConfirmationPopup(listener, lowestViable.sprite, lowestViable.fishName, lowestViable.length, questGiverName);
                        nextCanInteract = false;
                    }
                    else
                    {
                        CompleteQuest();
                        nextCanInteract = true;
                    }
                    canInteract = false;
                }
            }
            else
            {
                if (questLine)
                {
                    currentQuestIndex = currentOrder;
                    currentOrder++;
                }
                else
                {
                    currentQuestIndex = Random.Range(0, potentialQuests.Count);
                }
                potentialQuests[currentQuestIndex].quest.completeable = false;
                QuestManager.Instance.AddQuest(potentialQuests[currentQuestIndex].quest);
            }
            if(canInteract)
            {
                if (currentPopup != null && currentPopup.TryGetComponent(out VoidPopup popup))
                {
                    popup.ClosePopup();
                }
                currentPopup = PopupManager.Instance.CreateWorldStatementPopup(dialoguePopupTransform,
                    potentialQuests[currentQuestIndex].dialogues[dialogueIndex],
                    questGiverName, appearanceData);
                dialogueIndex++;
                if (dialogueIndex == potentialQuests[currentQuestIndex].dialogues.Length)
                {
                    dialogueIndex = potentialQuests[currentQuestIndex].loop ? 0 : dialogueIndex - 1;
                }
            }
            canInteract = nextCanInteract;
        }
    }

    public void CloseConfirmationPopup(bool complete)
    {
        if(!canInteract)
        {
            if (complete)
            {
                CompleteQuest();
            }
            else
            {
                Destroy(currentPopup);
            }
            canInteract = true;
        }
    }

    public void CompleteQuest()
    {
        if (potentialQuests[currentQuestIndex].quest.fishQuest)
        {
            Inventory.Instance.RemoveFish(lowestViable);
            lowestViable = null;
        }
        QuestManager.Instance.CompleteQuest(potentialQuests[currentQuestIndex].quest);
        if(!completedAQuest)
        {
            if (givesCosmetic)
            {
                completedAQuest = true;
                //TODO: Attach system to add cosmetics
            }
        }
        if(currentPopup != null && currentPopup.TryGetComponent(out DialogueVoidPopup pop))
        {
            pop.ClosePopup();
        }
        currentPopup = PopupManager.Instance.CreateWorldStatementPopup(dialoguePopupTransform, potentialQuests[currentQuestIndex].completionDialogue, questGiverName, appearanceData);
        if (potentialQuests[currentQuestIndex].quest.disabled)
        {
            potentialQuests.RemoveAt(currentQuestIndex);
            
        }
        currentQuestIndex = -1;
        dialogueIndex = 0;
        if (potentialQuests.Count == 0)
        {
            Landmark landmark = gameObject.AddComponent<Landmark>();
            landmark.baseData = dataForNoMoreQuests;
            landmark.landmarkName = questGiverName;
            landmark.dialoguePopupLocation = dialoguePopupTransform;
            landmark.popupLocation = popupLocation;
            landmark.interactEvent = interactEvent;
            landmark.enterInteractionRangeEvent = enterInteractionRangeEvent;
            landmark.exitInteractionRangeEvent = exitInteractionRangeEvent;
            landmark.interactionType = InteractionType.Landmark;
            landmark.currentPopup = currentPopup;
            landmark.OnTriggerEnter2D(colliderStorage);
            landmark.CreateId(Id);
            //OnTriggerExit2D(colliderStorage);
            Destroy(this);
        }
        else if (questLine)
        {
            currentOrder--;
            if (currentOrder == potentialQuests.Count)
            {
                currentOrder = loopAfterComplete ? 0 : currentOrder - 1;
            }
        }
    }

}
