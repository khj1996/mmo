using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : UI_Base, IPointerDownHandler
{
    [SerializeField] Image _icon = null;
    [SerializeField] Image _frame = null;
    [SerializeField] TMP_Text _quantity = null;

    private Item _item = null;

    public Item Item => _item;

    public int index = -1;

    public RectTransform _rect => GetComponent<RectTransform>();


    public Action actionLongPress;
    public Action actionShortPress;
    public Action actionBeginDrag;

    private bool isInput = false;
    private bool startDrag = false;

    public override void Init()
    {
        actionShortPress += UseItem;
    }

    public bool HasItem()
    {
        return (_item != null);
    }

    public void UseItem()
    {
        if (_item == null)
            return;

        if (_item is UsableItem usableItem)
        {
            usableItem.Use();
        }
    }

    public void SetItem(Item item)
    {
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


    public async void OnBtnDown()
    {
        if (isInput)
            return;

        bool startDrag = false;
        isInput = true;
        float waitTime = 0;
        var startPos = Input.mousePosition;
        Vector3 mousePos;
        bool actionPress = false;
        while (Input.GetMouseButton(0))
        {
            waitTime += Time.deltaTime;
            await UniTask.Yield();
            mousePos = Input.mousePosition;
            if (Vector3.Distance(startPos, mousePos) > 30)
            {
                if (!startDrag)
                {
                    actionBeginDrag?.Invoke();
                    startDrag = true;
                }

                break;
            }

            if (waitTime > 0.3f && !actionPress)
            {
                actionPress = true;
                actionLongPress?.Invoke();
            }
        }

        if (!startDrag && !actionPress)
        {
            if (waitTime >= 0.3f)
            {
                actionLongPress?.Invoke();
            }
            else
            {
                actionShortPress?.Invoke();
            }
        }

        isInput = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnBtnDown();
    }
}