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
            _targetTransform = null;
            _AttackCoroutine = null;

            transform.localPosition = Vector3.zero;
            stat.ResetData(creatureData);
            stateMachine.ChangeState(typeof(MonsterData.IdleState));
        };
        _hpBar.gameObject.transform.position = transform.TransformPoint(MonsterData.hpBarPos);
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
        // IdleState
        stateMachine.AddTransition<MonsterData.IdleState, MonsterData.ChaseState>(CheckCanChase);

        // ChaseState
        stateMachine.AddTransition<MonsterData.ChaseState, MonsterData.IdleState>(() => !CheckCanChase());

        // AttackState
        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.ChaseState>(() => _AttackCoroutine == null && !CheckCanAttack());
        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.IdleState>(() => _AttackCoroutine == null && !_targetTransform);

        // Global
        stateMachine.AddGlobalTransition<MonsterData.AttackState>(CheckCanAttack);
        stateMachine.AddGlobalTransition<MonsterData.DeadState>(() => isDie);
    }


    private bool CheckCanChase()
    {
        if (!Managers.Instance.isInit)
            return false;


        _targetTransform = Managers.ObjectManager.GetNearestPlayer(transform.position, MonsterData.sqrDetectionRange);

        return _targetTransform;
    }

    private bool CheckCanAttack()
    {
        return _targetTransform && !((_targetTransform.position - transform.position).sqrMagnitude > creatureData.sqrAttackRange);
    }

    private void InitComponent()
    {
        _hpBar = GetComponentInChildren<HpBar>();
        _controller = GetComponent<CharacterController>();
    }

    public void SetPos(Vector3 newPos)
    {
        Vector3 newPosition = newPos;
        Vector3 delta = newPosition - transform.position;
        _controller.Move(delta);
    }

    #endregion


    private void Update()
    {
        Debug.Log(stateMachine.CurrentState);
        stateMachine.Update();
    }

    private float _speed;

    public void Move()
    {
        Vector3 direction = (_targetTransform.position - transform.position).normalized;

        LookAtTarget(direction);

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        _speed = Mathf.Lerp(currentHorizontalSpeed, creatureData.speed, Time.deltaTime * creatureData.acceleration);
        _speed = Mathf.Round(_speed * 1000f) / 1000f;

        _controller.Move(direction.normalized * (_speed * Time.deltaTime));
    }

    #region 공격

    private float lastAttackTime = 0f;
    private Coroutine _AttackCoroutine = null;

    public void CheckAttack()
    {
        if (_AttackCoroutine != null || !(Time.time >= lastAttackTime + creatureData.attackSpeed)) return;

        _AttackCoroutine = StartCoroutine(AttackCoroutine());
        lastAttackTime = Time.time;
    }

    private IEnumerator AttackCoroutine()
    {
        animator.SetTrigger(AssignAnimationIDs.AnimIDAttackTrigger);
        LookAtTarget((_targetTransform.position - transform.position).normalized);

        // 애니메이션 대기
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
        List<CharacterController> players = GetTargetInRange(attackPoint.position, LayerData.PlayerLayer);

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
            isDie = true;
        }
    }
}