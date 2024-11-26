using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Data
{
    #region Shop

    [Serializable]
    public class ShopData
    {
        public string ShopId;
        public List<ProductData> ProductList;
    }

    [Serializable]
    public class ProductData
    {
        public string ProductId;
        public string ItemId;
        public int Price;
        public string PriceId;
    }


    [Serializable]
    public class ShopLoader : ILoader<string, ShopData>
    {
        public List<ShopData> Shops = new List<ShopData>();

        public Dictionary<string, ShopData> MakeDict()
        {
            Dictionary<string, ShopData> dict = new Dictionary<string, ShopData>();
            foreach (ShopData shop in Shops)
            {
                dict.Add(shop.ShopId, shop);
            }

            return dict;
        }
    }

    #endregion
}