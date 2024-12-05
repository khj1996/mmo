using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : UI_Base
{
    public ScrollRect scrollRect;

    public Dictionary<int, UI_Shop_Sub> Shops { get; } = new Dictionary<int, UI_Shop_Sub>();


    public override void Init()
    {
        Shops.Clear();

        foreach (Transform child in scrollRect.content.transform)
            Destroy(child.gameObject);


        foreach (var shopData in Managers.Data.ShopDict)
        {

            var sc = Managers.UI.MakeSubItem<UI_Shop_Sub>(scrollRect.content.transform);
            sc.gameObject.transform.localScale = Vector3.one;
            sc.SetData(shopData.Value);
            Shops.Add(shopData.Key, sc);
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (var shop in Shops)
        {
            shop.Value.RefreshUI();
        }
    }
}