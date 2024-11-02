using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coSkill;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    public override void OnDamaged()
    {
        //Managers.Object.Remove(Id);
        //Managers.Resource.Destroy(gameObject);
    }

    public override void UseSkill(S_Skill skillPacket)
    {
        LookDir = skillPacket.MoveDir;

        if (skillPacket.Info.SkillId == 1)
        {
            State = CreatureState.Skill;
        }
    }

    public override void UpdatePosition(S_Move movePacket)
    {
        Pos = movePacket.PosInfo.Pos;


        if (movePacket.PosInfo.Move != null)
        {
            Move = movePacket.PosInfo.Move;

            if (!(Move.X == 0 && Move.Y == 0))
                LookDir = movePacket.PosInfo.Move;
        }


        if (movePacket.PosInfo.State != CreatureState.Idle)
        {
            State = movePacket.PosInfo.State;
        }
        else
        {
            var destPos = new Vector3(Pos.X, Pos.Y, transform.position.z);
            var distance = (destPos - transform.position).magnitude;
            if (distance < Mathf.Epsilon && Move.X == 0 && Move.Y == 0)
            {
                State = CreatureState.Idle;
            }
        }

        UpdateAnimation();
    }
}