using UnityEngine;

public class CollectItemQuest : Quest
{
    public int currentItemCount = 0;
    public CollectItemQuestData ItemData => (CollectItemQuestData)Data;

    public CollectItemQuest(QuestData data) : base(data)
    {
    }

    public override void Subscribe()
    {
        EventManager.OnItemCollected += OnItemCollected;
    }

    public override void Unsubscribe()
    {
        EventManager.OnItemCollected -= OnItemCollected;
    }

    private void OnItemCollected(string itemId, int count)
    {
        if (itemId == ItemData.itemId)
        {
            currentItemCount += count;
            Debug.Log($"Collected {currentItemCount}/{ItemData.targetCount} {ItemData.itemId}");

            if (IsComplete())
            {
                Debug.Log($"Quest '{Data.title}' completed!");
            }
        }
    }

    public override bool IsComplete()
    {
        return currentItemCount >= ItemData.targetCount;
    }
}