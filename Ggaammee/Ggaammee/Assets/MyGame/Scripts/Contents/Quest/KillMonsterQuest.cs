using System;
using UnityEngine;

public class KillMonsterQuest : Quest
{
    public int currentKillCount = 0;
    public KillMonsterQuestData MonsterQuestData => (KillMonsterQuestData)Data;


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
        if (monsterId == MonsterQuestData.monsterId)
        {
            currentKillCount += count;
            
            InvokeOnUpdateProgress();

            if (CanComplete())
            {
                Debug.Log($"Quest '{Data.title}' completed!");
            }
        }
    }


    public override bool CanComplete()
    {
        return currentKillCount >= MonsterQuestData.targetCount;
    }

    public override string GetProgress()
    {
        if (CanComplete())
        {
            return "완료";
        }
        else
        {
            return $"{currentKillCount}/{MonsterQuestData.targetCount}";
        }
    }
}