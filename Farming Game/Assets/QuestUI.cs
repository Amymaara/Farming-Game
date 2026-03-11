using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;

    // var to test specific quets
    public Quests testQuest;
    public int testQuestAmount;
    private List<QuestProgress> testQuests = new();

    void Start()
    {
        for (int i = 0; i < testQuestAmount; i++)
        {
            testQuests.Add(new QuestProgress(testQuest)); // testing specific quests
        }
        UpdateQuestUI();
    }

   public void UpdateQuestUI()
    {
        //destroy existing quest entries
        foreach (Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        //build quest entries
        foreach (var quest in QuestController.Instance.activatQuests) // chnage var quest in (test quest) to test specific quests
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestNameText").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            questNameText.text = quest.quest.name;

            foreach (var objective in quest.objectives)
            {
                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
                objText.text = $"{objective.description} ({objective.currentAmount}/{objective.requiredAmount})"; // Collect X whatver (y/z)
            }
        }
    }
}
