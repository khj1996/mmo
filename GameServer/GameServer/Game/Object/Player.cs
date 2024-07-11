using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using GameServer.DB;
using GameServer.Game.Room;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Game
{
    public class Player : GameObject
    {
        //플레이어 DB ID
        public int PlayerDbId { get; set; }

        //유저 세션
        public ClientSession Session { get; set; }

        //시야
        public VisionCube Vision { get; private set; }

        //인벤토리
        public Inventory Inven { get; private set; } = new Inventory();

        //공격력(장비)
        public int WeaponDamage { get; private set; }

        //방어력(장비)
        public int ArmorDefence { get; private set; }

        //전체 공격력
        public override int TotalAttack
        {
            get { return Info.StatInfo.Attack + WeaponDamage; }
        }

        //전체 방어력
        public override int TotalDefence
        {
            get { return ArmorDefence; }
        }

        // FSM (Finite State Machine)
        IJob _job;

        //생성자
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
                    //UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;
            }

            // 25프레임 (0.2초마다 한번씩 Update)
            if (Room != null)
                _job = Room.PushAfter(40, Update);
        }

        protected virtual void UpdateIdle()
        {
        }

        public override void UpdateMoveDir(C_Move movePacket)
        {
            base.UpdateMoveDir(movePacket);
        }


        //유저 피격
        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }

        protected virtual void UpdateDead()
        {
        }

        //유저 사망
        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }

        private void UpdateMoving()
        {
            Move();

            BroadcastMove();
        }

        void BroadcastMove()
        {
            // 다른 플레이어한테도 알려준다
            S_Move resMovePacket = new S_Move();
            resMovePacket.ObjectId = Info.ObjectId;
            resMovePacket.PosInfo = Info.PosInfo;
            resMovePacket.MoveDir = MoveDir;

            List<Zone> zones = Room.GetAdjacentZones(CellPos);

            foreach (Player p in zones.SelectMany(z => z.Players))
            {
                float dx = p.CellPos.x - CellPos.x;
                float dy = p.CellPos.y - CellPos.y;
                if (Math.Abs(dx) > GameRoom.VisionCells)
                    continue;
                if (Math.Abs(dy) > GameRoom.VisionCells)
                    continue;

                p.Session.Send(resMovePacket);
            }
        }


        //TODO : 나중 테스트
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

        //장비 장착 처리
        public void HandleEquipItem(C_EquipItem equipPacket)
        {
            //아이템 데이터 획득
            Item item = Inven.Get(equipPacket.ItemDbId);
            if (item == null)
                return;

            //소모품 체크
            if (item.ItemType == ItemType.Consumable)
                return;

            // 착용 요청이라면, 겹치는 부위 해제
            if (equipPacket.Equipped)
            {
                Item unequipItem = null;

                if (item.ItemType == ItemType.Weapon)
                {
                    unequipItem = Inven.Find(
                        i => i.Equipped && i.ItemType == ItemType.Weapon);
                }
                else if (item.ItemType == ItemType.Armor)
                {
                    ArmorType armorType = ((Armor)item).ArmorType;
                    unequipItem = Inven.Find(
                        i => i.Equipped && i.ItemType == ItemType.Armor
                                        && ((Armor)i).ArmorType == armorType);
                }

                if (unequipItem != null)
                {
                    // 메모리 선적용
                    unequipItem.Equipped = false;

                    // DB에 Noti
                    DbTransaction.EquipItemNoti(this, unequipItem);

                    // 클라에 통보
                    S_EquipItem equipOkItem = new S_EquipItem();
                    equipOkItem.ItemDbId = unequipItem.ItemDbId;
                    equipOkItem.Equipped = unequipItem.Equipped;
                    Session.Send(equipOkItem);
                }
            }

            {
                // 메모리 선적용
                item.Equipped = equipPacket.Equipped;

                // DB에 Noti
                DbTransaction.EquipItemNoti(this, item);

                // 클라에 통보
                S_EquipItem equipOkItem = new S_EquipItem();
                equipOkItem.ItemDbId = equipPacket.ItemDbId;
                equipOkItem.Equipped = equipPacket.Equipped;
                Session.Send(equipOkItem);
            }

            RefreshAdditionalStat();
        }

        //추가 스탯 갱신
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