using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDatas : ScriptableObject
{
    [SerializeField] private List<ItemData> itemDatas;

    /*public ItemData GetData(string itemId)
    {
        itemDatas.First(x=>x.id == itemId)
    }*/
}