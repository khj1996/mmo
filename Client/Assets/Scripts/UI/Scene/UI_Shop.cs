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

    private float currentYPos = 0f;

    public override void Init()
    {
        Shops.Clear();

        foreach (Transform child in scrollRect.content.transform)
            Destroy(child.gameObject);


        foreach (var shopData in Managers.Data.ShopDict)
        {
            var hCount = (shopData.Value.productList.Count / 3);
            //TODO : 스크립터블 오브젝트로 빼기 가능
            //스페이싱 + 마진 + 오브젝트 크기
            var addY = 50f + (hCount - 1f) * 15f + 163.3f * hCount;
            var goYpos = currentYPos + addY / 2f;

            var sc = Managers.UI.MakeSubItem<UI_Shop_Sub>(scrollRect.content.transform);
            var pos = sc.transform.position;
            //sc.transform.position = new Vector3(pos.x, pos.y + goYpos);

            sc.SetData(shopData.Value);

            Shops.Add(shopData.Key, sc);
            currentYPos += addY;
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