using UnityEngine;

public class CollectItemQuest : Quest
{
    public int currentItemCount = 0;
    public CollectItemQuestData ItemQuestData => (CollectItemQuestData)Data;

    public CollectItemQuest(QuestData data) : base(data)
    {
        currentItemCount = Managers.InventoryManager.GetItemAmount(ItemQuestData.itemId);
        Debug.Log(currentItemCount);
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
        if (itemId == ItemQuestData.itemId)
        {
            currentItemCount = Managers.InventoryManager.GetItemAmount(itemId);
            InvokeOnUpdateProgress();
            Debug.Log($"Collected {currentItemCount}/{ItemQuestData.targetCount} {ItemQuestData.itemId}");

            if (CanComplete())
            {
                Debug.Log($"Quest '{Data.title}' completed!");
            }
        }
    }

    public override bool CanComplete()
    {
        return currentItemCount >= ItemQuestData.targetCount;
    }

    public override string GetProgress()
    {
        if (CanComplete())
        {
            return "완료";
        }
        else
        {
            var iteData = Managers.DataManager.ItemDatas.GetData(ItemQuestData.itemId);

            return $"{iteData.itemName} 수집 {currentItemCount}/{ItemQuestData.targetCount}";
        }
    }
}