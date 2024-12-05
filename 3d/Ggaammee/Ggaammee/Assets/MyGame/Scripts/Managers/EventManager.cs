using System;
using UnityEngine;

public static class EventManager
{
    public static event Action<string, int> OnMonsterKilled;
    public static event Action<string, int> OnItemCollected;
    public static event Action<Vector3> OnPlayerMoved;

    public static void TriggerMonsterKilled(string monsterId, int count)
    {
        OnMonsterKilled?.Invoke(monsterId, count);
    }

    public static void TriggerItemCollected(string itemId, int count)
    {
        OnItemCollected?.Invoke(itemId, count);
    }

    public static void TriggerPlayerMoved(Vector3 position)
    {
        OnPlayerMoved?.Invoke(position);
    }
}