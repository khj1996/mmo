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

        OnReturnToPoolAction += SetData;
    }

    private void SetData()
    {
        canUseSkillTimes = new float[MonsterData.SkillDatas.Length];

        isDie = false;
        targetTransform = null;

        transform.localPosition = Vector3.zero;
        hpBar.gameObject.transform.position = transform.TransformPoint(MonsterData.hpBarPos);

        stat.ResetStat(creatureData);
        stateMachine.ChangeState(typeof(MonsterData.IdleState));
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

        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.ChaseState>(() => !isInMotion && GetAttackActionIndex() == -2);
        stateMachine.AddTransition<MonsterData.AttackState, MonsterData.IdleState>(() => !isInMotion && !targetTransform);

        stateMachine.AddGlobalTransition<MonsterData.AttackState>(() => GetAttackActionIndex() != -2);
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

    private float _speed;

    public void Move()
    {
        if (isInMotion)
            return;

        Vector3 direction = (targetTransform.position - transform.position).normalized;

        LookAtTarget(direction);

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        _speed = Mathf.Lerp(currentHorizontalSpeed, creatureData.speed, Time.deltaTime * creatureData.acceleration);
        _speed = Mathf.Round(_speed * 1000f) / 300f;

        controller.Move(direction.normalized * (_speed * Time.deltaTime));
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
        }

        if (Time.time >= canAttackTime && !isInMotion)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        int skillIndex = GetAttackActionIndex();

        if (skillIndex != -1)
        {
            StartAttack(skillIndex, MonsterData.SkillDatas[skillIndex].skillCoolTime);
        }
        else
        {
            StartAttack(skillIndex, MonsterData.minAttackSpeed);
        }
    }

    private void StartAttack(int skillIndex, float coolTime)
    {
        isInMotion = true;

        animator.SetInteger(AssignAnimationIDs.AnimIDAttackType, skillIndex);
        animator.SetTrigger(AssignAnimationIDs.AnimIDAttackTrigger);

        LookAtTarget((targetTransform.position - transform.position).normalized);

        motionEndTime = Time.time + GetAnimationDuration(skillIndex);

        canAttackTime = Time.time + MonsterData.minAttackSpeed;

        if (skillIndex != -1)
        {
            canUseSkillTimes[skillIndex] = Time.time + coolTime;
        }
    }

    private void EndAttackMotion()
    {
        isInMotion = false;
    }

    private float GetAnimationDuration(int skillIndex)
    {
        return skillIndex != -1
            ? MonsterData.SkillDatas[skillIndex].motionDelay
            : MonsterData.defaultMotionDelay;
    }


    //현재 사용 가능한 스킬반환
    private int GetAttackActionIndex()
    {
        // 현재 대상이 없을경우 반환
        if (!targetTransform)
            return -2;

        var targetDistance = (targetTransform.position - transform.position).sqrMagnitude;

        for (var i = 0; i < canUseSkillTimes.Length; i++)
        {
            // 스킬 범위 내에 유저가 있고 스킬 쿨타임이 아닐 때
            if (targetDistance <= MonsterData.SkillDatas[i].attackSqrRadius && Time.time > canUseSkillTimes[i])
            {
                return i;
            }
        }

        // 기본 공격 가능 여부 확인
        if (targetDistance <= MonsterData.minSqrAttackRange)
        {
            return -1;
        }

        return -2;
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