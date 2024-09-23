using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class UI_Shop : UI_Base
{
    public List<UI_Shop_Item> Products { get; } = new List<UI_Shop_Item>();

    private ShopData shopdata;

    public override void Init()
    {
        Products.Clear();

        GameObject grid = transform.Find("ItemGrid").gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        Managers.Data.ShopDict.TryGetValue(1, out shopdata);

        for (int i = 0; i < shopdata.productList.Count; i++)
        {
            UI_Shop_Item item = Managers.UI.MakeSubItem<UI_Shop_Item>(grid.transform);
            Products.Add(item);
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < Products.Count; i++)
        {
            Products[i].SetData(shopdata.productList[i]);
        }
    }
}