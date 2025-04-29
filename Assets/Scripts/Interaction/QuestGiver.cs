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
    //True if you have not initiated an interaction with this object yet
    protected bool canInteract = true;

    [SerializeField] protected BoolListener listener;
    [SerializeField] protected GameObject confirmationPopupPrefab;
    [SerializeField] protected int confirmationPopupFishNameIndex = 1;
    [SerializeField] protected int confirmationPopupFishLengthIndex = 2;
    [SerializeField] protected Transform confirmationPopupTransform;

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
                Fish lowestViable = null;
                foreach(Fish fish in Inventory.Instance.GetFishData(potentialQuests[currentQuestIndex].FishName).currentFish)
                {
                    if(fish.length >= potentialQuests[currentQuestIndex].MinLength && (lowestViable == null || lowestViable.length > fish.length))
                    {
                        lowestViable = fish;
                    }
                }
                if(lowestViable != null)
                {
                    currentPopup = Instantiate(confirmationPopupPrefab, confirmationPopupTransform);
                    currentPopup.GetComponentInChildren<BoolPopup>().Event.RegisterListener(listener);
                    currentPopup.GetComponentInChildren<Image>().sprite = lowestViable.sprite;
                    TMP_Text[] texts = currentPopup.GetComponentsInChildren<TMP_Text>();
                    texts[confirmationPopupFishNameIndex].text = lowestViable.fishName;
                    texts[confirmationPopupFishLengthIndex].text = lowestViable.length.ToString();
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
                if (firstTimeDescribing)
                {
                    //TODO: Attach Dialogue system to display the quest description
                }
                else
                {
                    //TODO: Attache Dialogue System to display the repeat description for the quest
                    //(should default to the normal description if the string is empty)
                }
            }
        }
    }

    public void CloseConfirmationPopup(bool complete)
    {
        if(!canInteract)
        {
            if(complete)
            {
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
