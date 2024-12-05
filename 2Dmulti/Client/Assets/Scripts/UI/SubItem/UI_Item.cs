using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Google.Protobuf.Protocol;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Item : UI_Base, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image _icon = null;

    [SerializeField] Image _frame = null;

    [SerializeField] TMP_Text _quantity = null;

    private Item _item = null;

    private bool isInput = false;
    private bool startDrag = false;

    public override void Init()
    {
        _icon.gameObject.BindEvent(_ =>
        {
            Debug.Log("아이템 클릭");
            if (startDrag || _item == null)
                return;
            if (_item == null)
            {
                Debug.Log("설명 뜨게 할수도 있음?");
            }
            else
            {
                 Managers.UI.ShowPopupUI<UI_UseItem>().OpenPopUp(_item);
            }
        });
    }

    public void SetItem(ItemData item, int itemCount)
    {
        if (item == null)
        {
            _icon.gameObject.SetActive(false);
            _frame.gameObject.SetActive(false);
            _quantity.gameObject.SetActive(false);
        }
        else
        {
            _icon.sprite = Managers.Data.ItemImageSO.ItemImageStructs.First(x => x.DataKey == item.id).Image;
            _quantity.text = itemCount.ToString();

            _icon.gameObject.SetActive(true);
            _frame.gameObject.SetActive(false);
            _quantity.gameObject.SetActive(itemCount != 1);
        }
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

            _icon.sprite = Managers.Data.ItemImageSO.ItemImageStructs.First(x => x.DataKey == item.TemplateId).Image;

            _quantity.text = item.Count.ToString();

            _icon.gameObject.SetActive(true);
            _frame.gameObject.SetActive(item.Equipped);
            _quantity.gameObject.SetActive(_item.ItemType == ItemType.Consumable);
        }
    }

    public async void OnBtnDown()
    {
        if (isInput)
            return;

        startDrag = false;
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