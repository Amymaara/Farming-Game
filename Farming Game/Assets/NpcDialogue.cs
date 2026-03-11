using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NpcDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;

    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public bool[] endDialogueLines;
    public float autoProgressDelay = 1.5f;

    public DialogueChoice[] choices;
}

[System.Serializable]

public class DialogueChoice
{
    public int dialogueIndex; // dialogue line where choice appears
    public string[] choices; // player response 
    public int[] nextDialogueIndexes; // where choice leads

}