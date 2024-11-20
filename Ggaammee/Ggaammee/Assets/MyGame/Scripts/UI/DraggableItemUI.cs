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
        gameObject.SetActive(false); // 초기 비활성화
    }

    public void StartDrag(ItemSlotUI data)
    {
        Image.sprite = data.Item.Data.itemSprite;
        _beginDragIconPoint = data.Rect.transform.position;
        _beginDragCursorPoint = Input.mousePosition;

        gameObject.SetActive(true); // 드래그 오브젝트 활성화
    }

    public void OnDrag()
    {
        RectTransform.position = _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
    }

    public void EndDrag()
    {
        Image.sprite = null;
        gameObject.SetActive(false); // 드래그 오브젝트 비활성화
    }
}