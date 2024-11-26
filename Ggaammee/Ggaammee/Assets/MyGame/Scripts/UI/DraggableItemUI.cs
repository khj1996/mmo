using System;
using UnityEngine;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour
{
    public Image Image { get; private set; }
    public RectTransform RectTransform { get; private set; }

    private Vector3 _beginDragIconPoint;
    private Vector3 _beginDragCursorPoint;

    private void Awake()
    {
        Image = GetComponent<Image>();
        RectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false); 
    }

    public void StartDrag(ItemSlotUI data)
    {
        Image.sprite = data.Item.Data.itemSprite;
        _beginDragIconPoint = data.Rect.transform.position;
        _beginDragCursorPoint = Input.mousePosition;

        gameObject.SetActive(true); 
    }

    public void OnDrag()
    {
        RectTransform.position = _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
    }

    public void EndDrag()
    {
        Image.sprite = null;
        gameObject.SetActive(false); 
    }
}