using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

public class UI_Shop_Sub : UI_Base
{
    public List<UI_Shop_Product> Products { get; } = new List<UI_Shop_Product>();

    private ShopData shopdata;

    public override void Init()
    {
        Products.Clear();
    }

    public void SetData(ShopData _shopData)
    {   
        shopdata = _shopData;

        GameObject grid = transform.Find("Grid").gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        /*
        var gridGroup = grid.GetComponent<GridLayout>();
        var width = transform.GetComponent<RectTransform>().sizeDelta.*/


        foreach (var productData in _shopData.productList)
        {
            var sc = Managers.UI.MakeSubItem<UI_Shop_Product>(grid.transform);
            sc.gameObject.transform.localScale = Vector3.one;

            sc.SetData(productData);

            Products.Add(sc);
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < Products.Count; i++)
        {
            Products[i].SetData(shopdata.productList[i]);
        }
    }
}