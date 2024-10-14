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

            //이동 틱
            int tick = (int)Room.TickInterval;
            //다음 이동
            Room.PushAfter(tick, Update);

            var target = Room.FindPlayer(x => (CellPos - x.CellPos).Magnitude < 1f);

            if (target != null && target.Id != Owner.Id)
            {
                target.OnDamaged(Owner, Data.damage + Owner.TotalAttack);


                // 소멸
                Room.Push(Room.LeaveGame, Id);
                return;
            }

            var tickSpeed = Speed * tick / 1000;

            var destPos = new Vector2Float(Pos.X + Move.X * tickSpeed,
                Pos.Y + Move.Y * tickSpeed);

            if (!Room.Map.ApplyMove(this, destPos))
            {
                Room.Push(Room.LeaveGame, Id);
                return;
            }


            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = Info.PosInfo;
            Room.Broadcast(CellPos, movePacket);
        }

        public override GameObject GetOwner()
        {
            return Owner;
        }
    }
}