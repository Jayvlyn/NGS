using GameEvents;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FishListener))]
public class QuestManager : Singleton<QuestManager>
{
    private List<Quest> activeQuests = new List<Quest>();
    [SerializeField] protected InteractionEvent interactionEvent;
    public void AddQuest(Quest quest)
    {
        activeQuests.Add(quest);
    }

    public void RemoveQuest(Quest quest)
    {
        activeQuests.Remove(quest);
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
        ResetQuest(quest);
        quest.disabled = quest.disableUponComplete;
        Inventory.Instance.AddMoney(quest.reward);
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
