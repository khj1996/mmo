using Google.Protobuf.Protocol;
using GameServer.Data;
using GameServer.DB;
using System;
using System.Collections.Generic;

namespace GameServer.Game
{
    public class Monster : GameObject
    {
        public int TemplateId { get; private set; }

        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }

        public void Init(int templateId)
        {
            TemplateId = templateId;

            MonsterData monsterData = null;
            DataManager.MonsterDict.TryGetValue(TemplateId, out monsterData);
            Info.StatInfo.MergeFrom(monsterData.stat);
            Info.StatInfo.Hp = monsterData.stat.MaxHp;
            State = CreatureState.Idle;
        }

        IJob _job;

        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Skill:
                    UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;
            }

            if (Move == null)
                Console.WriteLine("test");

            // 5프레임 (0.2초마다 한번씩 Update)
            if (Room != null)
                _job = Room.PushAfter(200, Update);
        }

        Player _target;
        int searchDist = 10;
        int chaseDist = 20;
        long nextSearchTick = 0;

        protected virtual void UpdateIdle()
        {
            if (nextSearchTick > Environment.TickCount64)
                return;
            nextSearchTick = Environment.TickCount64 + 1000;

            Player target = Room.FindClosestPlayer(_Pos, searchDist);

            if (target == null)
                return;

            _target = target;
            State = CreatureState.Moving;
        }

        //공격 거리
        int _skillRange = 1;

        //다음 이동 틱
        long nextMoveTick = 0;
        private Vec2 lastTargetPos;
        private List<(int tileX, int tileY)> _path;

        //이동
        protected virtual void UpdateMoving()
        {
            if (nextMoveTick > Environment.TickCount64)
                return;

            int moveTick = (int)(1000 / Speed);
            nextMoveTick = Environment.TickCount64 + moveTick;

            if (_target == null || _target.Room != Room)
            {
                ResetTarget();
                return;
            }

            Vector2Float dir = _target._Pos - _Pos;
            float distSqr = dir.SqrMagnitude;

            if (distSqr > chaseDist * chaseDist)
            {
                ResetTarget();
                return;
            }

            if (distSqr <= _skillRange * _skillRange && (dir.x == 0 || dir.y == 0))
            {
                _coolTick = 0;
                State = CreatureState.Skill;
                return;
            }

            if (_path == null || _path.Count < 2 || !_target.Pos.Equals(lastTargetPos))
            {
                _path = Room.Map.FindPath(_Pos, _target._Pos);
                lastTargetPos = _target.Pos;
            }

            if (_path != null && _path.Count >= 2)
            {
                var normalizedDir = dir.Normalized;

                Move = new Vec2()
                {
                    X = normalizedDir.x,
                    Y = normalizedDir.y
                };


                Room.Map.ApplyMove(this, new Vector2Float()
                {
                    x = _path[1].tileX,
                    y = _path[1].tileY
                });
                _path.RemoveAt(0);
                BroadcastMove();
            }
        }

        private void ResetTarget()
        {
            _target = null;
            State = CreatureState.Idle;
            BroadcastMove();
        }

        protected override void BroadcastMove()
        {
            S_Move resMovePacket = new S_Move
            {
                ObjectId = Id,
                PosInfo = Info.PosInfo,
            };

            Room.Broadcast(_Pos, resMovePacket);
        }


        long _coolTick = 0;

        //스킬
        protected virtual void UpdateSkill()
        {
            if (_coolTick == 0)
            {
                // 유효한 타겟인지
                if (_target == null || _target.Room != Room)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }

                // 스킬이 아직 사용 가능한지
                Vector2Float dir = (_target._Pos - _Pos);
                float dist = dir.Magnitude;
                bool canUseSkill = (dist <= _skillRange && (dir.x == 0 || dir.y == 0));
                if (canUseSkill == false)
                {
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }

                Skill skillData = null;
                DataManager.SkillDict.TryGetValue(1, out skillData);

                // 데미지 판정
                _target.OnDamaged(this, skillData.damage + TotalAttack);

                // 스킬 사용 Broadcast
                S_Skill skill = new S_Skill() { Info = new SkillInfo() };
                skill.ObjectId = Id;
                skill.Info.SkillId = skillData.id;
                Room.Broadcast(_Pos, skill);

                // 스킬 쿨타임 적용
                int coolTick = (int)(1000 * skillData.cooldown);
                _coolTick = Environment.TickCount64 + coolTick;
            }

            if (_coolTick > Environment.TickCount64)
                return;

            _coolTick = 0;
        }

        protected virtual void UpdateDead()
        {
        }

        public override void OnDead(GameObject attacker)
        {
            if (_job != null)
            {
                _job.Cancel = true;
                _job = null;
            }

            base.OnDead(attacker);

            GameObject owner = attacker.GetOwner();
            if (owner.ObjectType == GameObjectType.Player)
            {
                Player player = (Player)owner;
                RewardData rewardData = GetRandomReward();
                if (rewardData != null)
                {
                    DbTransaction.RewardPlayer(player, rewardData, Room);
                }

                player.Exp += 5;
            }
        }

        RewardData GetRandomReward()
        {
            MonsterData monsterData = null;
            DataManager.MonsterDict.TryGetValue(TemplateId, out monsterData);

            int rand = new Random().Next(0, 101);

            int sum = 0;
            foreach (RewardData rewardData in monsterData.rewards)
            {
                sum += rewardData.probability;

                if (rand <= sum)
                {
                    return rewardData;
                }
            }

            return null;
        }
    }
}