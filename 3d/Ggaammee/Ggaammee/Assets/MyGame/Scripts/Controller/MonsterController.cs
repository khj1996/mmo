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

    //다음 스킬 사용 가능 시간
    private float[] canUseSkillTimes;

    private int currentSkillIndex;


    #region 초기화

    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        InitComponent();
        InitFSM();
    }

    public void SetData()
    {
        canUseSkillTimes = new float[MonsterData.SkillDatas.Length];

        isDie = false;
        targetTransform = null;
        _speed = 0;
        controller.Move(Vector3.zero);

        hpBar.gameObject.transform.position = transform.TransformPoint(MonsterData.hpBarPos);

        stat.ResetStat(creatureData);
        stateMachine.ChangeState(typeof(MonsterData.IdleState));
    }

    private void InitFSM()
    {
        stateMachine = new CreatureStateMachine<MonsterController>();

        RegisterStates();
        RegisterTransitions();
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

        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.ChaseState>(() => !isInMotion && GetAttackActionIndex() == -1);
        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.IdleState>(() => !isInMotion && !targetTransform);

        stateMachine.AddGlobalTransition<MonsterData.AttackState>(() => GetAttackActionIndex() != -1);
        stateMachine.AddGlobalTransition<MonsterData.DeadState>(() => isDie);
    }


    private bool CheckCanChase()
    {
        if (!Managers.Instance.isInit)
            return false;


        targetTransform = Managers.ObjectManager.GetNearestPlayer(transform.position, MonsterData.sqrDetectionRange);

        return targetTransform;
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

    [SerializeField] private float _speed;

    public void Move()
    {
        if (isInMotion)
            return;

        Vector3 direction = (targetTransform.position - transform.position).normalized;
        direction.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 100f);

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        if (currentHorizontalSpeed < 0.01f)
            currentHorizontalSpeed = 0;

        _speed = Mathf.Lerp(currentHorizontalSpeed, creatureData.speed, Time.deltaTime * creatureData.acceleration);

        if (_speed > creatureData.speed)
        {
            controller.Move(Vector3.zero);
        }
        else
        {
            controller.Move(direction.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, MonsterData.weight, 0.0f) * Time.deltaTime);
        }
    }


    public void ApplyGravity()
    {
        controller.Move(new Vector3(0.0f, MonsterData.weight, 0.0f) * Time.deltaTime);
    }


    #region 공격

    private float canAttackTime = 0f;
    private float motionEndTime = 0f;
    private bool isInMotion = false;

    public void CheckAttack()
    {
        if (Time.time >= motionEndTime && isInMotion)
        {
            EndAttackMotion();
            return;
        }

        if (Time.time >= canAttackTime && !isInMotion)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        currentSkillIndex = GetAttackActionIndex();

        StartAttack(currentSkillIndex, currentSkillIndex != -1 ? MonsterData.SkillDatas[currentSkillIndex].skillCoolTime : MonsterData.SkillDatas[0].skillCoolTime);
    }

    private void StartAttack(int skillIndex, float coolTime)
    {
        isInMotion = true;

        animator.SetInteger(AssignAnimationIDs.AnimIDAttackType, skillIndex);
        animator.SetTrigger(AssignAnimationIDs.AnimIDAttackTrigger);

        LookAtTarget((targetTransform.position - transform.position).normalized);

        motionEndTime = Time.time + GetAnimationDuration(skillIndex);
        canAttackTime = Time.time + MonsterData.SkillDatas[0].skillCoolTime;

        canUseSkillTimes[skillIndex] = Time.time + coolTime;
    }

    private void EndAttackMotion()
    {
        isInMotion = false;
    }

    private float GetAnimationDuration(int skillIndex)
    {
        return MonsterData.SkillDatas[skillIndex].motionDelay;
    }


    //현재 사용 가능한 스킬반환
    private int GetAttackActionIndex()
    {
        // 현재 대상이 없을경우 반환
        if (!targetTransform)
            return -1;

        var targetDistance = (targetTransform.position - transform.position).sqrMagnitude;

        for (var i = 1; i < canUseSkillTimes.Length; i++)
        {
            // 스킬 범위 내에 유저가 있고 스킬 쿨타임이 아닐 때
            if (targetDistance <= MonsterData.SkillDatas[i].attackSqrRadius && Time.time > canUseSkillTimes[i])
            {
                return i;
            }
        }

        return (targetDistance > 1) ? -1 : 0;
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


    private Vector3[] moveData = new Vector3[50];

    protected virtual void OnInvokeSkill(AnimationEvent animationEvent)
    {
        if (MonsterData.SkillDatas[currentSkillIndex].isMove)
        {
            MonsterData.SetPath(currentSkillIndex, transform, targetTransform, ref moveData);
            StartCoroutine(SkillMove());
        }
        else
        {
            MonsterData.InvokeSkill(currentSkillIndex, transform, targetTransform, stat.CurrentAttackPower);
        }
    }

    public IEnumerator SkillMove()
    {
        float duration = 2f;
        float elapsedTime = 0f;
        int curveIndex = 0;

        while (elapsedTime < duration && curveIndex < moveData.Length - 1)
        {
            float deltaTime = Time.deltaTime;
            elapsedTime += deltaTime;

            Vector3 direction = (moveData[curveIndex + 1] - controller.transform.position).normalized;
            float distance = Vector3.Distance(controller.transform.position, moveData[curveIndex + 1]);

            controller.Move(direction * (distance / (duration / moveData.Length) * deltaTime));

            if (distance < 0.1f)
            {
                curveIndex++;
            }

            yield return null;
        }

        MonsterData.InvokeSkill(currentSkillIndex, transform, targetTransform, stat.CurrentAttackPower);
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