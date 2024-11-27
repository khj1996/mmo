using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Transform questListParent;

    [SerializeField] private GameObject questItemPrefab;

    private Dictionary<string, QuestUISub> activeQuestItems = new Dictionary<string, QuestUISub>();

    public void RefreshUI(Dictionary<string, Quest> activeQuests)
    {
        foreach (Transform child in questListParent)
        {
            Destroy(child.gameObject);
        }

        activeQuestItems.Clear();

        foreach (var questPair in activeQuests)
        {
            var quest = questPair.Value;

            GameObject questItemObj = Instantiate(questItemPrefab, questListParent);
            QuestUISub questUIItem = questItemObj.GetComponent<QuestUISub>();

            questUIItem.Initialize(quest);
            activeQuestItems.Add(questPair.Key, questUIItem);
        }
    }

    public void UpdateQuestProgress(string questId, string progress)
    {
        if (activeQuestItems.TryGetValue(questId, out var questUIItem))
        {
            questUIItem.UpdateProgress(progress);
        }
    }
}