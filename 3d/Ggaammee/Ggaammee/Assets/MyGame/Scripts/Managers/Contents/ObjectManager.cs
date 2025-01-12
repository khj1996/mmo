using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public PlayerController MainPlayer;
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


    public List<CharacterController> GetTargetInRange(Vector3 position, int targetLayer, float radius = 0.5f)
    {
        var targetsInRange = new List<CharacterController>();

        var colliders = Physics.OverlapSphere(position, radius, targetLayer);

        foreach (Collider collider in colliders)
        {
            var target = collider.GetComponent<CharacterController>();

            if (target != null)
            {
                targetsInRange.Add(target);
            }
        }

        return targetsInRange;
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