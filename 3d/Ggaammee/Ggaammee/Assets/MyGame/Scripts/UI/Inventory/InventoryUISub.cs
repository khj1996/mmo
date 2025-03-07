﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUISub : UI_ScrollView_Sub
{
    [SerializeField] public ItemSlotUI[] items;
    public bool isCheck = false;

    public override void Init()
    {
    }

    public override void RefreshUI()
    {
        if (items == null || items.Length == 0)
        {
            return;
        }

        int startIndex = _index * items.Length;

        for (int i = 0; i < items.Length; i++)
        {
            var item = Managers.InventoryManager.GetSlotData(startIndex + i);
            items[i].SetItem(item, startIndex + i);
        }
    }
}