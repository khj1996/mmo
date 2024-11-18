using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : UI_Base
{
    [SerializeField] Image _icon = null;
    [SerializeField] Image _frame = null;
    [SerializeField] TMP_Text _quantity = null;

    private Item _item = null;

    public Item Item => _item;

    public int index = -1;

    public RectTransform _rect => GetComponent<RectTransform>();


    public event Action actionShortPress;

    private bool isInput = false;
    private bool startDrag = false;

    public override void Init()
    {
        actionShortPress += () =>
        {
            if (_item == null)
                return;

            if (_item is UsableItem usableItem)
            {
                usableItem.Use();
            }
        };
    }

    public bool HasItem()
    {
        return (_item != null);
    }

    public void UseItem()
    {
        actionShortPress?.Invoke();
    }


    public void SetItem(Item item, int _index)
    {
        index = _index;
        _item = item;
        if (item == null)
        {
            _icon.gameObject.SetActive(false);
            _frame.gameObject.SetActive(false);
            _quantity.gameObject.SetActive(false);
        }
        else
        {
            _icon.sprite = item.Data.itemSprite;
            if (item is StackableItem ci)
            {
                _quantity.text = ci.Count.ToString();
                _quantity.gameObject.SetActive(true);
            }


            _icon.gameObject.SetActive(true);
            //_frame.gameObject.SetActive(item.Equipped);
        }
    }
}