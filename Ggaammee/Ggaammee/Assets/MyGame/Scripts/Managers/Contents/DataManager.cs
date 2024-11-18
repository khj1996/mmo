using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

    public Dictionary<int, ItemData> ItemDict { get; private set; } = new();

    public Dictionary<int, MonsterData> MonsterDict { get; private set; } = new();

    public Dictionary<string, ShopData> ShopDict { get; private set; } = new();


    public DataManager()
    {
        ShopDict = LoadJson<ShopLoader, string, ShopData>("ShopData").MakeDict();
        Debug.Log(1);
    }


    public void Init()
    {
        //ItemImageSO = LoadSO<ItemImageSO>("ItemImageSO");
        //SkillDataContainer = LoadSO<SkillDataContainer>("SkillDataContainer");
    }

    Loader LoadJson<Loader, Key, Value>(string name) where Loader : ILoader<Key, Value>
    {
        var textAsset = Addressables.LoadAssetAsync<TextAsset>(name);

        var data = textAsset.WaitForCompletion();

        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(data.text);
    }

    /*T LoadSO<T>(string name) where T : ScriptableObject
    {
        //T sO = Util.HandleAndRelease<T>($"ScriptableObject/{name}.asset");
        //return sO;
    }*/

    #endregion
}