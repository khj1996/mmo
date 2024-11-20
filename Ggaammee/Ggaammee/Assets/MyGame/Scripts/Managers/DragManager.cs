using System.Collections.Generic;
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


    private ItemSlotUI _pointerOverSlot;
    private ItemSlotUI _beginDragSlot;
    private PointerEventData _ped;

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
        _ped = new PointerEventData(EventSystem.current);
    }

    private void Update()
    {
        _ped.position = Input.mousePosition;

        HandlePointerEvents();
    }

    private void HandlePointerEvents()
    {
        _pointerOverSlot = GetPointerOverObject();

        ShowOrHideItemTooltip();
        HandlePointerDown();
        HandlePointerDrag();
        HandlePointerUp();
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
            _beginDragSlot = GetPointerOverObject();

            if (_beginDragSlot != null && _beginDragSlot.HasItem())
            {
                StartDrag(_beginDragSlot);
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
        if (_beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            OnDrag();
        }
    }


    public void StartDrag(ItemSlotUI data)
    {
        _draggableItem.StartDrag(data);
    }

    public void OnDrag()
    {
        _draggableItem.OnDrag();
    }


    private void HandlePointerUp()
    {
        if (!Input.GetMouseButtonUp(0) || _beginDragSlot == null) return;

        ItemSlotUI endDragSlot = GetPointerOverObject();

        // 드래그된 아이템이 유효한 슬롯에 놓인 경우
        if (endDragSlot != null)
        {
            endDragSlot.HandleDrop(_beginDragSlot);
        }
        else
        {
            _beginDragSlot.SetItem(null);
        }

        _draggableItem.EndDrag();
        _beginDragSlot = null; // 드래그 시작 슬롯 초기화
    }


    private ItemSlotUI GetPointerOverObject()
    {
        _rrList.Clear();
        _gr.Raycast(_ped, _rrList);

        return _rrList.Count > 0 ? _rrList[0].gameObject.GetComponent<ItemSlotUI>() : null;
    }
}