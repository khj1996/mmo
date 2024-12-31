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
    [SerializeField] protected CharacterController controller;
    [SerializeField] protected Transform attackPoint;

    protected bool isDie;
    protected bool hasAnimator;
    protected Transform targetTransform;

    // 3. Unity Lifecycle or Initialization
    protected virtual void Init()
    {
        hasAnimator = TryGetComponent(out animator);
        stat = new CreatureStats(creatureData);
        stat.ChangeHpEvent += _ => { hpBar.UpdateHealthBar(stat.currentHp, stat.CurrentMaxHp); };
    }

    // 4. Core Logic Methods
    protected void LockAtTargetPosition()
    {
        var targetPosition = new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z);
        var direction = (targetPosition - transform.position).normalized;

        var distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        controller.Move(direction * distanceToTarget);
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