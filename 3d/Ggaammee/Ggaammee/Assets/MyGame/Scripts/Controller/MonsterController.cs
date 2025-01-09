using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterController : CreatureController
{
    public Transform lockOnPos;
    private MonsterData MonsterData => (MonsterData)creatureData;
    private CreatureStateMachine<MonsterController> stateMachine;

    #region 초기화

    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        InitFSM();
        InitComponent();

        OnReturnToPoolAction += () =>
        {
            isDie = false;
            targetTransform = null;
            _AttackCoroutine = null;

            transform.localPosition = Vector3.zero;
            stat.ResetData(creatureData);
            stateMachine.ChangeState(typeof(MonsterData.IdleState));
        };
        hpBar.gameObject.transform.position = transform.TransformPoint(MonsterData.hpBarPos);
    }

    private void InitFSM()
    {
        stateMachine = new CreatureStateMachine<MonsterController>();

        RegisterStates();
        RegisterTransitions();

        stateMachine.ChangeState(typeof(MonsterData.IdleState));
    }

    private void RegisterStates()
    {
        stateMachine.AddState(new MonsterData.IdleState(this));
        stateMachine.AddState(new MonsterData.ChaseState(this));
        stateMachine.AddState(new MonsterData.AttackState(this));
        stateMachine.AddState(new MonsterData.DeadState(this));
    }

    private void RegisterTransitions()
    {
        stateMachine.AddTransition<MonsterData.IdleState, MonsterData.ChaseState>(CheckCanChase);

        stateMachine.AddTransition<MonsterData.ChaseState, MonsterData.IdleState>(() => !CheckCanChase());

        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.ChaseState>(() => _AttackCoroutine == null && !CheckCanAttack());
        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.IdleState>(() => _AttackCoroutine == null && !targetTransform);

        stateMachine.AddGlobalTransition<MonsterData.AttackState>(CheckCanAttack);
        stateMachine.AddGlobalTransition<MonsterData.DeadState>(() => isDie);
    }


    private bool CheckCanChase()
    {
        if (!Managers.Instance.isInit)
            return false;


        targetTransform = Managers.ObjectManager.GetNearestPlayer(transform.position, MonsterData.sqrDetectionRange);

        return targetTransform;
    }

    private bool CheckCanAttack()
    {
        return targetTransform && !((targetTransform.position - transform.position).sqrMagnitude > MonsterData.minSqrAttackRange);
    }

    private void InitComponent()
    {
        hpBar = GetComponentInChildren<HpBar>();
        controller = GetComponent<CharacterController>();
    }

    public void SetPos(Vector3 newPos)
    {
        Vector3 newPosition = newPos;
        Vector3 delta = newPosition - transform.position;
        controller.Move(delta);
    }

    #endregion


    private void Update()
    {
        stateMachine.Update();
    }

    private float _speed;

    public void Move()
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;

        LookAtTarget(direction);

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        _speed = Mathf.Lerp(currentHorizontalSpeed, creatureData.speed, Time.deltaTime * creatureData.acceleration);
        _speed = Mathf.Round(_speed * 1000f) / 300f;

        controller.Move(direction.normalized * (_speed * Time.deltaTime));
    }

    #region 공격

    private float lastAttackTime = 0f;
    private Coroutine _AttackCoroutine = null;

    public void CheckAttack()
    {
        if (_AttackCoroutine != null || !(Time.time >= lastAttackTime + MonsterData.minAttackSpeed)) return;

        _AttackCoroutine = StartCoroutine(AttackCoroutine());
        lastAttackTime = Time.time;
    }

    private IEnumerator AttackCoroutine()
    {
        animator.SetTrigger(AssignAnimationIDs.AnimIDAttackTrigger);
        LookAtTarget((targetTransform.position - transform.position).normalized);

        while (!IsAnimationComplete(AssignAnimationIDs.AnimIDAttackTrigger))
        {
            yield return null;
        }

        _AttackCoroutine = null;
    }

    private bool IsAnimationComplete(int animationID)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime >= 1f;
    }

    public void DropItem()
    {
        List<MonsterData.DropItem> dropList = MonsterData.dropItems;

        foreach (var dropItem in dropList)
        {
            if (CheckDropItem(dropItem.dropRate))
            {
                Vector3 dropPosition = transform.position;
                Managers.DropManager.DropItem(dropItem.itemData, dropPosition);
            }
        }
    }

    private bool CheckDropItem(int dropRate)
    {
        return Random.Range(0, 100) < dropRate;
    }


    protected virtual void OnHit(AnimationEvent animationEvent)
    {
        List<CharacterController> players = GetTargetInRange(Util.GetModifiedPoint(transform, MonsterData.defaultAttackPoint), LayerData.PlayerLayer);

        foreach (CharacterController player in players)
        {
            player.gameObject.GetComponent<PlayerController>().GetDamaged(stat.CurrentAttackPower);
        }
    }

    #endregion

    public override void GetDamaged(float damage)
    {
        base.GetDamaged(damage);

        if (stat.currentHp <= 0)
        {
            EventManager.TriggerMonsterKilled(MonsterData.creatureId, 1);
            isDie = true;
        }
    }
}