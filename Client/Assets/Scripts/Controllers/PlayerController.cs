using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        Debug.Log(PosInfo.State);
        if (_animator == null || _sprite == null)
            return;


        if (State == CreatureState.Idle)
        {
            if (PosInfo.MoveDir == MoveDir.Up)
            {
                _animator.Play("IDLE_BACK");
                _sprite.flipX = false;
            }
            else if (PosInfo.MoveDir == MoveDir.Down)
            {
                _animator.Play("IDLE_FRONT");
                _sprite.flipX = false;
            }
            else if (PosInfo.MoveDir == MoveDir.Right)
            {
                _animator.Play("IDLE_RIGHT");
                _sprite.flipX = true;
            }
            else if (PosInfo.MoveDir == MoveDir.Left)
            {
                _animator.Play("IDLE_RIGHT");
                _sprite.flipX = false;
            }
        }
        else if (State == CreatureState.Moving)
        {
            if (PosInfo.MoveDir == MoveDir.Up)
            {
                _animator.Play("WALK_BACK");
                _sprite.flipX = false;
            }
            else if (PosInfo.MoveDir == MoveDir.Down)
            {
                _animator.Play("WALK_FRONT");
                _sprite.flipX = false;
            }
            else if (PosInfo.MoveDir == MoveDir.Right)
            {
                _animator.Play("WALK_RIGHT");
                _sprite.flipX = true;
            }
            else if (PosInfo.MoveDir == MoveDir.Left)
            {
                _animator.Play("WALK_RIGHT");
                _sprite.flipX = false;
            }
        }
        else if (State == CreatureState.Skill)
        {
            if (PosInfo.MoveDir == MoveDir.Up)
            {
                _animator.Play(_rangedSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
                _sprite.flipX = false;
            }
            else if (PosInfo.MoveDir == MoveDir.Down)
            {
                _animator.Play(_rangedSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                _sprite.flipX = false;
            }
            else if (PosInfo.MoveDir == MoveDir.Right)
            {
                _animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                _sprite.flipX = true;
            }
            else if (PosInfo.MoveDir == MoveDir.Left)
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

    protected override void UpdateMoving()
    {
        /*if (Mathf.Approximately(PosInfo.PosX, transform.position.x) &&
            Mathf.Approximately(PosInfo.PosY, transform.position.y) &&
            Mathf.Approximately(PosInfo.PosZ, transform.position.z))
        {
            State = CreatureState.Idle;
            return;
        }*/

        transform.DOMove(new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ), 0.2f).SetEase(Ease.Linear);
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