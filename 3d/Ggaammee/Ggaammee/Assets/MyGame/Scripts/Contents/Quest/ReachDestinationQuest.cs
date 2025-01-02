using UnityEngine;

public class ReachDestinationQuest : Quest
{
    private ReachDestinationQuestData DestinationQuestData => (ReachDestinationQuestData)Data;
    private float currentDistance;

    public ReachDestinationQuest(QuestData data) : base(data)
    {
        var test = Object.FindObjectOfType<GuideLine>();

        test.StartGuide(DestinationQuestData.targetPosition);
    }

    public override void Subscribe()
    {
        EventManager.OnPlayerMoved += UpdateDistance;
    }

    public override void Unsubscribe()
    {
        EventManager.OnPlayerMoved -= UpdateDistance;
    }

    private void UpdateDistance(Vector3 playerPosition)
    {
        
        float distance = Vector3.Distance(playerPosition, DestinationQuestData.targetPosition);
        currentDistance = distance;
        InvokeOnUpdateProgress();
    }

    public override bool CanComplete()
    {
        return currentDistance <= DestinationQuestData.radius;
    }

    public override string GetProgress()
    {
        if (CanComplete())
        {
            return "목표지점에 도착";
        }
        else
        {
            return currentDistance.ToString("F1");
        }
    }
}