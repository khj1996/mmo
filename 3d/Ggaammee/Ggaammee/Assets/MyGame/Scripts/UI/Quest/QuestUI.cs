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

        Util.BindEvent(gameObject, _ => { StartAutoMove(); });
    }

    public void RefreshUI()
    {
        var currentQuset = Managers.QuestManager.activeQuests;


        for (int i = 0; i < activeQuestItems.Count; i++)
        {
            activeQuestItems[i].Initialize(currentQuset.FirstOrDefault().Value);
        }
    }

    public void StartAutoMove()
    {
        Debug.Log("자동이동 시작");
        if (activeQuestItems[0].linkedQuest.Data is ReachDestinationQuestData data)
        {
            Managers.ObjectManager.MainPlayer.MoveAuto(data.targetPosition);
            
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