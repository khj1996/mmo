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
        if (_animator == null || _sprite == null)
            return;


        var dir = CheckDirection(LookDir);

        Debug.Log(1);
        switch (State)
        {
            case CreatureState.Idle:
                switch (dir)
                {
                    case MoveDirection.Up:
                        _animator.Play("IDLE_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Down:
                        _animator.Play("IDLE_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Right:
                        _animator.Play("IDLE_RIGHT");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Left:
                        _animator.Play("IDLE_RIGHT");
                        _sprite.flipX = true;
                        break;
                }

                break;
            case CreatureState.Moving:
                switch (dir)
                {
                    case MoveDirection.Up:
                        _animator.Play("WALK_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Down:
                        _animator.Play("WALK_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Right:
                        _animator.Play("WALK_RIGHT");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Left:
                        _animator.Play("WALK_RIGHT");
                        _sprite.flipX = true;
                        break;
                }

                break;
            case CreatureState.Skill:
                switch (dir)
                {
                    case MoveDirection.Up:
                        _animator.Play(_rangedSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Down:
                        _animator.Play(_rangedSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Right:
                        _animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                        _sprite.flipX = false;
                        break;
                    case MoveDirection.Left:
                        _animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                        _sprite.flipX = true;
                        break;
                }

                break;
        }
    }

    public override void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            _coSkill = StartCoroutine(nameof(CoStartPunch));
        }
        else if (skillId == 2)
        {
            _coSkill = StartCoroutine(nameof(CoStartShootArrow));
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