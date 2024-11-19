using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : UI_ScrollView<InventoryUISub>
{
    [Header("References")] [SerializeField]
    private ItemTooltipUI _itemTooltip;

    [SerializeField] private bool _showRemovingPopup = true;
    [SerializeField] private DraggableItem _draggableItem;
    [SerializeField] private InventoryPopupUI _popup;
    [SerializeField] private GraphicRaycaster _gr;

    private PointerEventData _ped;
    private InventorySlotUI _pointerOverSlot;
    private InventorySlotUI _beginDragSlot;

    private readonly List<RaycastResult> _rrList = new List<RaycastResult>(10);

    public Action<InventorySlotUI> startDragAction;
    public Action onDragAction;
    public Action endDragAction;

    private bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();

    public override void Init()
    {
        _ped = new PointerEventData(EventSystem.current);
        maxIndex = Managers.InventoryManager.SlotCapacity / 5 + 1;

        base.Init();

        Managers.InventoryManager.SlotChanged -= RefreshSlot;
        Managers.InventoryManager.SlotChanged += RefreshSlot;

        InitializeView();
    }

    private void Update()
    {
        _ped.position = Input.mousePosition;

        HandlePointerEvents();
    }

    private void HandlePointerEvents()
    {
        _pointerOverSlot = RaycastAndGetFirstComponent<InventorySlotUI>();

        ShowOrHideItemTooltip();
        HandlePointerDown();
        HandlePointerDrag();
        HandlePointerUp();
    }

    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear();
        _gr.Raycast(_ped, _rrList);

        return _rrList.Count > 0 ? _rrList[0].gameObject.GetComponent<T>() : null;
    }

    private void ShowOrHideItemTooltip()
    {
        if (_pointerOverSlot != null && _pointerOverSlot.HasItem() && _pointerOverSlot != _beginDragSlot)
        {
            UpdateTooltipUI(_pointerOverSlot);
            _itemTooltip.Show();
        }
        else
        {
            _itemTooltip.Hide();
        }
    }

    private void UpdateTooltipUI(InventorySlotUI slot)
    {
        if (!slot.HasItem()) return;

        _itemTooltip.SetItemInfo(slot.Item.Data);
        _itemTooltip.SetRectPosition(slot.Rect);
    }

    private void HandlePointerDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _beginDragSlot = RaycastAndGetFirstComponent<InventorySlotUI>();

            if (_beginDragSlot != null && _beginDragSlot.HasItem())
            {
                scrollRect.vertical = false;
                startDragAction?.Invoke(_beginDragSlot);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            var slot = RaycastAndGetFirstComponent<InventorySlotUI>();
            slot?.UseItem();
        }
    }

    private void HandlePointerDrag()
    {
        if (_beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            onDragAction?.Invoke();
        }
    }

    private void HandlePointerUp()
    {
        if (!Input.GetMouseButtonUp(0) || _beginDragSlot == null) return;

        scrollRect.vertical = true;

        InventorySlotUI endDragSlot = RaycastAndGetFirstComponent<InventorySlotUI>();
        if (endDragSlot != null)
        {
            HandleItemSwapOrSeparation(_beginDragSlot, endDragSlot);
        }

        endDragAction?.Invoke();
        _beginDragSlot = null;
    }

    private void HandleItemSwapOrSeparation(InventorySlotUI fromSlot, InventorySlotUI toSlot)
    {
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

        UpdateTooltipUI(toSlot);
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

    private void TrySwapItems(InventorySlotUI from, InventorySlotUI to)
    {
        if (from == to) return;
        Debug.Log(from.Index);
        Debug.Log(to.Index);

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