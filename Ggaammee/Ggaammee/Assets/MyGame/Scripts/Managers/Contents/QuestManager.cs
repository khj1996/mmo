﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager
{
    public Dictionary<string, Quest> activeQuests { get; private set; } = new Dictionary<string, Quest>(); // 진행 중
    public Dictionary<string, Quest> completedQuests { get; private set; } = new Dictionary<string, Quest>(); // 완료된 퀘스트

    public event Action OnChangeQuest;

    public void AddTestQuest()
    {
        AddQuest("quest_200");
    }

    public void AddQuest(string questId)
    {
        if (completedQuests.ContainsKey(questId))
        {
            return;
        }

        if (activeQuests.ContainsKey(questId))
        {
            return;
        }

        var questData = Managers.DataManager.QuestDatas.GetData(questId);
        if (questData == null)
        {
            return;
        }

        Quest newQuest = CreateQuestInstance(questData);
        activeQuests.Add(questId, newQuest);

        newQuest.Subscribe();
        OnChangeQuest?.Invoke();
    }

    public void CompleteQuest(string questId)
    {
        if (!activeQuests.TryGetValue(questId, out Quest quest))
        {
            return;
        }

        if (!quest.CanComplete())
            return;

        quest.Unsubscribe();

        activeQuests.Remove(questId);
        completedQuests.Add(questId, quest);
        OnChangeQuest?.Invoke();
    }

    public QuestState GetQuestStatus(string questId)
    {
        if (completedQuests.ContainsKey(questId))
        {
            return QuestState.Completed;
        }

        if (activeQuests.TryGetValue(questId, out var activeQuest))
        {
            return activeQuest.CanComplete() ? QuestState.CanComplete : QuestState.InProgress;
        }

        return QuestState.NotStarted;
    }


    private Quest CreateQuestInstance(QuestData data)
    {
        return data.Type switch
        {
            QuestType.KillMonster => new KillMonsterQuest(data),
            QuestType.CollectItem => new CollectItemQuest(data),
            QuestType.ReachDestination => new ReachDestinationQuest(data),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}