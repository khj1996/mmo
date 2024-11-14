using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    public Transform lockOnPos;

    private PlayerStateMachine<MonsterController> stateMachine;

    [SerializeField] private HpBar _hpBar;

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
        _hpBar.gameObject.transform.position = transform.TransformPoint(((MonsterData)creatureData).hpBarPos);

        _hpBar.UpdateHealthBar(hp, creatureData.maxHp);
    }

    private void InitFSM()
    {
        stateMachine = new PlayerStateMachine<MonsterController>();

        stateMachine.AddState(new MonsterData.IdleState(this));
        stateMachine.AddState(new MonsterData.ChaseState(this));
        stateMachine.AddState(new MonsterData.AttackState(this));

        #region 상태 전이 조건

        #region IdleState

        stateMachine.AddTransition<MonsterData.IdleState, MonsterData.ChaseState>(CheckCanChase);

        #endregion


        #region ChaseState

        stateMachine.AddTransition<MonsterData.ChaseState, MonsterData.AttackState>(CheckCanAttack);
        stateMachine.AddTransition<MonsterData.ChaseState, MonsterData.IdleState>(() => !CheckCanChase());

        #endregion

        #region AttackState

        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.ChaseState>(() => _AttackCoroutine == null && !CheckCanAttack());
        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.IdleState>(() => _AttackCoroutine == null && !_targetTransform);

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
        //_bt.Evaluate(); // BT에서 조건을 평가
        stateMachine.Update();
        Debug.Log(stateMachine.CurrentState);
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
        //공격 쿨타임이거나 모션 진행중
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

    #endregion

    public override void GetDamaged(float damage)
    {
        base.GetDamaged(damage);
        Debug.Log($"남은 체력 {hp}");
        _hpBar.UpdateHealthBar(hp, creatureData.maxHp);

        if (hp <= 0)
        {
            isDie = true;
        }
    }
}