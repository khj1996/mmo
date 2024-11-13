using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CreatureController : MonoBehaviour
{
    public CreatureData creatureData;

    private float hp;

    public Animator animator;
    protected CharacterController _controller;
    protected bool _hasAnimator;
    public Transform AttackPoint;


    protected Vector3 targetPosition;
    protected Vector3 lookTarget;

    protected virtual void Init()
    {
        _hasAnimator = TryGetComponent(out animator);
    }

    public void LockAtTargetPsition()
    {
        Vector3 targetPosition = new Vector3(this.targetPosition.x, transform.position.y, this.targetPosition.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        float distanceToLadder = Vector3.Distance(transform.position, targetPosition);

        _controller.Move(direction * distanceToLadder);
    }

    public void LookAtTarget()
    {
        Vector3 up = transform.rotation * Vector3.up;

        transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(transform.forward, lookTarget, up), up) * transform.rotation;
    }
}