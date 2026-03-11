using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance {  get; private set; }
    public List<QuestProgress> activateQuests = new();
    public QuestUI questUI;

    public List<string> handinQuestIDs = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        questUI = FindObjectOfType<QuestUI>();
    }

    public void AcceptQuest(Quests quest)
    {
        if (IsQuestActive(quest.questID)) return;
        
        activateQuests.Add(new QuestProgress(quest));

        questUI.UpdateQuestUI();
    }

    public bool IsQuestActive(string questID) => activateQuests.Exists(q => q.QuestID == questID);   

    public void RegisterCollectedItem(int itemID, int amount = 1)
    {
        foreach (QuestProgress quest in activateQuests)
        {
            foreach (QuestObjective questObjective in quest.objectives)
            {
                if (questObjective.type != ObjectiveType.Fetch) continue;
                if (!int.TryParse(questObjective.objectiveID, out int objectiveItemID)) continue;
                if (objectiveItemID != itemID) continue;

                questObjective.currentAmount = Mathf.Min(
                    questObjective.currentAmount + amount,
                    questObjective.requiredAmount
                );
            }
        }

        questUI.UpdateQuestUI();
    }

    public bool IsQuestCompleted(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        return quest != null && quest.objectives.TrueForAll(o => o.IsCompleted);
    }

    public void HandInQuest(string questID)
    {
        Debug.Log("Trying to hand in quest: " + questID);

        // remove required items 
        if (!RemoveRequiredItemsFromInventory(questID))
        {
            Debug.Log("Hand in failed: missing required items");
            // quest couldnt complete / missing items
            return;
        }

        // remove quest from quest log
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        if (quest != null)
        {
            handinQuestIDs.Add(questID);
            activateQuests.Remove(quest);

            Debug.Log("Quest removed successfully. Active quests left: " + activateQuests.Count);

            questUI.UpdateQuestUI();
        }

        else
        {
            Debug.Log("Quest not found in active quests");
        }
    }

    public bool IsQuestHandedIn(string questID)
    {
        return handinQuestIDs.Contains(questID);
    }

    public bool RemoveRequiredItemsFromInventory(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.QuestID == questID);
        if (quest == null) return false;

        Dictionary<int, int> requiredItems = new();

        //item requirements from objs
        foreach (QuestObjective objective in quest.objectives)
        {
            if (objective.type == ObjectiveType.Fetch && int.TryParse(objective.objectiveID, out int itemID))
            {
                requiredItems[itemID] = objective.requiredAmount;
            }
        }

        //verify we have items

        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();
        foreach (var item in requiredItems)
        {
            if (itemCounts.GetValueOrDefault(item.Key) < item.Value)
            {
                // not enough itemns to compelte
                return false;
            }
        }

        //remove required items from inv
        foreach (var itemRequirement in requiredItems)
        {
            //remove items from inventory
            InventoryController.Instance.RemoveItemsFromInventory(itemRequirement.Key, itemRequirement.Value);
        }
        InventoryController.Instance.RebuildItemCounts();
        return true;
    }
    public void LoadQuestProgress(List<QuestProgress> savedQuests)
    {
        activateQuests = savedQuests ?? new();
        questUI.UpdateQuestUI();
    }
}
