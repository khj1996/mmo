using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class ShopManager
{
    public string CurrentOpenShopId = "shop_000";
    
    private Dictionary<string, ShopData> shopDatas = new Dictionary<string, ShopData>();

    /// <summary>
    /// 특정 상점 데이터를 가져옵니다. 없을 경우 DataManager에서 로드합니다.
    /// </summary>
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

    /// <summary>
    /// 아이템 구매
    /// </summary>
    public bool BuyItem(string productId)
    {
        if (!TryGetProduct(CurrentOpenShopId, productId, out var product))
        {
            Debug.LogWarning($"Product not found: ShopID({CurrentOpenShopId}), ProductID({productId})");
            return false;
        }

        var itemData = Managers.DataManager.ItemDatas.GetData(product.itemId);
        if (!CanBuyItem(product, itemData, out int availableSlot))
        {
            Debug.LogWarning($"Cannot buy item: ProductID({productId})");
            return false;
        }

        // 구매 처리
        Managers.InventoryManager.UseCurrency(product.priceId, product.price);
        Managers.InventoryManager.Add(itemData, 1);

        return true;
    }

    /// <summary>
    /// 아이템 판매
    /// </summary>
    public void SellItem(Item item, int count)
    {
        // 구현 필요
        Debug.Log($"SellItem: {item?.Data?.name}, Count: {count}");
    }

    /// <summary>
    /// 특정 상품을 가져옵니다.
    /// </summary>
    private bool TryGetProduct(string shopId, string productId, out ProductData product)
    {
        product = null;

        var shopData = GetShopData(shopId);
        if (shopData == null)
        {
            Debug.LogWarning($"Shop not found: ShopID({shopId})");
            return false;
        }

        product = shopData.productList.FirstOrDefault(x => x.productId == productId);
        return product != null;
    }

    /// <summary>
    /// 아이템 구매 가능 여부를 확인합니다.
    /// </summary>
    private bool CanBuyItem(ProductData product, ItemData itemData, out int availableSlot)
    {
        availableSlot = -1;

        if (product == null || itemData == null)
        {
            Debug.LogWarning("Invalid product or item data.");
            return false;
        }

        // 가격 확인
        if (!Managers.InventoryManager.CheckCurrencyCost(product.priceId, product.price))
        {
            Debug.LogWarning($"Not enough currency: PriceID({product.priceId}), Price({product.price})");
            return false;
        }

        // 인벤토리 슬롯 확인
        availableSlot = Managers.InventoryManager.GetAvailableSlot(itemData);
        if (availableSlot == -1)
        {
            Debug.LogWarning("No available inventory slot.");
            return false;
        }

        return true;
    }
}
