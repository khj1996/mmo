using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteract
{
    private readonly PlayerController playerController;
    private readonly InputSystem inputSystem;
    private readonly Collider[] npcList = new Collider[3];

    private List<DropItem> currentDropItems;

    public PlayerInteract(PlayerController controller, InputSystem _inputSystem)
    {
        playerController = controller;
        inputSystem = _inputSystem;
        Init();
    }

    public void Init()
    {
        currentDropItems = new List<DropItem>();
    }

    public void Interact()
    {
        if (playerController.isNearLadder && inputSystem.interaction)
        {
            playerController.isClimbing = true;
            return;
        }

        if (inputSystem.interaction && currentDropItems.Count > 0)
        {
            DropItem closestItem = GetClosestObject(currentDropItems, item => item.transform.position);
            if (closestItem != null)
            {
                closestItem.Interact(playerController);
                currentDropItems.Remove(closestItem);
            }

            return;
        }

        if (inputSystem.interaction)
        {
            Npc closestNpc = GetClosestNpc();
            if (closestNpc != null)
            {
                closestNpc.Interact();
            }
        }
    }

    private Npc GetClosestNpc()
    {
        int result = Physics.OverlapSphereNonAlloc(playerController.transform.position, 1.0f, npcList, LayerData.NpcLayer);
        if (result == 0)
        {
            return null;
        }

        var npcs = npcList
            .Select(x => x ? x.GetComponent<Npc>() : null)
            .Where(npc => npc != null)
            .ToList();

        return GetClosestObject(npcs, npc => npc.transform.position);
    }

    private T GetClosestObject<T>(List<T> objects, Func<T, Vector3> positionSelector) where T : class
    {
        T closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (var obj in objects)
        {
            float distance = Vector3.Distance(playerController.transform.position, positionSelector(obj));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        return closestObject;
    }

    private void EnterLadderPosition(Collider other)
    {
        if (playerController.isClimbing) return;
        playerController.isNearLadder = true;
        playerController.targetTransform = other.gameObject.transform;
    }

    private void ExitLadderPosition()
    {
        if (playerController.isClimbing) return;
        playerController.isNearLadder = false;
        playerController.targetTransform = null;
    }

    private void EndofLadder(int animName)
    {
        if (playerController.isClimbing && !playerController.inLadderMotion)
        {
            playerController.animator.SetTrigger(animName);
            playerController.animator.SetBool(AssignAnimationIDs.AnimIDLadder, false);
        }

        playerController.inLadderMotion = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item") && other.TryGetComponent<DropItem>(out var dropItem))
        {
            if (!currentDropItems.Contains(dropItem))
            {
                currentDropItems.Add(dropItem);
            }
        }
        else if (other.CompareTag(TagData.LadderBottomTag))
        {
            if (playerController.isClimbing)
            {
                EndofLadder(AssignAnimationIDs.AnimIDLadderDownEnd);
            }
            else
            {
                EnterLadderPosition(other);

                if (playerController.navMeshAgent.isOnOffMeshLink && playerController.isAutoMove)
                {
                    var offMeshData = playerController.navMeshAgent.currentOffMeshLinkData;
                    if (offMeshData.offMeshLink.endTransform.position.y > playerController.transform.position.y)
                    {
                        playerController.isUpLadder = true;
                    }

                    playerController.DisableNavMesh();
                    playerController.isClimbing = true;
                }
            }
        }
        else if (other.CompareTag(TagData.LadderTopTag))
        {
            if (playerController.isClimbing)
            {
                EndofLadder(AssignAnimationIDs.AnimIDLadderUpEnd);
            }
            else
            {
                EnterLadderPosition(other);

                if (playerController.navMeshAgent.isOnOffMeshLink && playerController.isAutoMove)
                {
                    var offMeshData = playerController.navMeshAgent.currentOffMeshLinkData;
                    if (offMeshData.offMeshLink.startTransform.position.y < playerController.transform.position.y)
                    {
                        playerController.isUpLadder = false;
                    }

                    playerController.DisableNavMesh();
                    playerController.isClimbing = true;
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item") && other.TryGetComponent<DropItem>(out var dropItem))
        {
            currentDropItems.Remove(dropItem);
        }
        else if (other.CompareTag(TagData.LadderBottomTag))
        {
            if (playerController.isClimbing)
            {
                EndofLadder(AssignAnimationIDs.AnimIDLadderDownEnd);
            }
            else
            {
                ExitLadderPosition();
            }
        }
        else if (other.CompareTag(TagData.LadderTopTag))
        {
            if (playerController.isClimbing)
            {
                EndofLadder(AssignAnimationIDs.AnimIDLadderUpEnd);
            }
            else
            {
                ExitLadderPosition();
            }
        }
    }
}