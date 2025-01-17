using System.Linq;
using Data;
using UnityEngine;

public class ShopUI : UI_ScrollView<ShopUISub>
{
    private ShopData shopData;

    public override void Init()
    {
        base.Init();
        Managers.ShopManager.shopUI = this;
        PopupUIManager.Instance.popList.First(p => p.UI.name == gameObject.name).OnOpen += OpenShop;
        gameObject.SetActive(false);
    }

    private void OpenShop(string shopId)
    {
        if (string.IsNullOrEmpty(shopId))
        {
            return;
        }

        shopData = Managers.ShopManager.GetShopData(shopId);
        RefreshShop();
    }

    public void RefreshShop()
    {
        maxIndex = shopData.productList.Count;
        InitializeView();
    }

    protected override void UpdateItemForIndex(UI_ScrollView_Sub item, int index)
    {
        var shopItem = (ShopUISub)item;
        shopItem.productData = shopData.productList[index];
        base.UpdateItemForIndex(item, index);
    }
}