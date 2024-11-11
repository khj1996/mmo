using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public enum CreatureState
    {
        Idle = 0,
        Move = 1,
        Jump = 2,
        Crouch = 3,
        Attack = 4,
        Interactive = 5,
        GetHit = 6,
        Die = 7,
    }
}

public static class LayerData
{
    public static readonly int DefaultLayer = 1 << 0;
    public static readonly int GroundLayer = 1 << 3;
    public static readonly int PlayerLayer = 1 << 18;
    public static readonly int MonsterLayer = 1 << 19;
}


public static class AssignAnimationIDs
{
    public static readonly int AnimIDSpeed = Animator.StringToHash("MoveSpeed");
    public static readonly int AnimIDGrounded = Animator.StringToHash("Grounded");
    public static readonly int AnimIDJump = Animator.StringToHash("Jump");
    public static readonly int AnimIDCrouch = Animator.StringToHash("Crouch");
    public static readonly int AnimIDFreeFall = Animator.StringToHash("FreeFall");
    public static readonly int AnimIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    public static readonly int AnimIDAttackType = Animator.StringToHash("AttackType");
    public static readonly int AnimIDAttackTrigger = Animator.StringToHash("AttackTrigger");
}