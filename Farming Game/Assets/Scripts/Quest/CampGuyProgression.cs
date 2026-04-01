using UnityEngine;

public class CampGuyProgression : MonoBehaviour
{
    [Header("Dialogue Stages")]
    public NpcDialogue dialogue1;
    public NpcDialogue dialogue2;
    public NpcDialogue dialogue3;
    public NpcDialogue endDialogue;

    [Header("References")]
    public NPC npc;

    private bool finished = false;

    void Start()
    {
        UpdateDialogueStage();
    }

    public void UpdateDialogueStage()
    {
        Debug.Log("UpdateDialogueStage called");

        string quest1ID = dialogue1.quest.questID;
        string quest2ID = dialogue2.quest.questID;
        string quest3ID = dialogue3.quest.questID;

        bool q1Done = QuestController.Instance.IsQuestHandedIn(quest1ID);
        bool q2Done = QuestController.Instance.IsQuestHandedIn(quest2ID);
        bool q3Done = QuestController.Instance.IsQuestHandedIn(quest3ID);

        Debug.Log("Q1 handed in: " + q1Done);
        Debug.Log("Q2 handed in: " + q2Done);
        Debug.Log("Q3 handed in: " + q3Done);

        if (!q1Done)
        {
            npc.dialogueData = dialogue1;
            Debug.Log("Camp Guy  Stage 1");
        }
        else if (!q2Done)
        {
            npc.dialogueData = dialogue2;
            Debug.Log("Camp Guy  Stage 2");
        }
        else if (!q3Done)
        {
            npc.dialogueData = dialogue3;
            Debug.Log("Camp Guy  Stage 3");
        }
        else
        {
            if (endDialogue != null)
                npc.dialogueData = endDialogue;

            Debug.Log("Camp Guy Finished");
        }

        if (npc.dialogueData != null && npc.dialogueData.quest != null)
        {
            Debug.Log("Camp Guy current quest ID is now: " + npc.dialogueData.quest.questID);
        }
    }

   

}
