using TMPro;
using UnityEngine;

public class QuestUISub : MonoBehaviour
{
    [Header("UI Elements")] [SerializeField]
    private TMP_Text questTitleText;

    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private TMP_Text questProgressText;

    private Quest linkedQuest;

    public void Initialize(Quest quest)
    {
        if (quest == null)
        {
            ResetUI();
            return;
        }

        linkedQuest = quest;
        questTitleText.text = quest.Data.title;
        questDescriptionText.text = quest.Data.description;
        questProgressText.text = linkedQuest.GetProgress();
        linkedQuest.OnUpdateProgress += UpdateProgressUI;
    }

    private void UpdateProgressUI()
    {
        questProgressText.text = linkedQuest?.GetProgress();
    }

    private void ResetUI()
    {
        linkedQuest.OnUpdateProgress -= UpdateProgressUI;
        linkedQuest = null;
        questTitleText.text = "진행중인 퀘스트가 없습니다";
        questDescriptionText.text = "대기 중";
        questProgressText.text = "";
    }
}