using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using GameServer.DB;
using GameServer.Game.Room;
using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Data;

namespace GameServer.Game
{
    public class Player : GameObject
    {
        IJob? _job;


        public int PlayerDbId { get; set; }
        public ClientSession Session { get; set; }
        public VisionCube Vision { get; private set; }

        public Inventory Inven { get; private set; } = new Inventory();

        public int WeaponDamage { get; private set; }
        public int ArmorDefence { get; private set; }

        public override int TotalAttack
        {
            get { return Info.StatInfo.Attack + WeaponDamage; }
        }

        public override int TotalDefence
        {
            get { return ArmorDefence; }
        }


        public Player()
        {
            State = CreatureState.Idle;
            ObjectType = GameObjectType.Player;
            Vision = new VisionCube(this);
        }


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

            if (Room != null)
                _job = Room.PushAfter(Room.TickInterval, Update);
        }

        protected virtual void UpdateIdle()
        {
            if (State == CreatureState.Skill)
                return;

            if (Move.X != 0 || Move.Y != 0)
                State = CreatureState.Moving;
        }

        protected virtual void UpdateMoving()
        {
            if (State == CreatureState.Skill)
                return;
            if (Move is { X: 0, Y: 0 })
            {
                State = CreatureState.Idle;
            }
            else
            {
                UpdatePosition();
            }
        }

        public void RefreshMoveData()
        {
        }

        public long coolTick = 0;

        protected virtual void UpdateSkill()
        {
            if (coolTick > Environment.TickCount64)
                return;

            State = CreatureState.Idle;
        }

        protected virtual void UpdateDead()
        {
        }

        public override void UpdateLevel(StatInfo statInfo)
        {
            if (Level == statInfo.Level || Room == null)
            {
                return;
            }

            Info.StatInfo.Level = statInfo.Level;
            Info.StatInfo.MaxHp = statInfo.MaxHp;
            Info.StatInfo.Hp = statInfo.MaxHp;
            Info.StatInfo.Attack = statInfo.Attack;
            Info.StatInfo.Speed = statInfo.Speed;

            S_UpdateLevel levelPacket = new S_UpdateLevel()
            {
                Level = statInfo.Level,
                StatInfo = statInfo
            };
            Session.Send(levelPacket);


            var changePacket = new S_ChangeHp
            {
                ObjectId = Id,
                Hp = Info.StatInfo.Hp
            };

            Room.Broadcast(CellPos, changePacket, PlayerDbId);
        }

        public void OnLeaveGame()
        {
            // TODO
            // DB 연동?
            // -- 피가 깎일 때마다 DB 접근할 필요가 있을까?
            // 1) 서버 다운되면 아직 저장되지 않은 정보 날아감
            // 2) 코드 흐름을 다 막아버린다 !!!!
            // - 비동기(Async) 방법 사용?
            // - 다른 쓰레드로 DB 일감을 던져버리면 되지 않을까?
            // -- 결과를 받아서 이어서 처리를 해야 하는 경우가 많음.
            // -- 아이템 생성

            DbTransaction.SavePlayerStatus(this, Room);
        }

        public void HandleUseItem(C_UseItem usePacket)
        {
            var item = Inven.Get(usePacket.ItemDbId);
            if (item == null)
                return;

            if (item.ItemType == ItemType.Consumable)
            {
                var updatedCostItem = new ItemDb()
                {
                    ItemDbId = item.ItemDbId,
                    TemplateId = item.TemplateId,
                    Count = item.Count - 1,
                    Slot = item.Slot,
                    OwnerDbId = PlayerDbId
                };
                // DB에 Noti
                DbTransaction.UseItemNoti(this, updatedCostItem);
                S_UseItem equipOkItem = new S_UseItem();
                equipOkItem.ItemDbId = updatedCostItem.ItemDbId;

                Hp += ((Consumable)item).Value;

                Session.Send(equipOkItem);

                var changePacket = new S_ChangeHp
                {
                    ObjectId = Id,
                    Hp = Info.StatInfo.Hp
                };

                Room.Broadcast(CellPos, changePacket);
            }
            else if (item.ItemType == ItemType.Armor || item.ItemType == ItemType.Weapon)
            {
                // 착용 요청이라면, 겹치는 부위 해제
                if (usePacket.Equipped)
                {
                    Item? unequipItem = null;

                    switch (item.ItemType)
                    {
                        case ItemType.Weapon:
                            unequipItem = Inven.Find(x => x.Equipped && x.ItemType == ItemType.Weapon);
                            break;
                        case ItemType.Armor:
                        {
                            var armorType = ((Armor)item).ArmorType;
                            unequipItem = Inven.Find(x => x.Equipped && x.ItemType == ItemType.Armor && ((Armor)x).ArmorType == armorType);
                            break;
                        }
                    }

                    if (unequipItem != null)
                    {
                        // 메모리 선적용
                        unequipItem.Equipped = false;

                        // DB에 Noti
                        DbTransaction.EquipItemNoti(this, unequipItem);

                        // 클라에 통보
                        S_UseItem equipOkItem = new S_UseItem();
                        equipOkItem.ItemDbId = unequipItem.ItemDbId;
                        equipOkItem.Equipped = unequipItem.Equipped;
                        Session.Send(equipOkItem);
                    }
                }


                {
                    // 메모리 선적용
                    item.Equipped = usePacket.Equipped;

                    // DB에 Noti
                    DbTransaction.EquipItemNoti(this, item);

                    // 클라에 통보
                    S_UseItem equipOkItem = new S_UseItem();
                    equipOkItem.ItemDbId = usePacket.ItemDbId;
                    equipOkItem.Equipped = usePacket.Equipped;
                    Session.Send(equipOkItem);
                }

                RefreshAdditionalStat();
            }
        }


        public Vector2Float GetFrontPos()
        {
            var cellPos = CellPos;
            if (Math.Abs(LookDir.X) >= Math.Abs(LookDir.Y))
            {
                if (LookDir.X > 0)
                    return cellPos + Vector2Float.Right;
                else
                    return cellPos + Vector2Float.Left;
            }
            else
            {
                if (LookDir.Y > 0)
                    return cellPos + Vector2Float.Up;
                else
                    return cellPos + Vector2Float.Down;
            }

            return cellPos;
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
                var rewardData = GetRandomReward();
                if (rewardData != null)
                {
                    Player player = (Player)owner;
                    DbTransaction.RewardPlayer(player, rewardData, Room);
                }

                owner.Exp += 3;
            }
        }

        RewardData? GetRandomReward()
        {
            DataManager.MonsterDict.TryGetValue(1, out var monsterData);

            var rand = new Random().Next(0, 101);

            var sum = 0;
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

        public void RefreshAdditionalStat()
        {
            WeaponDamage = 0;
            ArmorDefence = 0;

            foreach (Item item in Inven.Items.Values)
            {
                if (item.Equipped == false)
                    continue;

                switch (item.ItemType)
                {
                    case ItemType.Weapon:
                        WeaponDamage += ((Weapon)item).Damage;
                        break;
                    case ItemType.Armor:
                        ArmorDefence += ((Armor)item).Defence;
                        break;
                }
            }
        }
    }
}