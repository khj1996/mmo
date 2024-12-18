﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryUI : UI_ScrollView<InventoryUISub>
{
    //[SerializeField] private bool showRemovingPopup = true;
    [SerializeField] private InventoryPopupUI _popup;

    public override void Init()
    {
        maxIndex = Managers.InventoryManager.SlotCapacity / 5 + 1;
        Managers.InventoryManager.SlotChanged -= RefreshSlot;
        Managers.InventoryManager.SlotChanged += RefreshSlot;

        base.Init();
        InitializeView();
        Managers.InventoryManager.InitializeInventory();

        DragManager.Instance.onBeginDrag += () => { scrollRect.vertical = false; };

        DragManager.Instance.onEndDrag += () => { scrollRect.vertical = true; };

        gameObject.SetActive(false);
    }

    protected override void InitializedItem(InventoryUISub item, int _index)
    {
        foreach (var slot in item.items)
        {
            slot.OnDropItem += HandleItemSwapOrSeparation;
        }

        base.InitializedItem(item, _index);
    }

    private void HandleItemSwapOrSeparation(ItemSlotUI fromSlot, ItemSlotUI toSlot)
    {
        if (fromSlot.Index == -1)
        {
            Managers.InventoryManager.UnequipItem(((EquipItemData)fromSlot.Item.Data).type);
            return;
        }

        bool isSeparatable = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) &&
                             (fromSlot.Item is StackableItem stackable && stackable.Count > 1);

        if (isSeparatable)
        {
            TrySeparateAmount(fromSlot.Index, toSlot.Index, ((StackableItem)fromSlot.Item).Count);
        }
        else
        {
            TrySwapItems(fromSlot, toSlot);
        }

        //UpdateTooltipUI(toSlot);
    }

    private void TryRemoveItem(int index)
    {
        Managers.InventoryManager.RemoveItem(index);
    }

    private void TrySeparateAmount(int indexA, int indexB, int amount)
    {
        string itemName = $"{Managers.InventoryManager.GetItemName(indexA)} x{amount}";

        _popup.OpenAmountInputPopup(
            amt => Managers.InventoryManager.SeparateAmount(indexA, indexB, amt),
            amount, itemName
        );
    }

    private void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
    {
        if (from == to) return;

        Managers.InventoryManager.Swap(from.Index, to.Index);
    }

    public void RefreshSlot(int index, Item item)
    {
        int line = index / 5;
        int slot = index % 5;

        var current = items.First;
        for (int i = 0; i < items.Count; i++)
        {
            if (current == null) break;

            if (current.Value._index == line)
            {
                current.Value.items[slot].SetItem(item, index);
                break;
            }

            current = current.Next;
        }
    }
}