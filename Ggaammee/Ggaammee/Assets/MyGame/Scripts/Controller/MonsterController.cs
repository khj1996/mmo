using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    public Transform lockOnPos;

    private CreatureStateMachine<MonsterController> stateMachine;

    #region 초기화

    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        InitFSM();
        InitComponent();

        hp = creatureData.maxHp;
        _hpBar.UpdateHealthBar(hp, creatureData.maxHp);
        _hpBar.gameObject.transform.position = transform.TransformPoint(((MonsterData)creatureData).hpBarPos);
    }

    private void InitFSM()
    {
        stateMachine = new CreatureStateMachine<MonsterController>();

        stateMachine.AddState(new MonsterData.IdleState(this));
        stateMachine.AddState(new MonsterData.ChaseState(this));
        stateMachine.AddState(new MonsterData.AttackState(this));
        stateMachine.AddState(new MonsterData.DeadState(this));

        #region 상태 전이 조건

        #region ChaseState

        stateMachine.AddTransition<MonsterData.ChaseState, MonsterData.IdleState>(() => !CheckCanChase());

        #endregion

        #region AttackState

        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.ChaseState>(() => _AttackCoroutine == null && !CheckCanAttack());
        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.IdleState>(() => _AttackCoroutine == null && !_targetTransform);

        #endregion

        #region 전역

        stateMachine.AddGlobalTransition<MonsterData.AttackState>(CheckCanChase);
        stateMachine.AddGlobalTransition<MonsterData.DeadState>(() => isDie);

        #endregion

        #endregion

        stateMachine.ChangeState(typeof(MonsterData.IdleState));
    }


    private bool CheckCanChase()
    {
        if (!Managers.Instance.isInit)
            return false;


        _targetTransform = Managers.ObjectManager.GetNearestPlayer(transform.position, ((MonsterData)creatureData).sqrDetectionRange);

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

    #endregion


    private void Update()
    {
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
        Vector3 direction = (_targetTransform.position - transform.position).normalized;
        LookAtTarget(direction);

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        _AttackCoroutine = null;
    }

    public void DropItem()
    {
        List<MonsterData.DropItem> dropList = ((MonsterData)creatureData).dropItems;

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

        foreach (CharacterController monster in players)
        {
            monster.gameObject.GetComponent<PlayerController>().GetDamaged(creatureData.attack);
        }
    }

    #endregion

    public override void GetDamaged(float damage)
    {
        base.GetDamaged(damage);
        Debug.Log($"남은 체력 {hp}");

        if (hp <= 0)
        {
            isDie = true;
        }
    }
}