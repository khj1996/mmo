using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Data;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    #region 데이터 초기화

    public static Dictionary<int, StatInfo> StatDict { get; private set; } = new Dictionary<int, StatInfo>();
    public Dictionary<int, Data.Skill> SkillDict { get; private set; } = new Dictionary<int, Data.Skill>();
    public Dictionary<int, Data.ItemData> ItemDict { get; private set; } = new Dictionary<int, Data.ItemData>();

    public Dictionary<int, Data.MonsterData> MonsterDict { get; private set; } = new Dictionary<int, Data.MonsterData>();

    public Dictionary<int, Data.ShopData> ShopDict { get; private set; } = new Dictionary<int, Data.ShopData>();

    public ItemImageSO ItemImageSO { get; private set; }

    public void Init()
    {
        StatDict = LoadJson<StatDataLoader, int, StatInfo>("StatData").MakeDict();
        ItemDict = LoadJson<Data.ItemLoader, int, Data.ItemData>("ItemData").MakeDict();
        SkillDict = LoadJson<Data.SkillDataLoader, int, Data.Skill>("SkillData").MakeDict();
        MonsterDict = LoadJson<Data.MonsterLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        ShopDict = LoadJson<Data.ShopLoader, int, Data.ShopData>("ShopData").MakeDict();

        ItemImageSO = LoadSO<ItemImageSO>("ItemImageSO");
    }

    Loader LoadJson<Loader, Key, Value>(string name) where Loader : ILoader<Key, Value>
    {
        var textAsset = Util.HandleAndRelease<TextAsset>($"Data/{name}.json");

        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }

    #endregion

    T LoadSO<T>(string name) where T : ScriptableObject
    {
        T sO = Util.HandleAndRelease<T>($"ScriptableObject/{name}.asset");
        return sO;
    }
    public (int currentLevel, int expToNextLevel, int currentExpInLevel) GetCurrentLevelData(int totalExp)
    {
        int currentLevel = 1;
        int expToNextLevel = 0;
        int currentExpInLevel = 0;

        if (StatDict == null || StatDict.Count == 0)
            return (currentLevel, expToNextLevel, currentExpInLevel);

        foreach (var statEntry in StatDict)
        {
            StatInfo statInfo = statEntry.Value;

            if (totalExp < statInfo.TotalExp)
            {
                expToNextLevel = statInfo.TotalExp - StatDict[currentLevel].TotalExp;
                currentExpInLevel = totalExp - StatDict[currentLevel].TotalExp;
                return (currentLevel, expToNextLevel, currentExpInLevel);
            }

            currentLevel = statInfo.Level;
        }

        expToNextLevel = 0;
        currentExpInLevel = totalExp - StatDict[currentLevel].TotalExp;

        return (currentLevel, expToNextLevel, currentExpInLevel);
    }

}