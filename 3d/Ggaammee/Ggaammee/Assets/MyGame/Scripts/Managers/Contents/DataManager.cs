using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    #region 데이터 초기화

    public bool IsInitialize = false;

    public Dictionary<string, ShopData> ShopDict { get; private set; } = new();
    public ItemDatas ItemDatas { get; private set; }
    public QuestDatas QuestDatas { get; private set; }

    public DataManager()
    {
        _ = Initialize();
    }


    private async UniTaskVoid Initialize()
    {
        var shopTask = LoadJsonAsync<ShopLoader, string, ShopData>("ShopData");
        var itemTask = LoadSOAsync<ItemDatas>("ItemDatas");
        var questTask = LoadSOAsync<QuestDatas>("QuestDatas");

        (ShopDict, ItemDatas, QuestDatas) = await UniTask.WhenAll(shopTask, itemTask, questTask);

        IsInitialize = true;
    }

    #endregion

    #region 데이터 로드

    private async UniTask<Dictionary<TKey, TValue>> LoadJsonAsync<TLoader, TKey, TValue>(string addressableKey) where TLoader : ILoader<TKey, TValue>
    {
        var textAsset = await Addressables.LoadAssetAsync<TextAsset>(addressableKey).ToUniTask();

        var loader = JsonConvert.DeserializeObject<TLoader>(textAsset.text);
        if (loader == null)
        {
            return new Dictionary<TKey, TValue>();
        }

        return loader.MakeDict();
    }

    private async UniTask<T> LoadSOAsync<T>(string addressableKey) where T : ScriptableObject
    {
        var scriptableObject = await Addressables.LoadAssetAsync<T>(addressableKey).ToUniTask();
        if (scriptableObject == null)
        {
            return null;
        }

        return scriptableObject;
    }

    #endregion
}