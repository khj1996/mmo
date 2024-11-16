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
        var slotDatas = Managers.InventoryManager.Items.Values.Where(x => x.Slot < (_index + 1) * 5 && x.Slot >= _index * 5).ToList();

        for (int i = 0; i < 5; i++)
        {
            items[i].SetItem(slotDatas.FirstOrDefault(x => x.Slot == _index * 5 + i));
        }
    }
}