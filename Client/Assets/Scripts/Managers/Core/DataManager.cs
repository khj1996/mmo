using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.Skill> SkillDict { get; private set; } = new Dictionary<int, Data.Skill>();
    public Dictionary<int, Data.ItemData> ItemDict { get; private set; } = new Dictionary<int, Data.ItemData>();

    public Dictionary<int, Data.MonsterData> MonsterDict { get; private set; } = new Dictionary<int, Data.MonsterData>();

    public Dictionary<int, Data.ShopData> ShopDict { get; private set; } = new Dictionary<int, Data.ShopData>();

    public ItemImageSO ItemImageSO { get; private set; }

    public async void Init()
    {
        ItemDict = LoadJson<Data.ItemLoader, int, Data.ItemData>("ItemData").MakeDict();
        SkillDict = LoadJson<Data.SkillDataLoader, int, Data.Skill>("SkillData").MakeDict();
        MonsterDict = LoadJson<Data.MonsterLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        ShopDict = LoadJson<Data.ShopLoader, int, Data.ShopData>("ShopData").MakeDict();

        ItemImageSO = await LoadSO<ItemImageSO>("ItemImageSO");
    }

    Loader LoadJson<Loader, Key, Value>(string name) where Loader : ILoader<Key, Value>
    {
        var handleTextAsset = Addressables.LoadAssetAsync<TextAsset>($"Data/{name}.json");

        var result = handleTextAsset.WaitForCompletion();

        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(result.text);
    }

    async UniTask<T> LoadSO<T>(string name) where T : ScriptableObject
    {
        T sO = await Addressables.LoadAssetAsync<T>($"ScriptableObject/{name}.asset").ToUniTask();
        return sO;
    }
}