using UnityEngine;

[CreateAssetMenu(fileName = "ClearQuestAction", menuName = "Dialogue/DialogueAction/ClearQuestAction")]
public class ClearQuestAction : DialogueAction
{
    [SerializeField] private string questId = "shop_";

    public override void Execute()
    {
        Managers.QuestManager.CompleteQuest(questId);
    }
}