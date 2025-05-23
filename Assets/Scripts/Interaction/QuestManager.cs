using GameEvents;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FishListener))]
public class QuestManager : Singleton<QuestManager>
{
    private List<Quest> activeQuests = new List<Quest>();
    [SerializeField] protected InteractionEvent interactionEvent;

	private void Start()
	{
		interactionEvent.Subscribe(UpdateQuests);
	}

	public void AddQuest(Quest quest)
    {
        activeQuests.Add(quest);
        GameUI.Instance.questUIFiller.addQuestToList(quest);
    }

    public void RemoveQuest(Quest quest)
    {
        activeQuests.Remove(quest);
        GameUI.Instance.questUIFiller.removeQuestFromList(quest);
    }

    public Quest[] GetQuests()
    {
        return activeQuests.ToArray();
    }

    public void ResetQuest(Quest quest)
    {
        quest.completeable = false;
    }

    public void CompleteQuest(Quest quest)
    {
        if(quest.onCompleteEvent != null) quest.onCompleteEvent.Raise();
        ResetQuest(quest);
        if(quest.remainingCompletions != -1)
        {
            quest.remainingCompletions--;
            if(quest.remainingCompletions == 0)
            {
                quest.disabled = true;
            }
        }
        Inventory.Instance.AddMoney(quest.reward.carrots);
        RemoveQuest(quest);
    }

    public void UpdateQuests(InteractionPair pair)
    {
        if (pair.obj is Landmark landmark)
        {
            foreach (Quest quest in activeQuests)
            {
                if (!quest.completeable && !quest.fishQuest)
                {
                    quest.completeable = quest.destinationName == landmark.landmarkName;
                }
            }
        }
    }
    public void UpdateQuests(Fish fish = null)
    {
        foreach (Quest quest in activeQuests)
        {
            if(quest.fishQuest && Inventory.Instance.GetLowestViable(quest.fish.fishName, quest.minLength))
            {
                quest.completeable = true;
            }
        }
    }

    public bool HasQuestFor(string destinationName)
    {
        foreach(Quest quest in activeQuests)
        {
            if(!quest.fishQuest && quest.destinationName == destinationName)
            {
                return true;
            }
        }
        return false;
    }
}
