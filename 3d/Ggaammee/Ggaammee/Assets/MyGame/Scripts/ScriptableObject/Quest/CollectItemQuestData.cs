using UnityEngine;

[CreateAssetMenu(fileName = "CollectItemQuest", menuName = "ScriptableObjects/Quest/CollectItemQuest")]
public class CollectItemQuestData : QuestData
{
    public string itemId; // 수집해야 할 아이템 ID
    public int targetCount; // 목표 수집 개수

    public override QuestType Type => QuestType.CollectItem;
}