using System;
using UnityEngine;


public abstract class Item
{
    public ItemData Data { get; private set; }

    public Item(ItemData data)
    {
        Data = data;
    }


}


/*public class Item
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

    public Sprite ItemSprite
    {
        get => data.itemSprite;
    }

    public bool Stackable
    {
        get => ((UsableItemData)data).Stackable;
    }
    public bool IsMax
    {
        get => ItemData.;
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




    public Item(ItemData _data)
    {
        data = _data;
    }

    public static Item MakeItem(ItemData itemData)
    {
        Item item = itemData switch
        {
            WeaponItemData weaponData => new Weapon(weaponData),
            ArmorItemData armorData => new Armor(armorData),
            UsableItemData usableData => new Consumable(usableData),
            CurrencyItemData currencyData => new Currency(currencyData),
            _ => null
        };

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
    public Weapon(ItemData _itemData) : base(_itemData)
    {
        data = _itemData;
        UseItem += OnUseItem;
    }

    private void OnUseItem()
    {
        Debug.Log("장착");
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
    public int currentStack;


    public int MaxStack
    {
        get => ((UsableItemData)data).maxStack;
    }

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
}*/