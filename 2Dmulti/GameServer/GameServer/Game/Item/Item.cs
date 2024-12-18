﻿using Google.Protobuf.Protocol;
using GameServer.Data;
using GameServer.DB;

namespace GameServer.Game
{
    public class Item
    {
        public ItemInfo Info { get; } = new ItemInfo();

        //해당 아이템의 고유 id
        public int ItemDbId
        {
            get { return Info.ItemDbId; }
            set { Info.ItemDbId = value; }
        }

        //데이터 상의 아이템 정보 확인용
        public int TemplateId
        {
            get { return Info.TemplateId; }
            set { Info.TemplateId = value; }
        }

        //수량
        public int Count
        {
            get { return Info.Count; }
            set { Info.Count = value; }
        }

        //아이템 위치
        public int Slot
        {
            get { return Info.Slot; }
            set { Info.Slot = value; }
        }

        //장착 여부
        public bool Equipped
        {
            get { return Info.Equipped; }
            set { Info.Equipped = value; }
        }


        //아이템 타입
        public ItemType ItemType { get; private set; }

        //누적 가능 여부
        public bool Stackable { get; protected set; }

        //아이템 타입 설정
        public Item(ItemType itemType)
        {
            ItemType = itemType;
        }

        //아이템 데이터
        public static Item? MakeItem(ItemDb itemDb, int amount = 1)
        {
            Item item = null;

            DataManager.ItemDict.TryGetValue(itemDb.TemplateId, out var itemData);

            if (itemData == null)
                return null;

            switch (itemData.itemType)
            {
                case ItemType.Weapon:
                    item = new Weapon(itemDb.TemplateId);
                    break;
                case ItemType.Armor:
                    item = new Armor(itemDb.TemplateId);
                    break;
                case ItemType.Consumable:
                    item = new Consumable(itemDb.TemplateId);
                    break;
                case ItemType.Currency:
                    item = new Currency(itemDb.TemplateId, amount);
                    break;
            }

            if (item != null)
            {
                item.ItemDbId = itemDb.ItemDbId;
                item.Count = itemDb.Count;
                item.Slot = itemDb.Slot;
                item.Equipped = itemDb.Equipped;
            }

            return item;
        }
    }

    //무기타입
    public class Weapon : Item
    {
        public WeaponType WeaponType { get; private set; }
        public int Damage { get; private set; }

        public Weapon(int templateId) : base(ItemType.Weapon)
        {
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templateId, out itemData);
            if (itemData.itemType != ItemType.Weapon)
                return;

            WeaponData data = (WeaponData)itemData;
            {
                TemplateId = data.id;
                Count = 1;
                WeaponType = data.weaponType;
                Damage = data.damage;
                Stackable = false;
            }
        }
    }

    //방어구 타입
    public class Armor : Item
    {
        public ArmorType ArmorType { get; private set; }
        public int Defence { get; private set; }

        public Armor(int templateId) : base(ItemType.Armor)
        {
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templateId, out itemData);
            if (itemData.itemType != ItemType.Armor)
                return;

            ArmorData data = (ArmorData)itemData;
            {
                TemplateId = data.id;
                Count = 1;
                ArmorType = data.armorType;
                Defence = data.defence;
                Stackable = false;
            }
        }
    }

    //소비타입
    public class Consumable : Item
    {
        public ConsumableType ConsumableType { get; private set; }
        public int MaxCount { get; set; }
        public int Value { get; set; }

        public Consumable(int templateId) : base(ItemType.Consumable)
        {
            Init(templateId);
        }

        void Init(int templateId)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templateId, out itemData);
            if (itemData.itemType != ItemType.Consumable)
                return;

            ConsumableData data = (ConsumableData)itemData;
            {
                TemplateId = data.id;
                Count = 1;
                MaxCount = data.maxCount;
                ConsumableType = data.consumableType;
                Stackable = (data.maxCount > 1);
                Value = data.value;
            }
        }
    }

    //재화타입
    public class Currency : Item
    {
        public int MaxCount { get; set; }

        public Currency(int templateId, int amount = 1) : base(ItemType.Currency)
        {
            Init(templateId, amount);
        }

        void Init(int templateId, int amount = 1)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(templateId, out itemData);
            if (itemData.itemType != ItemType.Currency)
                return;

            CurrencyData data = (CurrencyData)itemData;
            {
                TemplateId = data.id;
                Count = amount;
                MaxCount = data.maxCount;
                Stackable = (data.maxCount > 1);
            }
        }
    }
}