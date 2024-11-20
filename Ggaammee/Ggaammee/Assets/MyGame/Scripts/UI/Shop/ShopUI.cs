using System;
using Data;
using TMPro;
using UnityEngine;

public class ShopUI : UI_ScrollView<ShopUISub>
{
    public TextMeshProUGUI goldText; // 골드를 표시할 텍스트


    public string currentShopId = "shop_000";

    private ShopData shopData;

    public override void Init()
    {
        base.Init();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (Managers.Instance.isInit)
            RefreshShop();
    }

    public void RefreshShop()
    {
        shopData = Managers.ShopManager.GetShopData(currentShopId);
        maxIndex = shopData.productList.Count;
        InitializeView();
    }

    // 골드 UI를 갱신하는 메서드
    private void UpdateGoldUI(int newGoldAmount)
    {
        goldText.text = $"Gold: {newGoldAmount}";
    }


    protected override void UpdateItemForIndex(UI_ScrollView_Sub item, int _index)
    {
        ((ShopUISub)item).productData = shopData.productList[_index];
        base.UpdateItemForIndex(item, _index);
    }
}