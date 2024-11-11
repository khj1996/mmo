using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewPlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : CreatureData
{
    public class IdleAndMoveState : State<PlayerController>
    {
        public IdleAndMoveState(PlayerController owner) : base(owner)
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
            _owner.JumpAndGravity();
            _owner.GroundedCheck();
            _owner.CheckAttack();
            _owner.Move();
        }
    }

    public class CrouchState : State<PlayerController>
    {
        public CrouchState(PlayerController owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            _owner._animator.SetBool(AssignAnimationIDs.AnimIDCrouch, true);
        }

        public override void OnExit()
        {
            _owner._animator.SetBool(AssignAnimationIDs.AnimIDCrouch, false);
        }

        public override void OnUpdate()
        {
            _owner.CheckAttack();
            _owner.GroundedCheck();
            _owner.Move();
        }
    }

    public class JumpState : State<PlayerController>
    {
        public JumpState(PlayerController owner) : base(owner)
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
            _owner.JumpAndGravity();
            _owner.GroundedCheck();
            _owner.Move();
        }
    }

    public class GetHitState : State<PlayerController>
    {
        public GetHitState(PlayerController owner) : base(owner)
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