using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public List<PlayerController> Players { get; set; } = new List<PlayerController>();
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

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