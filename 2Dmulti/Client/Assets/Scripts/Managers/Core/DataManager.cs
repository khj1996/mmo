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

    public static Dictionary<int, StatInfo> StatDict { get; private set; } = new();
    public Dictionary<int, Skill> SkillDict { get; private set; } = new();
    public Dictionary<int, ItemData> ItemDict { get; private set; } = new();

    public Dictionary<int, MonsterData> MonsterDict { get; private set; } = new();

    public Dictionary<int, ShopData> ShopDict { get; private set; } = new();

    public ItemImageSO ItemImageSO { get; private set; }

    public SkillDataContainer SkillDataContainer { get; private set; }

    public void Init()
    {
        StatDict = LoadJson<StatDataLoader, int, StatInfo>("StatData").MakeDict();
        ItemDict = LoadJson<ItemLoader, int, ItemData>("ItemData").MakeDict();
        SkillDict = LoadJson<SkillDataLoader, int, Skill>("SkillData").MakeDict();
        MonsterDict = LoadJson<MonsterLoader, int, MonsterData>("MonsterData").MakeDict();
        ShopDict = LoadJson<ShopLoader, int, ShopData>("ShopData").MakeDict();

        ItemImageSO = LoadSO<ItemImageSO>("ItemImageSO");
        SkillDataContainer = LoadSO<SkillDataContainer>("SkillDataContainer");
    }

    Loader LoadJson<Loader, Key, Value>(string name) where Loader : ILoader<Key, Value>
    {
        var textAsset = Util.HandleAndRelease<TextAsset>($"Data/{name}.json");

        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }

    T LoadSO<T>(string name) where T : ScriptableObject
    {
        T sO = Util.HandleAndRelease<T>($"ScriptableObject/{name}.asset");
        return sO;
    }

    #endregion


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