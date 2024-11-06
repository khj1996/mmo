using System;
using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public ItemInfo Info { get; } = new ItemInfo();
    public Action useItem;

    public int ItemDbId
    {
        get { return Info.ItemDbId; }
        set { Info.ItemDbId = value; }
    }

    public int TemplateId
    {
        get { return Info.TemplateId; }
        set { Info.TemplateId = value; }
    }

    public int Count
    {
        get { return Info.Count; }
        set { Info.Count = value; }
    }

    public int Slot
    {
        get { return Info.Slot; }
        set { Info.Slot = value; }
    }

    public bool Equipped
    {
        get { return Info.Equipped; }
        set { Info.Equipped = value; }
    }

    public string Name
    {
        get => name;
        set => name = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }

    public ItemType ItemType { get; private set; }
    public bool Stackable { get; protected set; }

    private string name;
    private string description;

    public Item(ItemType itemType)
    {
        ItemType = itemType;
    }

    public static Item MakeItem(ItemInfo itemInfo)
    {
        Item item = null;

        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(itemInfo.TemplateId, out itemData);

        if (itemData == null)
            return null;

        switch (itemData.itemType)
        {
            case ItemType.Weapon:
                item = new Weapon(itemInfo.TemplateId);
                break;
            case ItemType.Armor:
                item = new Armor(itemInfo.TemplateId);
                break;
            case ItemType.Consumable:
                item = new Consumable(itemInfo.TemplateId);
                break;
            case ItemType.Currency:
                item = new Currency(itemInfo.TemplateId);
                break;
        }

        if (item != null)
        {
            item.ItemDbId = itemInfo.ItemDbId;
            item.Count = itemInfo.Count;
            item.Slot = itemInfo.Slot;
            item.Equipped = itemInfo.Equipped;
            item.Name = itemData.name;
        }

        return item;
    }
}

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
        Managers.Data.ItemDict.TryGetValue(templateId, out itemData);
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

        useItem += () =>
        {
            C_UseItem equipPacket = new C_UseItem();
            equipPacket.ItemDbId = ItemDbId;
            equipPacket.Equipped = !Equipped;

            Managers.Network.Send(equipPacket);
        };

        Description = $"장착부위 : 무기\n공격력 : {data.damage}증가";
    }
}

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
        Managers.Data.ItemDict.TryGetValue(templateId, out itemData);
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
        useItem += () =>
        {
            C_UseItem equipPacket = new C_UseItem();
            equipPacket.ItemDbId = ItemDbId;
            equipPacket.Equipped = !Equipped;

            Managers.Network.Send(equipPacket);
        };

        Description = $"장착부위 : {data.armorType}\n방어력 : {data.defence}증가";
    }
}

public class Consumable : Item
{
    public ConsumableType ConsumableType { get; private set; }
    public int MaxCount { get; set; }

    public Consumable(int templateId) : base(ItemType.Consumable)
    {
        Init(templateId);
    }

    void Init(int templateId)
    {
        if (Managers.Data.ItemDict.TryGetValue(templateId, out var itemData))
        {
            if (itemData.itemType != ItemType.Consumable)
                return;


            ConsumableData data = (ConsumableData)itemData;
            {
                TemplateId = data.id;
                Count = 1;
                MaxCount = data.maxCount;
                ConsumableType = data.consumableType;
                Stackable = (data.maxCount > 1);
            }

            useItem += () =>
            {
                if (Count <= 0)
                    return;
                C_UseItem equipPacket = new C_UseItem();
                equipPacket.ItemDbId = ItemDbId;

                Managers.Network.Send(equipPacket);
            };

            Description = $"소모아이템 : 포션\n생명력 : {data.value}회복";
        }
    }
}

public class Currency : Item
{
    public int MaxCount { get; set; }

    public Currency(int templateId, int amount = 1) : base(ItemType.Currency)
    {
        Init(templateId, amount);
    }

    void Init(int templateId, int amount = 1)
    {
        if (Managers.Data.ItemDict.TryGetValue(templateId, out var itemData))
        {
            if (itemData.itemType != ItemType.Currency)
                return;

            CurrencyData data = (CurrencyData)itemData;
            {
                TemplateId = data.id;
                Count = amount;
                MaxCount = data.maxCount;
                Stackable = (data.maxCount > 1);
            }
            Description = "재화";
        }
    }
}