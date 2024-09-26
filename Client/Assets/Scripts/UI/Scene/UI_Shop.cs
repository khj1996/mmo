using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

public class UI_Shop : UI_Base
{
    public List<UI_Shop_Item> Products { get; } = new List<UI_Shop_Item>();

    private ShopData shopdata;

    public override async void Init()
    {
        Products.Clear();

        GameObject grid = transform.Find("ItemGrid").gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        Managers.Data.ShopDict.TryGetValue(1, out shopdata);

        var newTask = new List<UniTask<UI_Shop_Item>>();

        for (int i = 0; i < shopdata.productList.Count; i++)
        {
            newTask.Add(Managers.UI.MakeSubItem<UI_Shop_Item>(grid.transform));
        }

        Products.AddRange(await UniTask.WhenAll(newTask));

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