using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CreatureController : Poolable
{
    public CreatureData creatureData;
    public CreatureStats stat;
    public Animator animator;

    public event Action OnReturnToPoolAction;
    public event Action OnGetFromPoolAction;

    [SerializeField] protected HpBar hpBar;
    [SerializeField] public CharacterController controller;

    protected bool isDie;
    public Transform targetTransform;

    protected virtual void Init()
    {
        stat = new CreatureStats(creatureData);
        stat.ChangeHpEvent += _ => { hpBar.UpdateHealthBar(stat.currentHp, stat.CurrentMaxHp); };
    }

    protected void LockAtTargetPosition()
    {
        var targetPosition = new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z);
        var direction = (targetPosition - transform.position).normalized;

        var distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        controller.Move(direction * distanceToTarget);
    }

    public void LookAtTarget(Vector3 targetVector)
    {
        Vector3 up = transform.rotation * Vector3.up;

        transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(transform.forward, targetVector, up), up) * transform.rotation;
    }

    public virtual void GetDamaged(float damage)
    {
        stat.HpChange(-damage);
    }

    // 5. Poolable Methods
    public override void OnGetFromPool()
    {
        OnGetFromPoolAction?.Invoke();
    }

    public override void OnReturnToPool()
    {
        OnReturnToPoolAction?.Invoke();
        gameObject.SetActive(false);
    }
}