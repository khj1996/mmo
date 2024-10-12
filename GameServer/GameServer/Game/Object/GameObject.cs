using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using static GameServer.Game.GameLogic;

namespace GameServer.Game
{
    public class GameObject
    {
        //오브젝트 타입
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;

        //id 반환
        public int Id
        {
            get => Info.ObjectId;
            set => Info.ObjectId = value;
        }

        //게임룸
        public GameRoom? Room { get; set; }

        //오브젝트 정보
        public ObjectInfo Info { get; private set; } = new();


        //총 공격력
        public virtual int TotalAttack => Info.StatInfo.Attack;

        //총 방어력
        public virtual int TotalDefence => 0;

        //속도
        public float Speed
        {
            get => Info.StatInfo.Speed;
            set => Info.StatInfo.Speed = value;
        }

        //체력
        public int Hp
        {
            get => Info.StatInfo.Hp;
            set => Info.StatInfo.Hp = Math.Clamp(value, 0, Info.StatInfo.MaxHp);
        }

        //오브젝트 상태
        public CreatureState State
        {
            get => Info.PosInfo.State;
            set => Info.PosInfo.State = value;
        }

        public Vec2 Pos
        {
            get => Info.PosInfo.Pos;
            set => Info.PosInfo.Pos = value;
        }

        public Vec2 Move
        {
            get => Info.PosInfo.Move;
            set => Info.PosInfo.Move = value;
        }

        public Vec2 LookDir
        {
            get => Info.PosInfo.LookDir;
            set => Info.PosInfo.LookDir = value;
        }

        //위치
        public Vector2Float CellPos => new(Info.PosInfo.Pos.X, Info.PosInfo.Pos.Y);


        //생성자
        public GameObject()
        {
            Info = new ObjectInfo
            {
                PosInfo = new PositionInfo(),
                StatInfo = new StatInfo()
            };
        }

        //업데이트
        public virtual void Update()
        {
        }


        public void UpdatePosition()
        {
            // 속도 보정 후 프레임당 이동 거리 계산
            float moveX = Speed * Move.X * Room.tickInterval / 1000;
            float moveY = Speed * Move.Y * Room.tickInterval / 1000;
            

            // 새로운 위치 계산
            Pos.X += moveX;
            Pos.Y += moveY;
        }

        public virtual void UpdatePositionData(C_Move movePacket)
        {
            //위치 오류 확인 필요
            Move = movePacket.PosInfo.Move;
            LookDir = movePacket.PosInfo.LookDir;
            State = movePacket.PosInfo.State;
        }


        //피격
        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            if (Room == null)
                return;

            damage = Math.Max(damage - TotalDefence, 0);
            Info.StatInfo.Hp = Math.Max(Info.StatInfo.Hp - damage, 0);

            var changePacket = new S_ChangeHp();
            changePacket.ObjectId = Id;
            changePacket.Hp = Info.StatInfo.Hp;
            Room.Broadcast(CellPos, changePacket);

            if (Info.StatInfo.Hp <= 0)
            {
                OnDead(attacker);
            }
        }

        //사망
        //사망시 해당 오브젝트를 퇴장 후 재입장 방식
        //TODO : 재입장이 아닌 방식으로 수정 도전
        public virtual void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;

            var diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(CellPos, diePacket);

            var room = Room;
            room.LeaveGame(Id);

            Info.StatInfo.Hp = Info.StatInfo.MaxHp;
            Info.PosInfo.State = CreatureState.Idle;
            Info.PosInfo.LookDir = new Vec2();

            room.EnterGame(this);
        }

        //소유주
        public virtual GameObject GetOwner()
        {
            return this;
        }
    }
}