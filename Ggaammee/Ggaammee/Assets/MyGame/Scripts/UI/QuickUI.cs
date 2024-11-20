using System;
using System.Linq;
using UnityEngine;

public class QuickUI : UI_Base
{
    [SerializeField] private ItemSlotUI[] quickSlots; // 3개의 퀵슬롯

    public event Action<ItemData> quickSlotChange;

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
        // 같은 아이템을 포함한 퀵슬롯이 있는 경우 null 처리
        foreach (var slot in quickSlots)
        {
            var firstItem = slot.Item;
            var secondItem = fromSlot.Item;

            //처음 아이템이 존재하며 두번째 아이템에 처음 아이템이랑 같은 아이템이 들어가있는경우
            if (firstItem != null && secondItem != null && (secondItem.Data == firstItem.Data))
            {
                slot.SetItem(null);
                break;
            }
        }

        // 아이템 이동 처리
        toSlot.SetItem(fromSlot.Item);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            quickSlots[0].UseItem(); // 슬롯 1 아이템 사용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            quickSlots[1].UseItem(); // 슬롯 2 아이템 사용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            quickSlots[2].UseItem(); // 슬롯 3 아이템 사용
        }
    }
}