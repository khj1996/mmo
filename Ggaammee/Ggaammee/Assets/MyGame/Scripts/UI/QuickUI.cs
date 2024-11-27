using System;
using System.Linq;
using UnityEngine;

public class QuickUI : UI_Base
{
    [SerializeField] private ItemSlotUI[] quickSlots; 

    //public event Action<ItemData> quickSlotChange;

    public override void Init()
    {
        for (var index = 0; index < quickSlots.Length; index++)
        {
            var slot = quickSlots[index];
            slot.SetItem(null, index);
            slot.OnDropItem -= HandleQuickSlotChange;
            slot.OnDropItem += HandleQuickSlotChange;
        }
    }

    private void HandleQuickSlotChange(ItemSlotUI fromSlot, ItemSlotUI toSlot)
    {
        foreach (var slot in quickSlots)
        {
            var firstItem = slot.Item;
            var secondItem = fromSlot.Item;

            if (firstItem != null && secondItem != null && (secondItem.Data == firstItem.Data))
            {
                slot.SetItem(null);
                break;
            }
        }

        toSlot.SetItem(fromSlot.Item);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            quickSlots[0].UseItem();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            quickSlots[1].UseItem(); 
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            quickSlots[2].UseItem();
        }
    }
}