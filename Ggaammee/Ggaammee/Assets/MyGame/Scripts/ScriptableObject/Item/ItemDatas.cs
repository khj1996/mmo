using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatas", menuName = "Inventory/ItemDatas")]
public class ItemDatas : ScriptableObject
{
    [SerializeField] private List<ItemData> itemDatas;

    public ItemData GetData(string itemId)
    {
        var itemData = itemDatas.FirstOrDefault(x => x.id == itemId);

        return itemData;
    }
}