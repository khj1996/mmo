using UnityEngine;

[CreateAssetMenu(fileName = "ReachDestinationQuest", menuName = "ScriptableObjects/Quest/ReachDestinationQuest")]
public class ReachDestinationQuestData : QuestData
{
    public Vector3 targetPosition; // 도착해야 할 위치
    public float radius; // 도착 확인을 위한 허용 반경

    public override QuestType Type => QuestType.ReachDestination;
}