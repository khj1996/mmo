using UnityEngine;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour
{
    public Image itemIcon;

    public void Show(Sprite icon)
    {
        itemIcon.sprite = icon;
        itemIcon.enabled = true;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void Hide()
    {
        itemIcon.enabled = false;
    }
}