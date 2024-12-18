﻿using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    #region 데이터 초기화

    public Dictionary<string, ShopData> ShopDict { get; private set; } = new();
    //public Dictionary<string, QuestData> QuestDict { get; private set; } = new();

    public ItemDatas ItemDatas;
    public QuestDatas QuestDatas;

    public DataManager()
    {
        ShopDict = LoadJson<ShopLoader, string, ShopData>("ShopData").MakeDict();
        //QuestDict = LoadJson<QuestLoader, string, QuestData>("QuestData").MakeDict();
        ItemDatas = LoadSO<ItemDatas>("ItemDatas");
        QuestDatas = LoadSO<QuestDatas>("QuestDatas");
    }

    Loader LoadJson<Loader, Key, Value>(string name) where Loader : ILoader<Key, Value>
    {
        var textAsset = Addressables.LoadAssetAsync<TextAsset>(name);

        var data = textAsset.WaitForCompletion();

        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(data.text);
    }

    T LoadSO<T>(string name) where T : ScriptableObject
    {
        var sO = Addressables.LoadAssetAsync<T>(name);
        var data = sO.WaitForCompletion();
        return data;
    }

    #endregion
}