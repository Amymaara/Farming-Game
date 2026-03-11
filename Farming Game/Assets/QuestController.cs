using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance {  get; private set; }
    public List<QuestProgress> activateQuests = new();
    public QuestUI questUI;

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

    public void LoadQuestProgress(List<QuestProgress> savedQuests)
    {
        activateQuests = savedQuests ?? new();
        questUI.UpdateQuestUI();
    }
}
