using System;
using UnityEngine;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour
{
    public Image _image;
    [SerializeField] private RectTransform _rectTransform;

    private InventorySlotUI currentDrag;
    private Vector3 _beginDragIconPoint;
    private Vector3 _beginDragCursorPoint;

    private void Start()
    {
        var inventoryUI = FindObjectOfType<InventoryUI>();

        inventoryUI.startDragAction += StartDrag;
        inventoryUI.onDragAction += OnDrag;
        inventoryUI.endDragAction += EndDrag;
        gameObject.SetActive(false);
    }


    public void StartDrag(InventorySlotUI inventorySlotUI)
    {
        gameObject.SetActive(true);
        currentDrag = inventorySlotUI;
        _beginDragIconPoint = inventorySlotUI.Rect.transform.position;
        _beginDragCursorPoint = Input.mousePosition;
        _image.sprite = inventorySlotUI.Item.Data.itemSprite;
    }

    public void OnDrag()
    {
        _rectTransform.position = _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
    }

    public void EndDrag()
    {
        _image.sprite = null;
        gameObject.SetActive(false);
    }
}