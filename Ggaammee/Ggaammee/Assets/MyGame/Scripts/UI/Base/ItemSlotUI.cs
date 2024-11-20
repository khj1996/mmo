using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : UI_Base
{
    [Header("UI Elements")] [SerializeField]
    private Image _icon;

    [SerializeField] private Image _frame;
    [SerializeField] private TMP_Text _quantity;

    [Header("Item Data")] private Item _item;
    public Item Item => _item;
    public int Index { get; private set; } = -1;


    public RectTransform Rect => GetComponent<RectTransform>();

    public event Action OnShortPress;
    public event Action<ItemSlotUI, ItemSlotUI> OnDropItem;

    public override void Init()
    {
        OnShortPress += HandleShortPress;
    }

    private void HandleShortPress()
    {
        if (_item == null) return;

        if (_item is UsableItem usableItem)
        {
            usableItem.Use();
        }
    }

    public bool HasItem() => _item != null;

    public void UseItem()
    {
        OnShortPress?.Invoke();
    }

    public void SetItem(Item item)
    {
        if (_item == item) return;

        _item = item;
        if (_item == null)
        {
            ResetUI();
        }
        else
        {
            UpdateUI();
        }
    }


    public void SetItem(Item item, int index)
    {
        Index = index;
        _item = item;

        if (_item == null)
        {
            ResetUI();
        }
        else
        {
            UpdateUI();
        }
    }

    public void RefreshCount()
    {
        if (_item is StackableItem stackableItem)
        {
            _quantity.text = stackableItem.Count.ToString();
            _quantity.gameObject.SetActive(true);
        }
    }

    private void ResetUI()
    {
        _icon?.gameObject.SetActive(false);
        _frame?.gameObject.SetActive(false);
        _quantity?.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        if (_item is StackableItem oldStackableItem)
        {
            oldStackableItem.OnChangeCount -= RefreshCount;
        }

        _icon.sprite = _item.Data.itemSprite;
        _icon.gameObject.SetActive(true);

        if (_item is StackableItem stackableItem)
        {
            _quantity.text = stackableItem.Count.ToString();
            _quantity.gameObject.SetActive(true);
            stackableItem.OnChangeCount -= RefreshCount;
            stackableItem.OnChangeCount += RefreshCount;
        }
        else
        {
            _quantity.gameObject.SetActive(false);
        }
    }


    public void HandleDrop(ItemSlotUI itemSlotUI)
    {
        if (_item != null && itemSlotUI.HasItem())
        {
            Debug.Log("Dropping item from slot " + itemSlotUI.Index + " to " + Index);
        }

        OnDropItem?.Invoke(itemSlotUI, this);
    }
}