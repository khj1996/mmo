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
        SkillDict = (await LoadJson<Data.SkillDataLoader, int, Data.Skill>("Assets/Resources_moved/Data/SkillData.json")).MakeDict();
        ItemDict = (await LoadJson<Data.ItemLoader, int, Data.ItemData>("Assets/Resources_moved/Data/ItemData.json")).MakeDict();
        MonsterDict = (await LoadJson<Data.MonsterLoader, int, Data.MonsterData>("Assets/Resources_moved/Data/MonsterData.json")).MakeDict();
        ShopDict = (await LoadJson<Data.ShopLoader, int, Data.ShopData>("Assets/Resources_moved/Data/ShopData.json")).MakeDict();

        ItemImageSO = await LoadSO<ItemImageSO>("ItemImageSO");
    }

    async UniTask<Loader> LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = await Addressables.LoadAssetAsync<TextAsset>(path).ToUniTask();
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }

    async UniTask<T> LoadSO<T>(string name) where T : ScriptableObject
    {
        T sO = await Addressables.LoadAssetAsync<T>($"Assets/Resources_moved/ScriptableObject/{name}.asset").ToUniTask();
        return sO;
    }
}