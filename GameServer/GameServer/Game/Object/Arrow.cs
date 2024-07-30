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
            int tick = (int)(1000 / GameLogic.Instance.updateFrame);
            //다음 이동
            Room.PushAfter(tick, Update);

            List<Player> players = Room.GetAdjacentPlayers(CellPos, 1);

            foreach (var player in players)
            {
                if(player.Id == Owner.Id)
                    continue;
                
                var dis = (CellPos - player.CellPos).magnitude;

                if (dis < 0.3f)
                {
                    if (player != null)
                    {
                        player.OnDamaged(this, Data.damage + Owner.TotalAttack);
                    }

                    // 소멸
                    Room.Push(Room.LeaveGame, Id);
                    return;
                }
            }

            var tickSpeed = Speed * tick / 1000;

            Info.PosInfo.PosX += moveDir.X * tickSpeed;
            Info.PosInfo.PosY += moveDir.Y * tickSpeed;
            Info.PosInfo.PosZ += moveDir.Z * tickSpeed;

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