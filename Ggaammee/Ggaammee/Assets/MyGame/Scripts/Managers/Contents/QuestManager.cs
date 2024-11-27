using System.Collections.Generic;
using UnityEngine;

public class QuestManager
{
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>(); // 진행 중
    private Dictionary<string, Quest> completedQuests = new Dictionary<string, Quest>(); // 완료된 퀘스트

    [SerializeField] private List<QuestData> questDataList; // ScriptableObject 데이터

    public void AddQuest(string questId)
    {
        if (completedQuests.ContainsKey(questId))
        {
            Debug.LogWarning($"Quest with ID {questId} is already completed.");
            return;
        }

        if (activeQuests.ContainsKey(questId))
        {
            Debug.LogWarning($"Quest with ID {questId} is already active.");
            return;
        }

        var questData = questDataList.Find(q => q.id == questId);
        if (questData == null)
        {
            Debug.LogWarning($"QuestData with ID {questId} not found!");
            return;
        }

        Quest newQuest = CreateQuestInstance(questData);
        activeQuests.Add(questId, newQuest);

        newQuest.Subscribe();
        Debug.Log($"Quest '{newQuest.Data.title}' added.");
    }

    public void CompleteQuest(string questId)
    {
        if (!activeQuests.TryGetValue(questId, out Quest quest))
        {
            Debug.LogWarning($"Quest with ID {questId} is not active.");
            return;
        }

        quest.Unsubscribe();

        activeQuests.Remove(questId);
        completedQuests.Add(questId, quest);

        Debug.Log($"Quest '{quest.Data.title}' completed and moved to completed quests.");
    }

    public bool IsQuestActive(string questId)
    {
        return activeQuests.ContainsKey(questId);
    }

    public bool IsQuestCompleted(string questId)
    {
        return completedQuests.ContainsKey(questId);
    }

    private Quest CreateQuestInstance(QuestData data)
    {
        return data.Type switch
        {
            QuestType.KillMonster => new KillMonsterQuest(data),
            QuestType.CollectItem => new CollectItemQuest(data),
            QuestType.ReachDestination => new CollectItemQuest(data),
        };
    }
}