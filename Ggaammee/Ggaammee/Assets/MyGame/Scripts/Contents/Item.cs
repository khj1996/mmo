using System;
using UnityEngine;


public class Item
{
    public ItemData data;
    public Action UseItem;

    public int ItemId
    {
        get { return data.id; }
    }

    public string Description
    {
        get => data.description;
    }

    public string Name
    {
        get => data.name;
    }


    private int _count;
    private int _slot;
    private bool _equipped;


    public int Count
    {
        get { return _count; }
        set { _count = value; }
    }

    public int Slot
    {
        get { return _slot; }
        set { _slot = value; }
    }

    public bool Equipped
    {
        get { return _equipped; }
        set { _equipped = value; }
    }


    public ItemType ItemType { get; private set; }
    public bool Stackable { get; protected set; }


    public Item(ItemType itemType)
    {
        ItemType = itemType;
    }

    public static Item MakeItem(ItemData itemData)
    {
        Item item = null;


        switch (itemData.itemType)
        {
            case ItemType.Weapon:
                item = new Weapon(itemData);
                break;
            case ItemType.Armor:
                item = new Armor(itemData);
                break;
            case ItemType.Usable:
                item = new Consumable(itemData);
                break;
            case ItemType.Currency:
                item = new Currency(itemData);
                break;
        }

        if (item != null)
        {
            item.Count = 1;
            item.Slot = 0;
            item.Equipped = false;
        }

        return item;
    }
}

public class Weapon : Item
{
    public Weapon(ItemData _itemData) : base(ItemType.Weapon)
    {
        Init(_itemData);
    }

    void Init(ItemData _itemData)
    {
        data = _itemData;


        UseItem += () => { Debug.Log("장착"); };
    }
}

public class Armor : Item
{
    public Armor(ItemData _itemData) : base(ItemType.Armor)
    {
        Init(_itemData);
    }

    void Init(ItemData _itemData)
    {
        data = _itemData;


        UseItem += () => { Debug.Log("장착"); };
    }
}

public class Consumable : Item
{
    public Consumable(ItemData _itemData) : base(ItemType.Usable)
    {
        Init(_itemData);
    }

    void Init(ItemData _itemData)
    {
        data = _itemData;


        UseItem += () => { Debug.Log("장착"); };
    }
}

public class Currency : Item
{
    public int MaxCount { get; set; }

    public Currency(ItemData _itemData, int amount = 1) : base(ItemType.Currency)
    {
        Init(_itemData, amount);
    }

    void Init(ItemData _itemData, int amount = 1)
    {
        data = _itemData;


        UseItem += () => { Debug.Log("장착"); };
    }
}