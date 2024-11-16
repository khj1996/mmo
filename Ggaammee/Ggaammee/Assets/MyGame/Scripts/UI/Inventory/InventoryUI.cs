using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : UI_ScrollView<InventoryUISub>
{
    public override void Init()
    {
        maxIndex = Managers.InventoryManager.SlotLen / 5 + 1;


        base.Init();

        Managers.InventoryManager.ChangeItemAction -= RefreshUI;
        Managers.InventoryManager.ChangeItemAction += RefreshUI;

        InitializeView();
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