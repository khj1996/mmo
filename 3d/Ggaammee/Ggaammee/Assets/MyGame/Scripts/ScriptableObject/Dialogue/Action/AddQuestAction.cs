using UnityEngine;

[CreateAssetMenu(fileName = "AddQuestAction", menuName = "Dialogue/DialogueAction/AddQuestAction")]
public class AddQuestAction : DialogueAction
{
    [SerializeField] private string questId = "quest_";

    public override void Execute()
    {
        Managers.QuestManager.AddQuest(questId);
    }
}