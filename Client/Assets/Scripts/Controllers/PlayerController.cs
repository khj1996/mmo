﻿using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (_animator == null || _sprite == null)
            return;


        if (State == CreatureState.Idle)
        {
            if (Dir.y > 0)
            {
                _animator.Play("IDLE_BACK");
                _sprite.flipX = false;
            }
            else if (Dir.y < 0)
            {
                _animator.Play("IDLE_FRONT");
                _sprite.flipX = false;
            }
            else if (Dir.x < 0)
            {
                _animator.Play("IDLE_RIGHT");
                _sprite.flipX = true;
            }
            else if (Dir.x > 0)
            {
                _animator.Play("IDLE_RIGHT");
                _sprite.flipX = false;
            }
        }
        else if (State == CreatureState.Moving)
        {
            if (Dir.y > 0)
            {
                _animator.Play("WALK_BACK");
                _sprite.flipX = false;
            }
            else if (Dir.y < 0)
            {
                _animator.Play("WALK_FRONT");
                _sprite.flipX = false;
            }
            else if (Dir.x < 0)
            {
                _animator.Play("WALK_RIGHT");
                _sprite.flipX = true;
            }
            else if (Dir.x > 0)
            {
                _animator.Play("WALK_RIGHT");
                _sprite.flipX = false;
            }
        }
        else if (State == CreatureState.Skill)
        {
            if (Dir.y > 0)
            {
                _animator.Play(_rangedSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
                _sprite.flipX = false;
            }
            else if (Dir.y < 0)
            {
                _animator.Play(_rangedSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                _sprite.flipX = false;
            }
            else if (Dir.x < 0)
            {
                _animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                _sprite.flipX = true;
            }
            else if (Dir.x > 0)
            {
                _animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                _sprite.flipX = false;
            }
        }
    }

    protected override void UpdateController()
    {
        base.UpdateController();
    }

    public override void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            _coSkill = StartCoroutine("CoStartPunch");
        }
        else if (skillId == 2)
        {
            _coSkill = StartCoroutine("CoStartShootArrow");
        }
    }

    protected virtual void CheckUpdatedFlag()
    {
    }

    IEnumerator CoStartPunch()
    {
        // 대기 시간
        _rangedSkill = false;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag();
    }

    IEnumerator CoStartShootArrow()
    {
        // 대기 시간
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