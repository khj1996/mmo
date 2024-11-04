using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    protected bool _rangedSkill = false;

    protected override void Init()
    {
        base.Init();
    }


    protected override void UpdateAnimation()
    {
        if (!_animator || !_sprite)
            return;

        var dir = CheckDirection(LookDir);
        string animationName = "";
        bool flipX = false;

        switch (State)
        {
            case CreatureState.Idle:
                animationName = dir == MoveDirection.Up ? "IDLE_BACK" :
                    dir == MoveDirection.Down ? "IDLE_FRONT" : "IDLE_RIGHT";
                flipX = dir == MoveDirection.Left;
                break;

            case CreatureState.Moving:
                animationName = dir == MoveDirection.Up ? "WALK_BACK" :
                    dir == MoveDirection.Down ? "WALK_FRONT" : "WALK_RIGHT";
                flipX = dir == MoveDirection.Left;
                break;

            case CreatureState.Skill:
                animationName = dir == MoveDirection.Up ? _rangedSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK" :
                    dir == MoveDirection.Down ? _rangedSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT" :
                    _rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT";
                flipX = dir == MoveDirection.Left;
                break;
        }

        _animator.Play(animationName);
        _sprite.flipX = flipX;
    }


    public override void UseSkill(S_Skill skillPacket)
    {
        LookDir = skillPacket.MoveDir;

        if (skillPacket.Info.SkillId == 1)
        {
            _coSkill = StartCoroutine(nameof(CoStartPunch));
        }
        else if (skillPacket.Info.SkillId == 2)
        {
            _coSkill = StartCoroutine(nameof(CoStartShootArrow));
        }
    }

    protected virtual void CheckUpdatedFlag()
    {
    }

    IEnumerator CoStartPunch()
    {
        _rangedSkill = false;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag();
    }

    IEnumerator CoStartShootArrow()
    {
        _rangedSkill = true;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag();
    }

    public override void OnDamaged()
    {
        Debug.Log("Player HIT !");
    }
}