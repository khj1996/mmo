using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CreatureController : MonoBehaviour
{
    public CreatureData creatureData;
    [SerializeField] protected HpBar _hpBar;

    protected float hp;
    protected bool isDie;

    public Animator animator;
    public Transform attackPoint;

    protected bool _hasAnimator;
    protected CharacterController _controller;
    protected Transform _targetTransform;

    protected virtual void Init()
    {
        _hasAnimator = TryGetComponent(out animator);
    }

    public void LockAtTargetPsition()
    {
        Vector3 targetPosition = new Vector3(this._targetTransform.position.x, transform.position.y, this._targetTransform.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        _controller.Move(direction * distanceToTarget);
    }

    public void LookAtTarget(Vector3 targetVector)
    {
        Vector3 up = transform.rotation * Vector3.up;

        transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(transform.forward, targetVector, up), up) * transform.rotation;
    }

    public List<CharacterController> GetTargetInRange(Vector3 position, int targetLayer, float radius = 0.5f)
    {
        List<CharacterController> targetsInRange = new List<CharacterController>();

        Collider[] colliders = Physics.OverlapSphere(position, radius, targetLayer);

        foreach (Collider collider in colliders)
        {
            CharacterController target = collider.GetComponent<CharacterController>();

            if (target != null)
            {
                targetsInRange.Add(target);
            }
        }

        return targetsInRange;
    }

    public virtual void GetDamaged(float damage)
    {
        hp -= damage;
        _hpBar.UpdateHealthBar(hp, creatureData.maxHp);
    }
}