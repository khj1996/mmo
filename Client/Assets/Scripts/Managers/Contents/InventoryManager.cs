using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class InventoryManager
{
    public Dictionary<int, Item> Items { get; private set; }

    public int SlotLen
    {
        get { return Util.StaticValues.InventorySize; }
        private set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            SlotLen = value;
        }
    }

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

    public void Add(List<ItemInfo> _itemInfos)
    {
        foreach (ItemInfo itemInfo in _itemInfos)
        {
            Item itemData = Item.MakeItem(itemInfo);

            if (Items.TryGetValue(itemData.ItemDbId, out var _item))
            {
                Items[_item.ItemDbId] = _item;
                return;
            }

            Items.Add(itemData.ItemDbId, itemData);
        }

        UI_GameScene gameSceneUI = Managers.UI.CurrentSceneUI as UI_GameScene;
        gameSceneUI.InvenUI.RefreshUI(Define.InvenRefreshType.All);

        Managers.UI.ShowPopupUI<UI_GetItemPopUp>().OpenPopUpMultiItem(_itemInfos);
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