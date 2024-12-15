using UnityEngine;

[CreateAssetMenu(fileName = "KillMonsterQuest", menuName = "ScriptableObjects/Quest/KillMonsterQuest")]
public class KillMonsterQuestData : QuestData
{
    public string monsterId;  // 처치해야 할 몬스터 ID
    public int targetCount;   // 목표 처치 수

    public override QuestType Type => QuestType.KillMonster;
}