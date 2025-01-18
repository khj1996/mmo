using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "NewMonster", menuName = "ScriptableObjects/Monster")]
public class MonsterData : CreatureData
{
    public enum SkillType
    {
        Melee,
        Range,
        Area
    }

    public enum DeafultAttackType
    {
        Melee,
        Range
    }

    [Header("-------------------MonsterData--------------------")]
    public GameObject creatureModel;


    public Vector3 hpBarPos;

    public float sqrDetectionRange;
    public float sqrChaseRange;

    public List<DropItem> dropItems;
    public float exp;

    [Header("-------------------Skills--------------------")]
    public SkillData[] SkillDatas;

    [Serializable]
    public struct SkillData
    {
        public SkillAction action;
        [Tooltip("스킬 사용 과정에서 이동 여부")] public bool isMove;
        public Vector3 attackPos;
        [Tooltip("공격 사용이 가능한 거리")] public float attackSqrRadius;
        [Tooltip("공격시 효과 범위")] public float attackEffectRadius;
        public float motionDelay;
        public float skillCoolTime;

        public void InvokeSkill(Transform caster, Transform target, float power)
        {
            action.InvokeSkill(this, caster, target, power);
        }

        public void SetPath(Transform caster, Transform target, ref Vector3[] curvePointsArr)
        {
            action.SetPath(this, caster, target, ref curvePointsArr);
        }
    }


    [Serializable]
    public struct DropItem
    {
        public ItemData itemData;
        [Range(0, 100)] public int dropRate;
    }


    public void InvokeSkill(int skillIndex, Transform caster, Transform target, float power)
    {
        SkillDatas[skillIndex].InvokeSkill(caster, target, power);
    }

    public void SetPath(int skillIndex, Transform caster, Transform target, ref Vector3[] curvePointsArr)
    {
        SkillDatas[skillIndex].SetPath(caster, target, ref curvePointsArr);
    }


    public class IdleState : State<MonsterController>
    {
        public IdleState(MonsterController owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            _owner.ApplyGravity();
        }
    }

    public class ChaseState : State<MonsterController>
    {
        public ChaseState(MonsterController owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            _owner.animator.SetBool(AssignAnimationIDs.AnimIDMove, true);
        }

        public override void OnExit()
        {
            _owner.animator.SetBool(AssignAnimationIDs.AnimIDMove, false);
        }

        public override void OnUpdate()
        {
            _owner.Move();
        }
    }

    public class AttackState : State<MonsterController>
    {
        public AttackState(MonsterController owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            _owner.CheckAttack();
        }
    }

    public class DeadState : State<MonsterController>
    {
        public DeadState(MonsterController owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            _owner.DropItem();
            _owner.ReturnToPool();
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
        }
    }
}