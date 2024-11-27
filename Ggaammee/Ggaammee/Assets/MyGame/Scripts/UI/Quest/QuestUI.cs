using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : UI_Base
{
    [SerializeField] private List<QuestUISub> activeQuestItems = new List<QuestUISub>();


    public override void Init()
    {
        Managers.QuestManager.OnChangeQuest += RefreshUI;
    }

    public void RefreshUI()
    {
        var currentQuset = Managers.QuestManager.activeQuests;


        for (int i = 0; i < activeQuestItems.Count; i++)
        {
            activeQuestItems[i].Initialize(currentQuset.First().Value);
        }
    }

    public void UpdateQuestProgress(string questId, string progress)
    {
        /*if (activeQuestItems.TryGetValue(questId, out var questUIItem))
        {
            questUIItem.UpdateProgress(progress);
        }*/
    }
}