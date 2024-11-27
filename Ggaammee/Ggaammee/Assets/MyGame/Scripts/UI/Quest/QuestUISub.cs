using TMPro;
using UnityEngine;

public class QuestUISub : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text questTitleText; 
    [SerializeField] private TMP_Text questDescriptionText; 
    [SerializeField] private TMP_Text questProgressText; 

    private Quest linkedQuest;

    public void Initialize(Quest quest)
    {
        linkedQuest = quest;

        questTitleText.text = quest.Data.title;
        questDescriptionText.text = quest.Data.description;
        questProgressText.text = GetProgressText();
    }

    public void UpdateProgress(string progress)
    {
        questProgressText.text = progress;
    }

    private string GetProgressText()
    {
        if (linkedQuest is KillMonsterQuest killQuest)
        {
            //return $"{killQuest.co}/{killQuest.m.TargetKillCount}";
        }
        else if (linkedQuest is CollectItemQuest itemQuest)
        {
            //return $"{itemQuest.CurrentItemCount}/{itemQuest.Data.TargetCount}";
        }
        else if (linkedQuest is ReachDestinationQuest)
        {
            return "Pending...";
        }

        return string.Empty;
    }
}