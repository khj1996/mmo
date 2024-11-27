using UnityEngine;

public class ReachDestinationQuest : Quest
{
    private ReachDestinationQuestData DestinationData => (ReachDestinationQuestData)Data;
    private bool hasReached = false;

    public ReachDestinationQuest(QuestData data) : base(data) { }

    public override void Subscribe()
    {
        EventManager.OnPlayerMoved += CheckPlayerPosition;
    }

    public override void Unsubscribe()
    {
        EventManager.OnPlayerMoved -= CheckPlayerPosition;
    }

    private void CheckPlayerPosition(Vector3 playerPosition)
    {
        float distance = Vector3.Distance(playerPosition, DestinationData.targetPosition);
        if (distance <= DestinationData.radius)
        {
            hasReached = true;
            Debug.Log($"Player reached the destination for quest '{Data.title}'!");

            if (IsComplete())
            {
                Debug.Log($"Quest '{Data.title}' completed!");
            }
        }
    }

    public override bool IsComplete()
    {
        return hasReached;
    }
}