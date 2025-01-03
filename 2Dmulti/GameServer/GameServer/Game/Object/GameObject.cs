﻿using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using GameServer.Data;
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


        public ObjectSize size = new()
        {
            Top = 0.33f,
            Bottom = 0.77f,
            Left = 0.35f,
            Right = 0.35f,
        };

        //오브젝트 상태
        public CreatureState State
        {
            get => Info.PosInfo.State;
            set
            {
                if (State == value)
                    return;
                //상태진입전
                Info.PosInfo.State = value;
                //상태진입후
                switch (value)
                {
                    case CreatureState.Moving:
                        BroadcastMove();
                        break;
                    case CreatureState.Idle:
                        BroadcastMove();
                        break;
                }
            }
        }

        public Vec2 Pos
        {
            get => Info.PosInfo.Pos;
            set => Info.PosInfo.Pos = value;
        }

        public Vec2 Move
        {
            get => Info.PosInfo.Move;
            set
            {
                if(value == null)
                    return;
                Info.PosInfo.Move = value;
            }
        }

        public Vec2 LookDir
        {
            get => Info.PosInfo.LookDir;
            set => Info.PosInfo.LookDir = value;
        }

        //위치
        public Vector2Float _Pos => new(Info.PosInfo.Pos.X, Info.PosInfo.Pos.Y);


        //생성자
        public GameObject()
        {
            Info = new ObjectInfo
            {
                PosInfo = new PositionInfo(),
                StatInfo = new StatInfo(),
                
            };
        }

        //업데이트
        public virtual void Update()
        {
        }

        public virtual void UpdateLevel(StatInfo statInfo)
        {

        }


        public virtual bool UpdatePosition()
        {
            if (Room == null) return false;

            var speedScale = Speed * Room.SpeedScale;

            var result = Room.Map.ApplyMove(this, new Vector2Float(Pos.X + speedScale * Move.X, Pos.Y + speedScale * Move.Y), Move);

            Console.WriteLine(speedScale * Move.X);
            BroadcastMove();

            return result;
        }

        protected virtual void BroadcastMove()
        {
            S_Move resMovePacket = new S_Move
            {
                ObjectId = Id,
                PosInfo = Info.PosInfo,
            };

            Room.Broadcast(_Pos, resMovePacket);
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
            Room.Broadcast(_Pos, changePacket);

            if (Info.StatInfo.Hp <= 0)
            {
                OnDead(attacker);
            }
        }

        public virtual void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;

            var diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(_Pos, diePacket);

            var room = Room;
            room.LeaveGame(Id);

            Info.StatInfo.Hp = Info.StatInfo.MaxHp;
            Info.PosInfo.State = CreatureState.Idle;
            Info.PosInfo.LookDir = new Vec2();

            room.EnterGame(this,false);
        }

        //소유주
        public virtual GameObject GetOwner()
        {
            return this;
        }
    }
}