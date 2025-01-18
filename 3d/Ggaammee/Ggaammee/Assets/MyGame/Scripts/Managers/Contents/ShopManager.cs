using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class ShopManager
{
    public ShopUI shopUI;
    public string CurrentOpenShopId = "shop_000";

    private Dictionary<string, ShopData> shopDatas = new Dictionary<string, ShopData>();

    public ShopData GetShopData(string shopId)
    {
        if (!shopDatas.TryGetValue(shopId, out var shopData))
        {
            if (Managers.DataManager.ShopDict.TryGetValue(shopId, out shopData))
            {
                shopDatas[shopId] = shopData;
            }
        }

        CurrentOpenShopId = shopId;

        return shopData;
    }

    public bool BuyItem(string productId)
    {
        if (!TryGetProduct(CurrentOpenShopId, productId, out var product))
        {
            return false;
        }

        var itemData = Managers.DataManager.ItemDatas.GetData(product.itemId);
        if (!CanBuyItem(product, itemData, out int availableSlot))
        {
            return false;
        }

        Managers.InventoryManager.UseCurrency(product.priceId, product.price);
        Managers.InventoryManager.Add(itemData, 1);

        return true;
    }

    private bool TryGetProduct(string shopId, string productId, out ProductData product)
    {
        product = null;

        var shopData = GetShopData(shopId);
        if (shopData == null)
        {
            return false;
        }

        product = shopData.productList.FirstOrDefault(x => x.productId == productId);
        return product != null;
    }

    private bool CanBuyItem(ProductData product, ItemData itemData, out int availableSlot)
    {
        availableSlot = -1;

        if (product == null || itemData == null)
        {
            return false;
        }

        if (!Managers.InventoryManager.CheckCurrencyCost(product.priceId, product.price))
        {
            return false;
        }

        availableSlot = Managers.InventoryManager.GetAvailableSlot(itemData);
        if (availableSlot == -1)
        {
            return false;
        }

        return true;
    }
}