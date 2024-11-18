using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : UI_ScrollView<InventoryUISub>
{
    [SerializeField] private ItemTooltipUI _itemTooltip;
    [SerializeField] private bool _showRemovingPopup = true;
    [SerializeField] private DraggableItem _draggableItem;
    [SerializeField] private InventoryPopupUI _popup;
    [SerializeField] private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private InventorySlotUI _pointerOverSlot;
    private List<RaycastResult> _rrList = new List<RaycastResult>(10);

    private InventorySlotUI _beginDragSlot;

    public Action<InventorySlotUI> startDragAction;
    public Action onDragAction;
    public Action endDragAction;

    private bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();

    public override void Init()
    {
        _ped = new PointerEventData(EventSystem.current);
        maxIndex = Managers.InventoryManager.SlotLen / 5 + 1;


        base.Init();

        Managers.InventoryManager.ChangeItemAction -= RefreshSlot;
        Managers.InventoryManager.ChangeItemAction += RefreshSlot;

        InitializeView();
    }

    private void Update()
    {
        _ped.position = Input.mousePosition;

        OnPointerEnterAndExit();
        ShowOrHideItemTooltip();
        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
    }

    private void OnPointerEnterAndExit()
    {
        _pointerOverSlot = RaycastAndGetFirstComponent<InventorySlotUI>();
    }

    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear();

        _gr.Raycast(_ped, _rrList);

        if (_rrList.Count == 0)
            return null;

        return _rrList[0].gameObject.GetComponent<T>();
    }

    private void ShowOrHideItemTooltip()
    {
        bool isValid = _pointerOverSlot != null && _pointerOverSlot.HasItem() && (_pointerOverSlot != _beginDragSlot);

        if (isValid)
        {
            UpdateTooltipUI(_pointerOverSlot);
            _itemTooltip.Show();
        }
        else
            _itemTooltip.Hide();
    }

    private void UpdateTooltipUI(InventorySlotUI slot)
    {
        if (!slot.HasItem())
            return;

        _itemTooltip.SetItemInfo(slot.Item.Data);
        _itemTooltip.SetRectPosition(slot._rect);
    }


    private void OnPointerDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _beginDragSlot = RaycastAndGetFirstComponent<InventorySlotUI>();

            if (_beginDragSlot != null && _beginDragSlot.HasItem())
            {
                scrollRect.vertical = false;
                startDragAction?.Invoke(_beginDragSlot);
            }
            else
            {
                _beginDragSlot = null;
            }
        }

        else if (Input.GetMouseButtonDown(1))
        {
            InventorySlotUI slot = RaycastAndGetFirstComponent<InventorySlotUI>();

            if (slot != null && slot.HasItem())
            {
                slot.UseItem();
            }
        }
    }

    private void OnPointerDrag()
    {
        if (_beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            onDragAction?.Invoke();
        }
    }

    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (_beginDragSlot != null)
            {
                scrollRect.vertical = true;

                EndDrag();

                endDragAction?.Invoke();

                _beginDragSlot = null;
            }
        }
    }

    private void EndDrag()
    {
        InventorySlotUI endDragSlot = RaycastAndGetFirstComponent<InventorySlotUI>();

        if (endDragSlot != null)
        {
            bool isSeparatable = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) && (_beginDragSlot.Item is StackableItem);

            bool isSeparation = false;
            int currentAmount = 0;

            if (isSeparatable)
            {
                currentAmount = ((StackableItem)_beginDragSlot.Item).Count;
                if (currentAmount > 1)
                {
                    isSeparation = true;
                }
            }

            if (isSeparation)
                TrySeparateAmount(_beginDragSlot.index, endDragSlot.index, currentAmount);
            else
                TrySwapItems(_beginDragSlot, endDragSlot);

            UpdateTooltipUI(endDragSlot);
            return;
        }

        /*if (!IsOverUI())
        {
            int index = _beginDragSlot.index;
            string itemName = _beginDragSlot.Item.Data.name;
            int amount = Managers.InventoryManager.GetCurrentAmount(index);

            if (amount > 1)
                itemName += $" x{amount}";

            if (_showRemovingPopup)
                _popup.OpenConfirmationPopup(() => TryRemoveItem(index), itemName);
            else
                TryRemoveItem(index);
        }
        else
        {
        }*/
    }

    private void TryRemoveItem(int index)
    {
        Managers.InventoryManager.Remove(index);
    }

    private void TrySeparateAmount(int indexA, int indexB, int amount)
    {
        if (indexA == indexB)
        {
            return;
        }

        string itemName = $"{Managers.InventoryManager.GetItemName(indexA)} x{amount}";

        _popup.OpenAmountInputPopup(
            amt => Managers.InventoryManager.SeparateAmount(indexA, indexB, amt),
            amount, itemName
        );
    }

    private void TrySwapItems(InventorySlotUI from, InventorySlotUI to)
    {
        if (from == to)
        {
            return;
        }

        Debug.Log(from.index);
        Debug.Log(to.index);

        Managers.InventoryManager.Swap(from.index, to.index);
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

        current.Value.items[slot].SetItem(item, index);
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