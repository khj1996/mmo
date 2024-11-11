using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewPlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : CreatureData
{
    public class IdleAndMoveState : State<PlayerControllerFSM>
    {
        public IdleAndMoveState(PlayerControllerFSM owner) : base(owner)
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

    public class CrouchState : State<PlayerControllerFSM>
    {
        public CrouchState(PlayerControllerFSM owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            _owner._animator.SetBool(AssignAnimationIDs.AnimIDCrouch, true);
        }

        public override void OnExit()
        {
            _owner._animator.SetBool(AssignAnimationIDs.AnimIDCrouch, true);
        }

        public override void OnUpdate()
        {
            _owner.CheckAttack();
            _owner.GroundedCheck();
            _owner.Move();
        }
    }

    public class JumpState : State<PlayerControllerFSM>
    {
        public JumpState(PlayerControllerFSM owner) : base(owner)
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

    public class GetHitState : State<PlayerControllerFSM>
    {
        public GetHitState(PlayerControllerFSM owner) : base(owner)
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