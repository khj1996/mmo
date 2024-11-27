using UnityEngine;

public class KillMonsterQuest : Quest
{
    public int currentKillCount = 0;
    public KillMonsterQuestData MonsterData => (KillMonsterQuestData)Data;

    public KillMonsterQuest(QuestData data) : base(data)
    {
    }

    public override void Subscribe()
    {
        EventManager.OnMonsterKilled += OnMonsterKilled;
    }

    public override void Unsubscribe()
    {
        EventManager.OnMonsterKilled -= OnMonsterKilled;
    }

    private void OnMonsterKilled(string monsterId, int count)
    {
        if (monsterId == MonsterData.monsterId)
        {
            currentKillCount += count;
            if (IsComplete())
            {
                Debug.Log($"Quest '{Data.title}' completed!");
            }
        }
    }

    public override bool IsComplete()
    {
        return currentKillCount >= MonsterData.targetCount;
    }
}