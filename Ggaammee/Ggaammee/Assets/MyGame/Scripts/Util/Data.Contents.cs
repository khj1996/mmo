using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.Serialization;

namespace Data
{
    #region Shop

    [Serializable]
    public class ShopData
    {
        public string shopId;
        public List<ProductData> productList;
    }

    [Serializable]
    public class ProductData
    {
        public string productId;
        public string itemId;
        public int price;
        public string priceId;
    }


    [Serializable]
    public class ShopLoader : ILoader<string, ShopData>
    {
        public List<ShopData> shops = new List<ShopData>();

        public Dictionary<string, ShopData> MakeDict()
        {
            Dictionary<string, ShopData> dict = new Dictionary<string, ShopData>();
            foreach (ShopData shop in shops)
            {
                dict.Add(shop.shopId, shop);
            }

            return dict;
        }
    }

    #endregion

    /*#region Quest

    [System.Serializable]
    public class QuestData
    {
        public string id;
        public string title;
        public string description;
        public List<Objective> objectives;
        public Rewards rewards;
    }

    [System.Serializable]
    public class Objective
    {
        public string type;
        public string targetId;
        public int count;
        public Vector3 coordinates; // 목적지 좌표
    }

    [System.Serializable]
    public class Rewards
    {
        public int experience;
        public int gold;
        public List<RewardItem> items;
    }

    [System.Serializable]
    public class RewardItem
    {
        public string itemId;
        public int quantity;
    }

    [Serializable]
    public class QuestLoader : ILoader<string, QuestData>
    {
        public List<QuestData> quests = new List<QuestData>();

        public Dictionary<string, QuestData> MakeDict()
        {
            Dictionary<string, QuestData> dict = new Dictionary<string, QuestData>();
            foreach (var quest in quests)
            {
                dict.Add(quest.id, quest);
            }

            return dict;
        }
    }

    #endregion*/
}