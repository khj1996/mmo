using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Inventory_Sub : UI_Base
{
    [SerializeField] public UI_Item[] items = new UI_Item[] { };

    public int line;

    public override void Init()
    {
    }

    public void RefreshUI(Define.InvenRefreshType type)
    {
        var slotDatas = Managers.Inven.Items.Values.Where(x => x.Slot < (line + 1) * 5 && x.Slot >= line * 5).ToList();

        for (int i = 0; i < 5; i++)
        {
            items[i].SetItem(slotDatas.FirstOrDefault(x => x.Slot == line * 5 + i));
        }
    }
}