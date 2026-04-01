using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).

public class NPC : MonoBehaviour, IInteractable
{
   public NpcDialogue dialogueData;
   private DialogueController dialogueController;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private enum QuestState { NotStarted, InProgress, Completed}
    private QuestState questState = QuestState.NotStarted;

    private void Awake()
    {
        dialogueController = DialogueController.Instance;
    }
    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if(dialogueData == null ||  (PauseController.IsGamePaused && !isDialogueActive))
            return;

        if (isDialogueActive)
        {
            NextLine();
        }

        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        if (dialogueController == null)
            dialogueController = DialogueController.Instance;

        if (dialogueController == null)
        {
            Debug.LogError($"DialogueController missing for NPC: {gameObject.name}");
            return;
        }

        //sync with quest data
        SyncQuestState();

        // set dialogue line based on questState
        if (questState == QuestState.NotStarted)
        {
            dialogueIndex = 0;
        }

        else if (questState == QuestState.InProgress)
        {
            dialogueIndex = dialogueData.questInProgressIndex;

        }

        else if (questState == QuestState.Completed)
        {
            dialogueIndex = dialogueData.questCompletedIndex;
        }

        isDialogueActive = true;


        dialogueController.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueController.ShowDialogueUI(true);
        PauseController.SetPause(true);

        DisplayCurrentLine();

    }

    private void SyncQuestState()
    {
        if (dialogueData == null || dialogueData.quest == null)
        {
            questState = QuestState.NotStarted;
            return;
        }

        string questID = dialogueData.quest.questID;

        if (QuestController.Instance.IsQuestCompleted(questID) || QuestController.Instance.IsQuestHandedIn(questID))
        {
            questState = QuestState.Completed;
        }
        else if (QuestController.Instance.IsQuestActive(questID))
        {
            questState = QuestState.InProgress;
        }
        else
        {
            questState = QuestState.NotStarted;
        }
    }

    void NextLine()
    {
        if (dialogueController == null)
            dialogueController = DialogueController.Instance;

        if (dialogueController == null)
        {
            Debug.LogError($"DialogueController missing in NextLine for NPC: {gameObject.name}");
            return;
        }

        if (isTyping)
        {
            //skip animation and show full line
            StopAllCoroutines();
            dialogueController.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }

        // clear any existing choices 
        dialogueController.ClearChoices();

        // check end dialogue lines
        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        // check if choices and display 
        foreach(DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                //display choices 
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            //if another line type next line
            DisplayCurrentLine();
        }
        else
        {
            // end dialogue 
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueController.SetDialogueText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueController.SetDialogueText(dialogueController.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);

            //display next line
            NextLine();
        }
    }

    void ChooseOption(int nextIndex, bool givesQuest)
    {
        if (givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;

            if (TutorialProgress.Instance != null && !TutorialProgress.Instance.firstTileHoed)
            {
                TutorialProgress.Instance.firstTileHoed = true;
                TutorialPopupManager.Instance.ShowPopup("Use your hoe on soil to till it.");
            }
        }
        
        else
        {
            Debug.LogWarning($"{gameObject.name} tried to give a quest, but no quest is assigned.");
        }

        dialogueIndex = nextIndex;
        dialogueController.ClearChoices();
        DisplayCurrentLine();

    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            bool givesQuest = choice.givesQuest[i];
            dialogueController.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, givesQuest) );
        }
    }
    public void EndDialogue()
    {
        Debug.Log("Ending dialogue. Quest state = " + questState);

        if (questState == QuestState.Completed && !QuestController.Instance.IsQuestHandedIn(dialogueData.quest.questID))
        {
            // handle quest completion 
            Debug.Log("Calling HandleQuestCompletion for: " + dialogueData.quest.questID);
            HandleQuestCompletion(dialogueData.quest);
        }

        StopAllCoroutines();
        isDialogueActive = false;
        if (dialogueController == null)
            dialogueController = DialogueController.Instance;

        if (dialogueController != null)
        {
            dialogueController.SetDialogueText("");
            dialogueController.ShowDialogueUI(false);
        }

        PauseController.SetPause(false);
    }

    void HandleQuestCompletion(Quests quest)
    {
        if (QuestController.Instance.IsQuestHandedIn(quest.questID))
            return;

        QuestController.Instance.HandInQuest(quest.questID);
        RewardsController.Instance.GiveQuestReward(quest);

        CampGuyProgression progression = FindObjectOfType<CampGuyProgression>();
        if (progression != null)
        {
            progression.UpdateDialogueStage();
            Debug.Log("Camp Guy progression updated after hand-in");
        }
        else
        {
            Debug.LogWarning("CampGuyProgression not found in scene");
        }
    }
}
