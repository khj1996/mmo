using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragManager : UI_Base
{
    public static DragManager Instance { get; private set; }
    [SerializeField] private ItemTooltipUI _itemTooltip;
    [SerializeField] private DraggableItem _draggableItem;

    private readonly List<RaycastResult> _rrList = new List<RaycastResult>(10);
    [SerializeField] private GraphicRaycaster _gr;


    private ItemSlotUI pointerOverSlot;
    private ItemSlotUI beginDragSlot;
    private PointerEventData ped;

    public event Action onBeginDrag;
    public event Action onEndDrag;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void Init()
    {
        ped = new PointerEventData(EventSystem.current);
    }

    private void Update()
    {
        ped.position = Input.mousePosition;

        HandlePointerEvents();
    }

    private void HandlePointerEvents()
    {
        pointerOverSlot = GetPointerOverObject();

        ShowOrHideItemTooltip();
        HandlePointerDown();
        HandlePointerDrag();
        HandlePointerUp();
    }

    private void ShowOrHideItemTooltip()
    {
        if (pointerOverSlot != null && pointerOverSlot.HasItem() && pointerOverSlot != beginDragSlot)
        {
            UpdateTooltipUI(pointerOverSlot);
            _itemTooltip.Show();
        }
        else
        {
            _itemTooltip.Hide();
        }
    }

    private void UpdateTooltipUI(ItemSlotUI slot)
    {
        if (!slot.HasItem()) return;

        _itemTooltip.SetItemInfo(slot.Item.Data);
        _itemTooltip.SetRectPosition(slot.Rect);
    }

    private void HandlePointerDown()
    {
        /*
            switch (_beginDragSlot.gameObject.tag)
            {
                case "InventorySlot":
                    break;
                case "QuickSlot":
                    break;
                case "EquipSlot":
                    break;
            }*/

        if (Input.GetMouseButtonDown(0))
        {
            beginDragSlot = GetPointerOverObject();

            if (beginDragSlot != null && beginDragSlot.HasItem())
            {
                StartDrag(beginDragSlot);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            var slot = GetPointerOverObject();
            slot?.UseItem();
        }
    }

    private void HandlePointerDrag()
    {
        if (beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            OnDrag();
        }
    }


    public void StartDrag(ItemSlotUI data)
    {
        onBeginDrag?.Invoke();
        _draggableItem.StartDrag(data);
    }

    public void OnDrag()
    {
        _draggableItem.OnDrag();
    }


    private void HandlePointerUp()
    {
        if (!Input.GetMouseButtonUp(0) || beginDragSlot == null) return;

        ItemSlotUI endDragSlot = GetPointerOverObject();

        if (endDragSlot != null)
        {
            endDragSlot.HandleDrop(beginDragSlot);
        }
        else
        {
            beginDragSlot.SetItem(null);
        }

        onEndDrag?.Invoke();
        _draggableItem.EndDrag();
        beginDragSlot = null;
    }


    private ItemSlotUI GetPointerOverObject()
    {
        _rrList.Clear();
        _gr.Raycast(ped, _rrList);

        return _rrList.Count > 0 ? _rrList[0].gameObject.GetComponent<ItemSlotUI>() : null;
    }
}