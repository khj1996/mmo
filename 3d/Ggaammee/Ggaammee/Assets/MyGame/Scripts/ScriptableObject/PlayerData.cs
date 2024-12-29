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
            _owner.Interact();
            _owner.ChangeAttackType();
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
            _owner.animator.SetBool(AssignAnimationIDs.AnimIDCrouch, true);
        }

        public override void OnExit()
        {
            _owner.animator.SetBool(AssignAnimationIDs.AnimIDCrouch, false);
        }

        public override void OnUpdate()
        {
            _owner.Interact();
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
            _owner.Interact();
            _owner.JumpAndGravity();
            _owner.GroundedCheck();
            _owner.Move();
        }
    }

    public class LadderState : State<PlayerController>
    {
        public LadderState(PlayerController owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            _owner.CharacterToLadder();
            _owner.animator.SetLayerWeight(1, 0);
            _owner.DisableNav();
        }

        public override void OnExit()
        {
            _owner.animator.SetBool(AssignAnimationIDs.AnimIDLadderUpPlay, false);
            _owner.animator.SetBool(AssignAnimationIDs.AnimIDLadderDownPlay, false);
            _owner.animator.SetLayerWeight(1, 1);
            _owner.EnableNavMesh(_owner.transform.position);
        }

        public override void OnUpdate()
        {
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