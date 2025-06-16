using GameEvents;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FishListener))]
public class QuestManager : Singleton<QuestManager>
{
    public List<Quest> allKnownQuests = new();
    [SerializeField] protected InteractionEvent interactionEvent;
    private WardrobeManager wardrobe;

    private UIParticleFX particleSys;

    private void Start()
	{
		interactionEvent.Subscribe(UpdateQuests);
    }

	public void AddQuest(Quest quest)
    {
        if (quest.remainingCompletions < -1) return;
        quest.active = true;
        if(GetQuest(quest.questName) == null)
        {
            allKnownQuests.Add(quest);
        }
        GameUI.Instance.questUIFiller.addQuestToList(quest);
        UpdateQuests();
    }

    public void RemoveQuest(Quest quest)
    {
        GameUI.Instance.questUIFiller.removeQuestFromList(quest);
    }

    public Quest[] GetQuests()
    {
        return allKnownQuests.ToArray();
    }

    public void ResetQuest(Quest quest)
    {
        quest.completeable = false;
    }

    public void CompleteQuest(Quest quest)
    {
        if(quest.onCompleteEvent != null) quest.onCompleteEvent.Raise();
        ResetQuest(quest);
        if (quest.remainingCompletions != -1)
        {
            quest.remainingCompletions--;
            if (quest.remainingCompletions == 0)
            {
                quest.disabled = true;
            }
        }
        quest.active = false;
        int reward = (int)quest.reward.carrots;
        if (reward > 0) SpawnRewardCarrots(reward);
        if(!string.IsNullOrEmpty(quest.reward.flannelName))
        {
            if(wardrobe == null)
            {
                wardrobe = FindFirstObjectByType<WardrobeManager>();
            }
            wardrobe.UnlockFlannel(quest.reward.flannelName);
        }
        Inventory.Instance.AddMoney(reward);
        RemoveQuest(quest);
    }

    public void UpdateQuests(InteractionPair pair)
    {
        foreach (Quest quest in allKnownQuests)
        {
            if (quest.active && !quest.completeable && !quest.fishQuest)
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
        foreach (Quest quest in allKnownQuests)
        {
            if(quest.active && quest.fishQuest && Inventory.Instance.GetLowestViable(quest.fish.fishName, quest.minLength))
            {
                quest.completeable = true;
            }
        }
    }

    public bool HasQuestFor(string destinationName)
    {
        foreach(Quest quest in allKnownQuests)
        {
            if(quest.active && !quest.fishQuest && quest.destinationName == destinationName)
            {
                return true;
            }
        }
        return false;
    }

    public void SpawnRewardCarrots(int cost)
    {
        Vector3 screenPos = Input.mousePosition;
        if(particleSys == null)
        {
            particleSys = GameUI.Instance.GetComponent<UIParticleFX>();
        }
        particleSys.SpawnParticles(cost, screenPos, true);
    }

    public List<QuestSaveData> ExtractSaveData()
    {
        List<QuestSaveData> saves = new();
        foreach (var quest in allKnownQuests)
        {
            saves.Add(new QuestSaveData
            {
                questName = quest.questName,
                completeable = quest.completeable,
                disabled = quest.disabled,
                remainingCompletions = quest.remainingCompletions,
                active = quest.active
            });
        }
        return saves;
    }

    public void ApplyQuestSaveData(List<QuestSaveData> savedQuests)
    {
        foreach (var SQ in savedQuests)
        {
            Quest quest = allKnownQuests.Find(q => q.questName == SQ.questName);
            if (quest != null)
            {
                quest.completeable = SQ.completeable;
                quest.disabled = SQ.disabled;
                quest.remainingCompletions = SQ.remainingCompletions;
                quest.active = SQ.active;

                if (quest.active) AddQuest(quest);
            }
        }
    }
    public Quest GetQuest(string questName)
    {
        foreach(Quest quest in allKnownQuests)
        {
            if(quest.questName == questName) return quest;
        }
        return null;
    }

    public void ResetQuests()
    {
        allKnownQuests.Clear();
    }
}
