using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Inventory_Sub : UI_Base
{
    public List<UI_Item> Items { get; } = new List<UI_Item>();

    public int line;
    
    public override void Init()
    {
    }
    
    public void RefreshUI(Define.InvenRefreshType type)
    {

        if (Items.Count == 0)
            return;

        var items = Managers.Inven.Items.Values.Where(x => x.Slot >= 0).ToList();
        items.Sort((left, right) => left.Slot - right.Slot);

        foreach (Item item in items)
        {
            if (item.Slot < 0 || item.Slot >= 20)
                continue;

            Items[item.Slot].SetItem(item);
        }
    }
}
