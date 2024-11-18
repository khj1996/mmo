using System;
using System.Collections.Generic;

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
        public string itemId;
        public int price;
        public int stock;
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
                dict.Add(shop.shopId, shop);
            }

            return dict;
        }
    }

    #endregion
}