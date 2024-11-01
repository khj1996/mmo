using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Game
{
    public class Arrow : Projectile
    {
        //발사체를 발사한 사람
        public GameObject Owner { get; set; }

        //업데이트
        public override void Update()
        {
            //예외처리
            if (Data == null || Data.projectile == null || Owner == null || Room == null)
                return;

            if (isDestory)
            {
                Room.Push(Room.LeaveGame, Id);
                return;
            }


            var target = Room.FindPlayer(x => (CellPos - x.CellPos).Magnitude < 1f);

            if (target != null && target.Id != Owner.Id)
            {
                target.OnDamaged(Owner, Data.damage + Owner.TotalAttack);


                // 소멸
                Room.Push(Room.LeaveGame, Id);
                return;
            }

            if (!UpdatePosition())
            {
                isDestory = true;
            }

            Room.PushAfter(Room.TickInterval, Update);
        }

        public override GameObject GetOwner()
        {
            return Owner;
        }
    }
}