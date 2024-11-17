using UnityEngine;

[CreateAssetMenu(fileName = "DialogueModule", menuName = "Dialogue/DialogueModule")]
public class DialogueModule : ScriptableObject
{
    [TextArea]
    public string[] dialogueLines;
    public DialogueChoice[] choices; 
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText; 
    public DialogueModule nextModule; 
    public DialogueAction action; 
}