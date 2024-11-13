using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewMonster", menuName = "ScriptableObjects/Monster")]
public class MonsterData : CreatureData
{
    [Header("-------------------MonsterData--------------------")]
    public GameObject creatureModel;


    public float sqrDetectionRange;

    public List<DropItem> dropItems;
    public float exp;


    [Serializable]
    public struct DropItem
    {
        public int Id;
        public float DropRate;
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
        }
    }
}