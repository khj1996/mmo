using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
{
    public Dictionary<int, Item> Items { get; private set; }

    //public int SlotLen { get; private set; }

    public void Init()
    {
        Items = new Dictionary<int, Item>();
    }


    public void Add(Item _item)
    {
        if (Items.TryGetValue(_item.ItemDbId, out var item))
        {
            Items[item.ItemDbId] = _item;
            return;
        }

        Items.Add(_item.ItemDbId, _item);
    }

    public Item Get(int itemDbId)
    {
        Items.TryGetValue(itemDbId, out var item);
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