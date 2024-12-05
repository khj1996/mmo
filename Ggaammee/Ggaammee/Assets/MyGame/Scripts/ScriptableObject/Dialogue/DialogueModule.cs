using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DialogueModule", menuName = "Dialogue/DialogueModule")]
public class DialogueModule : ScriptableObject
{
    public DialogueAction enterAction;
    [TextArea] public string[] dialogueLines;
    public List<DialogueTimelineCue> dialogueTimelineCues;
    public DialogueChoice[] choices;
}

[Serializable]
public class DialogueTimelineCue
{
    public string TimelineName;
    public int LineNumber;
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
            if (branch.branchNode != null && branch.branchNode.CheckCondition())
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
public class DialogueBranchNode
{
    public virtual bool CheckCondition()
    {
        return false;
    }
}

[Serializable]
public class DialogueQuestBranchNode : DialogueBranchNode
{
    public string questId;
    public QuestState requiredState;

    public override bool CheckCondition()
    {
        var questManager = Managers.QuestManager;

        return requiredState == questManager.GetQuestStatus(questId);
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