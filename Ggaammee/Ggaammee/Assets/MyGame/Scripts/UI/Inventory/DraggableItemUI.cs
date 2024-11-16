using UnityEngine;
using UnityEngine.UI;

public class DraggableItem : Singleton<DraggableItem>
{
    public bool isDragItem =false;
    public Image _image;
    private RectTransform _rectTransform;

    private Item tmpItem;

    public void StartDrag(Item item)
    {
        transform.position = Input.mousePosition;
        _image.gameObject.SetActive(true);
        _image.sprite = item.Data.itemSprite;
    }
    
    public void EndDrag()
    {
        _image.sprite = null;
        _image.gameObject.SetActive(false);
    }
}