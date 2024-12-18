﻿using System.Linq;
using Google.Protobuf.Protocol;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Shop_Product : UI_Base
{
    private ProductData productInfo;
    private int shopId;

    [SerializeField] UI_Item _item;

    [SerializeField] TMP_Text _itemCost = null;
    [SerializeField] TMP_Text _name = null;
    [SerializeField] Image _costImg = null;

    public override void Init()
    {
        gameObject.BindEvent(Buyitem);
    }

    public void SetData(int _shopId, ProductData _productData)
    {
        productInfo = _productData;
        shopId = _shopId;

        if (Managers.Data.ItemDict.TryGetValue(productInfo.pId, out ItemData item))
        {
            _itemCost.text = productInfo.cAmount.ToString();
            _name.text = item.name;
            _costImg.sprite = Managers.Data.ItemImageSO.ItemImageStructs.First(x => x.DataKey == _productData.cType).Image;
            _item.SetItem(item, productInfo.quantity);
        }
    }

    void Buyitem(PointerEventData evt)
    {
        C_BuyItem buyPacket = new C_BuyItem()
        {
            Shop = shopId,
            ProductId = productInfo.id,
            JwtToken = Managers.Network.Token
        };

        Managers.Network.Send(buyPacket);

    }
}