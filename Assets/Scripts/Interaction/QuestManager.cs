using GameEvents;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FishListener))]
public class QuestManager : Singleton<QuestManager>
{
    private List<Quest> activeQuests = new List<Quest>();
    [SerializeField] protected InteractionEvent interactionEvent;
    [HideInInspector] public List<string> flannelsToUnlock = new();

    private UIParticleFX particleSys;

    private void Start()
	{
		interactionEvent.Subscribe(UpdateQuests);
        particleSys = GameUI.Instance.GetComponent<UIParticleFX>();
    }

	public void AddQuest(Quest quest)
    {
        activeQuests.Add(quest);
        GameUI.Instance.questUIFiller.addQuestToList(quest);
        UpdateQuests();
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
        int reward = (int)quest.reward.carrots;
        if (reward > 0) SpawnRewardCarrots(reward);
        if(!string.IsNullOrEmpty(quest.reward.flannelName))
        {
            flannelsToUnlock.Add(quest.reward.flannelName);
        }
        Inventory.Instance.AddMoney(reward);
        RemoveQuest(quest);
    }

    public void UpdateQuests(InteractionPair pair)
    {
        foreach (Quest quest in activeQuests)
        {
            if (!quest.completeable && !quest.fishQuest)
            {

                if (pair.obj is Landmark landmark)
                {
                    quest.completeable = quest.destinationName == landmark.landmarkName;
                }
                else if (pair.obj is QuestGiver giver)
                {
                    quest.completeable = quest.destinationName == giver.questGiverName;
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

    public void SpawnRewardCarrots(int cost)
    {
        Vector3 screenPos = Input.mousePosition;
        particleSys.SpawnParticles(cost, screenPos, true);
    }
}
