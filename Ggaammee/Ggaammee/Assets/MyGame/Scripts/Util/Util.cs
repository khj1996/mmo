using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }


    public static void BindEvent(this GameObject go, Action<PointerEventData> action, UIEvent type = UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }

    public static string FormatNumber(long num)
    {
        var suffixes = new[] { "K", "M", "B", "T" };
        var divisors = new[] { 1_000L, 1_000_000L, 1_000_000_000L, 1_000_000_000_000L };

        for (int i = divisors.Length - 1; i >= 0; i--)
        {
            if (num >= divisors[i])
            {
                double shortened = (double)num / divisors[i];
                return shortened.ToString(shortened >= 10 ? "0" : "0.#") + suffixes[i];
            }
        }

        return num.ToString("#,0");
    }
}

public static class StaticValues
{
    public static readonly int InventorySize = 88;
}

public enum UIEvent
{
    Click,
    Drag,
}

public enum ChangeType
{
    Add,
    Cost,
    New
}

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


public enum StatType
{
    Hp,
    Atk,
    Defense
}

public enum EquipType
{
    Weapon,
    Armor
}

public static class LayerData
{
    public static readonly int DefaultLayer = 1 << 0;
    public static readonly int GroundLayer = 1 << 3;
    public static readonly int NpcLayer = 1 << 17;
    public static readonly int PlayerLayer = 1 << 18;
    public static readonly int MonsterLayer = 1 << 19;
    public static readonly int ItemSlotLayer = 1 << 20;
}

public static class PopUpName
{
    public static readonly string InventoryUI = "InventoryUI";
    public static readonly string ShopUI = "ShopUI";
    public static readonly string StatUI = "StatUI";
}

public enum QuestType
{
    KillMonster,
    CollectItem,
    ReachDestination
}


public enum QuestState
{
    NotStarted,
    InProgress,
    Completed
}

public static class AssignAnimationIDs
{
    public static readonly int AnimIDSpeed = Animator.StringToHash("MoveSpeed");
    public static readonly int AnimIDMove = Animator.StringToHash("Move");
    public static readonly int AnimIDGrounded = Animator.StringToHash("Grounded");
    public static readonly int AnimIDJump = Animator.StringToHash("Jump");
    public static readonly int AnimIDCrouch = Animator.StringToHash("Crouch");
    public static readonly int AnimIDFreeFall = Animator.StringToHash("FreeFall");
    public static readonly int AnimIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    public static readonly int AnimIDAttackType = Animator.StringToHash("AttackType");
    public static readonly int AnimIDAttackTrigger = Animator.StringToHash("AttackTrigger");
    public static readonly int AnimIDLadder = Animator.StringToHash("Ladder");
    public static readonly int AnimIDLadderUpStart = Animator.StringToHash("LadderUpStart");
    public static readonly int AnimIDLadderDownStart = Animator.StringToHash("LadderDownStart");
    public static readonly int AnimIDLadderDownPlay = Animator.StringToHash("LadderDownPlay");
    public static readonly int AnimIDLadderUpPlay = Animator.StringToHash("LadderUpPlay");
    public static readonly int AnimIDLadderDownEnd = Animator.StringToHash("LadderDownEnd");
    public static readonly int AnimIDLadderUpEnd = Animator.StringToHash("LadderUpEnd");
}