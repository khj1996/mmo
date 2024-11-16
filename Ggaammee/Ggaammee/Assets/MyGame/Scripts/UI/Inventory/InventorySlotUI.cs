using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : UI_Base, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image _icon = null;

    [SerializeField] Image _frame = null;

    [SerializeField] TMP_Text _quantity = null;

    private Item _item = null;

    public Action actionShortPress;

    private bool isInput = false;
    private bool startDrag = false;

    public override void Init()
    {
        _icon.gameObject.BindEvent(_ =>
        {
            if (startDrag || _item == null)
                return;
            if (_item == null)
            {
                Debug.Log("설명 뜨게 할수도 있음?");
            }
            else
            {
                _item.UseItem();
            }
        });
    }

    public void SetItem(Item item)
    {
        if (item == null)
        {
            _icon.gameObject.SetActive(false);
            _frame.gameObject.SetActive(false);
            _quantity.gameObject.SetActive(false);
        }
        else
        {
            _item = item;

            _icon.sprite = item.ItemSprite;

            _quantity.text = item.Count.ToString();

            _icon.gameObject.SetActive(true);
            _frame.gameObject.SetActive(item.Equipped);
            _quantity.gameObject.SetActive(_item.ItemType == ItemType.Usable);
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
        while (Input.GetMouseButton(0))
        {
            waitTime += Time.deltaTime;
            await UniTask.Yield();
            mousePos = Input.mousePosition;
            if ((startPos - mousePos).y > 30 || (startPos - mousePos).y < -30)
            {
                startDrag = true;
                break;
            }

            if (waitTime > 0.3f)
                break;
        }

        if (!startDrag)
        {
            if (waitTime < 0.3f)
            {
                actionShortPress?.Invoke();
            }
        }

        isInput = false;
    }

    public void OnBtnUp()
    {
        if (!isInput)
            return;


        startDrag = false;
        isInput = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnBtnDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnBtnUp();
    }
}