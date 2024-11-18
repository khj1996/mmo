using System.Collections.Generic;
using UnityEngine;

public class ShopManager
{
    // 상점 상품 리스트
    public List<ShopItemData> shopItems = new List<ShopItemData>();

    // 플레이어의 금액 (예시)
    private int playerGold = 1000;

    // 상점에서 구매할 수 있는 아이템
    public void InitializeShop()
    {
        // 상점에 아이템 추가
        shopItems.Add(new ShopItemData("Sword", 100, "A sharp sword"));
        shopItems.Add(new ShopItemData("Shield", 150, "A strong shield"));
        shopItems.Add(new ShopItemData("Potion", 50, "A healing potion"));
    }

    // 아이템 구매
    public bool BuyItem(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= shopItems.Count)
            return false;

        ShopItemData item = shopItems[itemIndex];

        // 금액이 충분한지 체크
        if (playerGold >= item.price)
        {
            playerGold -= item.price;
            Debug.Log($"Bought {item.name} for {item.price} gold.");
            return true;
        }

        Debug.Log("Not enough gold.");
        return false;
    }

    // 아이템 판매
    public void SellItem(ShopItemData item)
    {
        playerGold += item.price;
        Debug.Log($"Sold {item.name} for {item.price} gold.");
    }

    // 상점 상품 반환
    public List<ShopItemData> GetShopItems()
    {
        return shopItems;
    }

    // 플레이어의 금액 반환
    public int GetPlayerGold()
    {
        return playerGold;
    }
}

[System.Serializable]
public class ShopItemData
{
    public string name;
    public int price;
    public string description;

    public ShopItemData(string name, int price, string description)
    {
        this.name = name;
        this.price = price;
        this.description = description;
    }
}