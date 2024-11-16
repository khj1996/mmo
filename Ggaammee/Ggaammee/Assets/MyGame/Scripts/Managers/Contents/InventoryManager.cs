using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
{
    public Dictionary<int, Item> Items { get; private set; } = new Dictionary<int, Item>();
    public Action ChangeItemAction;

    public int SlotLen
    {
        get { return Util.StaticValues.InventorySize; }
        private set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            SlotLen = value;
        }
    }


    public bool Add(ItemData _itemData)
    {
        Item newItme = Item.MakeItem(_itemData);

        if (Items.TryGetValue(newItme.ItemId, out var item))
        {
            Items[item.ItemId].Count += 1;
            return true;
        }

        Items.Add(newItme.ItemId, newItme);
        
        
        ChangeItemAction?.Invoke();
        return true;
    }

    public void Add(List<ItemData> _itemInfos)
    {
        List<Item> addedItem = new List<Item>();

        foreach (ItemData itemInfo in _itemInfos)
        {
            Item newItme = Item.MakeItem(itemInfo);

            if (Items.TryGetValue(newItme.ItemId, out var _item))
            {
                Items[_item.ItemId] = _item;
                addedItem.Add(_item);
                continue;
            }

            Items.Add(newItme.ItemId, newItme);
            addedItem.Add(newItme);
        }

        ChangeItemAction?.Invoke();
    }

    public Item Get(int Id)
    {
        Items.TryGetValue(Id, out var item);
        return item;
    }

    public Item Find(Func<Item, bool> condition)
    {
        foreach (Item item in Items.Values)
        {
            if (condition.Invoke(item))
                return item;
        }

        return null;
    }

    public void Clear()
    {
        Items.Clear();
    }
}