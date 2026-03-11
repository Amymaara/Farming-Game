using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance {  get; private set; }
    public List<QuestProgress> activatQuests = new();
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
        
        activatQuests.Add(new QuestProgress(quest));

        questUI.UpdateQuestUI();
    }

    public bool IsQuestActive(string questID) => activatQuests.Exists(q => q.QuestID == questID);   
}
