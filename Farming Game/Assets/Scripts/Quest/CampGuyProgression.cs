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
        if (npc == null)
        {
            Debug.LogError("NPC reference missing");
            return;
        }

        if (dialogue1 == null || dialogue2 == null || dialogue3 == null)
        {
            Debug.LogError("One or more Camp Guy dialogue stages are missing");
            return;
        }

        if (dialogue1.quest == null || dialogue2.quest == null || dialogue3.quest == null)
        {
            Debug.LogError("One or more dialogue assets are missing their quest reference");
            return;
        }

        string quest1ID = dialogue1.quest.questID;
        string quest2ID = dialogue2.quest.questID;
        string quest3ID = dialogue3.quest.questID;

        bool q1Done = QuestController.Instance.IsQuestHandedIn(quest1ID);
        bool q2Done = QuestController.Instance.IsQuestHandedIn(quest2ID);
        bool q3Done = QuestController.Instance.IsQuestHandedIn(quest3ID);

        if (!q1Done)
        {
            npc.dialogueData = dialogue1;
            Debug.Log("Camp Guy - Stage 1");
        }
        else if (!q2Done)
        {
            npc.dialogueData = dialogue2;
            Debug.Log("Camp Guy - Stage 2");
        }
        else if (!q3Done)
        {
            npc.dialogueData = dialogue3;
            Debug.Log("Camp Guy - Stage 3");
        }
        else
        {
            if (endDialogue != null)
                npc.dialogueData = endDialogue;

            Debug.Log("Camp Guy - Finished");

            if (!finished)
            {
                finished = true;
                PlayRepairMoment();
            }
        }
    }

    private void PlayRepairMoment()
    {
        Debug.Log("Camp Guy starts repairing the bridge.");

        Animator animator = npc.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Repair");
        }
    }

}
