using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUISub : UI_ScrollView_Sub
{
    [SerializeField] public InventorySlotUI[] items;

    public override void Init()
    {
    }

    public override void RefreshUI()
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogWarning("InventoryUISub: No slots available for UI update.");
            return;
        }

        int startIndex = _index * items.Length;
        Debug.Log(startIndex);

        for (int i = 0; i < items.Length; i++)
        {
            var item = Managers.InventoryManager.GetSlotData(startIndex + i);
            items[i].SetItem(item, startIndex + i);
        }
    }
}