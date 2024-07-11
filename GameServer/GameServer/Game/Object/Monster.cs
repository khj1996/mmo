using Google.Protobuf.Protocol;
using GameServer.Data;
using GameServer.DB;
using System;
using System.Collections.Generic;

namespace GameServer.Game
{
    public class Monster : GameObject
    {
        //오브젝트 id
        public int TemplateId { get; private set; }

        //생성자
        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }

        //초기화
        public void Init(int templateId)
        {
            TemplateId = templateId;

            MonsterData monsterData = null;
            DataManager.MonsterDict.TryGetValue(TemplateId, out monsterData);
            Info.StatInfo.MergeFrom(monsterData.stat);
            Info.StatInfo.Hp = monsterData.stat.MaxHp;
            State = CreatureState.Idle;
        }

        // FSM (Finite State Machine)
        IJob _job;

        //입장할때 실행후 자체 재호출
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

            // 5프레임 (0.2초마다 한번씩 Update)
            if (Room != null)
                _job = Room.PushAfter(40, Update);
        }

        //추격 대상
        Player _target;
        //탐색 거리
        int _searchCellDist = 10;
        //추격 거리
        int _chaseCellDist = 20;

        //다음 탐색 틱
        long _nextSearchTick = 0;

        //일반 상태일때 탐색틱마다 유저 탐색
        protected virtual void UpdateIdle()
        {
            if (_nextSearchTick > Environment.TickCount64)
                return;
            _nextSearchTick = Environment.TickCount64 + 1000;

            Player target = Room.FindClosestPlayer(CellPos, _searchCellDist);

            if (target == null)
                return;

            _target = target;
            State = CreatureState.Moving;
        }

        //공격 거리
        int _skillRange = 1;
        //다음 이동 틱
        long _nextMoveTick = 0;

        //이동
        protected virtual void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64)
                return;
            int moveTick = (int)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + moveTick;

            //대상이 없을시
            if (_target == null || _target.Room != Room)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            Vector2Float dir = _target.CellPos - CellPos;
            float dist = dir.cellDistFromZero;
            //대상이 멀어지면 추적 포기
            if (dist == 0 || dist > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            //경로 탐색
            List<Vector2Float> path = Room.Map.FindPath(CellPos, _target.CellPos, checkObjects: true);
            //이미 근접했거나 먼경우
            if (path.Count < 2 || path.Count > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            // 스킬로 넘어갈지 체크
            if (dist <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                _coolTick = 0;
                State = CreatureState.Skill;
                return;
            }

            // 이동
            //Dir = GetDirFromVec(path[1] - CellPos);
            Room.Map.ApplyMove(this, path[1]);
            BroadcastMove();
        }

        //이동 알림
        void BroadcastMove()
        {
            // 다른 플레이어한테도 알려준다
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = Info.PosInfo;
            Room.Broadcast(CellPos, movePacket);
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
                Vector2Float dir = (_target.CellPos - CellPos);
                float dist = dir.cellDistFromZero;
                bool canUseSkill = (dist <= _skillRange && (dir.x == 0 || dir.y == 0));
                if (canUseSkill == false)
                {
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }

                // 타게팅 방향 주시
                //TODO : 공격및 이동 가능하게
                /*MoveDir lookDir = GetDirFromVec(dir);
                if (Dir != lookDir)
                {
                    Dir = lookDir;
                    BroadcastMove();
                }*/

                Skill skillData = null;
                DataManager.SkillDict.TryGetValue(1, out skillData);

                // 데미지 판정
                _target.OnDamaged(this, skillData.damage + TotalAttack);

                // 스킬 사용 Broadcast
                S_Skill skill = new S_Skill() { Info = new SkillInfo() };
                skill.ObjectId = Id;
                skill.Info.SkillId = skillData.id;
                Room.Broadcast(CellPos, skill);

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
                RewardData rewardData = GetRandomReward();
                if (rewardData != null)
                {
                    Player player = (Player)owner;
                    DbTransaction.RewardPlayer(player, rewardData, Room);
                }
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