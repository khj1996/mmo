using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DialogueModule", menuName = "Dialogue/DialogueModule")]
public class DialogueModule : ScriptableObject
{
    public DialogueAction enterAction;
    [TextArea] public string[] dialogueLines;
    public DialogueChoice[] choices;
}

[Serializable]
public class DialogueChoice
{
    public string choiceText; 
    public DialogueModule defaultNextModule; 
    public DialogueAction endAction;
    public ConditionalDialogueBranch[] conditionalBranches;

    public DialogueModule GetNextModule()
    {
        foreach (var branch in conditionalBranches)
        {
            if (branch.branchNode.CheckCondition())
            {
                return branch.nextModule;
            }
        }

        return defaultNextModule; 
    }
}

[Serializable]
public class ConditionalDialogueBranch
{
    [SerializeReference] public DialogueBranchNode branchNode; 
    public DialogueModule nextModule; 
}


[Serializable]
public abstract class DialogueBranchNode
{
    public abstract bool CheckCondition(); 
}

[Serializable]
public class DialogueQuestBranchNode : DialogueBranchNode
{
    public string questId; 
    public QuestState requiredState; 

    public override bool CheckCondition()
    {
        var questManager = Managers.QuestManager;

        if (questManager == null)
        {
            Debug.LogWarning("QuestManager not found.");
            return false;
        }

        return requiredState switch
        {
            QuestState.NotStarted => !questManager.IsQuestActive(questId) && !questManager.IsQuestCompleted(questId),
            QuestState.InProgress => questManager.IsQuestActive(questId),
            QuestState.Completed => questManager.IsQuestCompleted(questId),
            _ => false,
        };
    }
}

[Serializable]
public class DialogueLevelBranchNode : DialogueBranchNode
{
    public int requiredLevel;

    public override bool CheckCondition()
    {

        return requiredLevel > 0;
    }
}