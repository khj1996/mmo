using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameServer.Game
{
    public class GameObject
    {
        //오브젝트 타입
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;

        //id 반환
        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }

        //게임룸
        public GameRoom Room { get; set; }

        //오브젝트 정보
        public ObjectInfo Info { get; set; } = new();

        //위치 정보
        public PositionInfo PosInfo { get; private set; } = new();

        //이동 방향
        public moveDir moveDir { get; private set; } = new();

        //능력치 정보
        public StatInfo Stat { get; private set; } = new();

        //총 공격력
        public virtual int TotalAttack
        {
            get { return Stat.Attack; }
        }

        //총 방어력
        public virtual int TotalDefence
        {
            get { return 0; }
        }

        //속도
        public float Speed
        {
            get { return Stat.Speed; }
            set { Stat.Speed = value; }
        }

        //체력
        public int Hp
        {
            get { return Stat.Hp; }
            set { Stat.Hp = Math.Clamp(value, 0, Stat.MaxHp); }
        }

        //보는 방향
        public moveDir Dir
        {
            get { return moveDir; }
            set { moveDir = value; }
        }

        //오브젝트 상태
        public CreatureState State
        {
            get { return PosInfo.State; }
            set { PosInfo.State = value; }
        }

        //생성자
        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = Stat;
        }

        //업데이트
        public virtual void Update()
        {
        }


        public virtual void Move()
        {
            //TODO : 이동 가능한 위치인지 추가 필요
            PosInfo.PosX += Dir.X * Speed * (1.0f / GameLogic.Instance.updateFrame);
            PosInfo.PosY += Dir.Y * Speed * (1.0f / GameLogic.Instance.updateFrame);
            Console.WriteLine(PosInfo.PosX + "  " + PosInfo.PosY);
        }

        public virtual void UpdateMoveDir(C_Move movePacket)
        {
            Dir = movePacket.MoveDir;
            State = movePacket.PosInfo.State;
        }

        //위치
        public Vector2Float CellPos
        {
            get { return new Vector2Float(PosInfo.PosX, PosInfo.PosY); }

            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
            }
        }

        //피격
        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            if (Room == null)
                return;

            damage = Math.Max(damage - TotalDefence, 0);
            Stat.Hp = Math.Max(Stat.Hp - damage, 0);

            S_ChangeHp changePacket = new S_ChangeHp();
            changePacket.ObjectId = Id;
            changePacket.Hp = Stat.Hp;
            Room.Broadcast(CellPos, changePacket);

            if (Stat.Hp <= 0)
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

            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(CellPos, diePacket);

            GameRoom room = Room;
            room.LeaveGame(Id);

            Stat.Hp = Stat.MaxHp;
            PosInfo.State = CreatureState.Idle;
            Dir = new moveDir()
            {
                X = 0,
                Y = 0,
                Z = 0
            };

            room.EnterGame(this, randomPos: true);
        }

        //소유주
        public virtual GameObject GetOwner()
        {
            return this;
        }
    }
}