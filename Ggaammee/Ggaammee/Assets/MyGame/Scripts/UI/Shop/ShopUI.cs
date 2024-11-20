using System;
using Data;
using TMPro;
using UnityEngine;

public class ShopUI : UI_ScrollView<ShopUISub>
{
    public string currentShopId = "shop_000";

    private ShopData shopData;

    public override void Init()
    {
        base.Init();
        gameObject.SetActive(false);
    }

    
    public void RefreshShop()
    {
        shopData = Managers.ShopManager.GetShopData(currentShopId);
        maxIndex = shopData.productList.Count;
        InitializeView();
    }

    protected override void UpdateItemForIndex(UI_ScrollView_Sub item, int _index)
    {
        ((ShopUISub)item).productData = shopData.productList[_index];
        base.UpdateItemForIndex(item, _index);
    }
}