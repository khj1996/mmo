using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : UI_ScrollView<InventoryUISub>
{
    public static DraggableItem dragItem;

    public override void Init()
    {
        maxIndex = Managers.InventoryManager.SlotLen / 5 + 1;


        base.Init();

        Managers.InventoryManager.ChangeItemAction -= RefreshSlot;
        Managers.InventoryManager.ChangeItemAction += RefreshSlot;

        InitializeView();
    }

    public void RefreshSlot(int index, Item item)
    {
        int line = index / 5;
        int slot = index - (line * 5);


        var current = items.First;
        for (int i = 0; i < line; i++)
        {
            current = current.Next;
        }

        current.Value.items[slot].SetItem(item);
    }


    public void RefreshUI()
    {
        if (Managers.InventoryManager.Items.Count == 0)
            return;

        foreach (var sub in items)
        {
            sub.RefreshUI(sub._index);
        }
    }
}