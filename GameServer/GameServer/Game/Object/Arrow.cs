using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
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
            int tick = (int)(1000 / Data.projectile.speed);
            //다음 이동
            Room.PushAfter(tick, Update);

            //보는 방향 획득
            Vector2Int destPos = GetFrontCellPos();
            //방향 이동 적용
            //이동이 가능한 상황
            if (Room.Map.ApplyMove(this, destPos, collision: false))
            {
                S_Move movePacket = new S_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                Room.Broadcast(CellPos, movePacket);
            }
            else
            {
                //이동 방향에 물체가 있는 경우 공격
                GameObject target = Room.Map.Find(destPos);
                if (target != null)
                {
                    target.OnDamaged(this, Data.damage + Owner.TotalAttack);
                }

                // 소멸
                Room.Push(Room.LeaveGame, Id);
            }
        }

        public override GameObject GetOwner()
        {
            return Owner;
        }
    }
}