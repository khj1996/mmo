using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUISub : UI_ScrollView_Sub
{
    [SerializeField] public InventorySlotUI[] items = new InventorySlotUI[] { };

    public override void Init()
    {
    }


    public override void RefreshUI(int _index)
    {
        var startIndex = _index * 5;


        for (int i = 0; i < 5; i++)
        {
            var item = Managers.InventoryManager.Get(startIndex + i);
            items[i].SetItem(item, startIndex + i);
        }
    }
}