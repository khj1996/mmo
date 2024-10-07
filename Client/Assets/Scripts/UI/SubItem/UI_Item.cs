using System.Linq;
using Google.Protobuf.Protocol;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Item : UI_Base
{
    [SerializeField] Image _icon = null;

    [SerializeField] Image _frame = null;

    [SerializeField] TMP_Text _quantity = null;

    public int ItemDbId { get; private set; }
    public int TemplateId { get; private set; }
    public int Count { get; private set; }
    public bool Equipped { get; private set; }

    public override void Init()
    {
        _icon.gameObject.BindEvent((e) =>
        {
            Debug.Log("Click Item");

            Managers.Data.ItemDict.TryGetValue(TemplateId, out var itemData);
            if (itemData == null)
                return;
            if (itemData.itemType == ItemType.Currency)
                return;
            // TODO : C_USE_ITEM 아이템 사용 패킷
            if (itemData.itemType == ItemType.Consumable)
                return;

            C_EquipItem equipPacket = new C_EquipItem();
            equipPacket.ItemDbId = ItemDbId;
            equipPacket.Equipped = !Equipped;

            Managers.Network.Send(equipPacket);
        });
    }

    public void SetItem(ItemData item, int itemCount)
    {
        if (item == null)
        {
            ItemDbId = 0;
            TemplateId = 0;
            Count = 0;
            Equipped = false;

            _icon.gameObject.SetActive(false);
            _frame.gameObject.SetActive(false);
            _quantity.gameObject.SetActive(false);
        }
        else
        {
            TemplateId = item.id;
            Count = itemCount;


            _icon.sprite = Managers.Data.ItemImageSO.ItemImageStructs.First(x => x.DataKey == TemplateId).Image;
            _quantity.text = Count.ToString();

            _icon.gameObject.SetActive(true);
            _frame.gameObject.SetActive(false);
            _quantity.gameObject.SetActive(Count != 1);
        }
    }

    public void SetItem(Item item)
    {
        if (item == null)
        {
            ItemDbId = 0;
            TemplateId = 0;
            Count = 0;
            Equipped = false;

            _icon.gameObject.SetActive(false);
            _frame.gameObject.SetActive(false);
            _quantity.gameObject.SetActive(false);
        }
        else
        {
            ItemDbId = item.ItemDbId;
            TemplateId = item.TemplateId;
            Count = item.Count;
            Equipped = item.Equipped;

            Managers.Data.ItemDict.TryGetValue(TemplateId, out ItemData itemData);

            _icon.sprite = Managers.Data.ItemImageSO.ItemImageStructs.First(x => x.DataKey == TemplateId).Image;

            _quantity.text = Count.ToString();

            _icon.gameObject.SetActive(true);
            _frame.gameObject.SetActive(Equipped);
            _quantity.gameObject.SetActive(itemData.itemType == ItemType.Consumable);
        }
    }
}