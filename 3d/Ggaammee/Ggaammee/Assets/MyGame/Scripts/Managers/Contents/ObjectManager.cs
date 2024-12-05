using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public PlayerController MainPlayer = new PlayerController();
    public List<PlayerController> Players { get; set; } = new List<PlayerController>();
    public List<MonsterController> Monsters { get; set; } = new List<MonsterController>();

    public void RegisterPlayer(PlayerController player)
    {
        if (player != null && !Players.Contains(player))
        {
            Players.Add(player);
        }
    }

    public void UnregisterPlayer(PlayerController player)
    {
        if (player != null && Players.Contains(player))
        {
            Players.Remove(player);
        }
    }

    public Transform GetNearestPlayer(Vector3 position, float sqrMaxDistance = Mathf.Infinity)
    {
        PlayerController closestPlayer = null;
        float sqrShortestDistance = sqrMaxDistance;

        foreach (var player in Players)
        {
            float sqrDistance = (player.transform.position - position).sqrMagnitude;
            if (sqrDistance < sqrShortestDistance)
            {
                sqrShortestDistance = sqrDistance;
                closestPlayer = player;
            }
        }

        return closestPlayer?.transform;
    }
}