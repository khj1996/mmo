using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CreatureController : Poolable
{
    public CreatureData creatureData;
    [SerializeField] protected HpBar _hpBar;

    public CreatureStats stat;
    protected bool isDie;

    public Animator animator;

    protected bool _hasAnimator;
    protected CharacterController _controller;
    protected Transform _targetTransform;
    [SerializeField] protected Transform attackPoint;


    public event Action OnReturnToPoolAction;
    public event Action OnGetFromPoolAction;

    protected virtual void Init()
    {
        _hasAnimator = TryGetComponent(out animator);
        stat = new CreatureStats(creatureData);
        stat.ChangeHpEvent += _ => { _hpBar.UpdateHealthBar(stat.currentHp, stat.CurrentMaxHp); };
    }

    protected void LockAtTargetPosition()
    {
        var targetPosition = new Vector3(_targetTransform.position.x, transform.position.y, _targetTransform.position.z);
        var direction = (targetPosition - transform.position).normalized;

        var distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        _controller.Move(direction * distanceToTarget);
    }

    protected void LookAtTarget(Vector3 targetVector)
    {
        Vector3 up = transform.rotation * Vector3.up;

        transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(transform.forward, targetVector, up), up) * transform.rotation;
    }

    protected List<CharacterController> GetTargetInRange(Vector3 position, int targetLayer, float radius = 0.5f)
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

    public virtual void GetDamaged(float damage)
    {
        stat.HpChange(-damage);
    }

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