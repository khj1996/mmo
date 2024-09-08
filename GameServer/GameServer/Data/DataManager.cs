using System.Collections.Generic;
using System.IO;
using GameServer.DB;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Data
{
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
    }

    public class DataManager
    {
        public static Dictionary<int, StatInfo> StatDict { get; private set; } = new Dictionary<int, StatInfo>();
        public static Dictionary<int, Skill> SkillDict { get; private set; } = new Dictionary<int, Data.Skill>();
        public static Dictionary<int, ItemData> ItemDict { get; private set; } = new Dictionary<int, Data.ItemData>();

        public static Dictionary<int, MonsterData> MonsterDict { get; private set; } =
            new Dictionary<int, MonsterData>();

        public static void LoadData()
        {
            StatDict = LoadJson<StatData, int, StatInfo>("StatData").MakeDict();
            SkillDict = LoadJson<SkillData, int, Skill>("SkillData").MakeDict();
            LoadItemData();
            LoadMonsterData();
        }

        public static void LoadMonsterData()
        {
            using AppDbContext db = new AppDbContext();


            List<DB.MonsterDataDb> monsterDatas =
                db.MonsterDatas.Include(x => x.rewards).Where(x => x.MonsterDataDbid != -1).ToList();

            if (monsterDatas.Count == 0)
            {
                MonsterDict = LoadJson<MonsterLoader, int, MonsterData>("MonsterData").MakeDict();

                foreach (var monsterDataKp in MonsterDict)
                {
                    var newMonsterData = new MonsterDataDb()
                    {
                        MonsterDataDbid = monsterDataKp.Key,
                        name = monsterDataKp.Value.name,
                        Level = monsterDataKp.Value.stat.Level,
                        MaxHp = monsterDataKp.Value.stat.MaxHp,
                        Attack = monsterDataKp.Value.stat.Attack,
                        Speed = monsterDataKp.Value.stat.Speed,
                        TotalExp = monsterDataKp.Value.stat.TotalExp,
                        rewards = new List<DB.RewardDataDb>()
                    };

                    foreach (var rewardData in monsterDataKp.Value.rewards)
                    {
                        var newRewardData = new RewardDataDb()
                        {
                            count = rewardData.count,
                            probability = rewardData.probability,
                            itemId = rewardData.itemId,
                            OwnerDbId = monsterDataKp.Key
                        };
                        db.RewardDatas.Add(newRewardData);
                    }

                    db.MonsterDatas.Add(newMonsterData);
                }

                bool success = db.SaveChangesEx();

                if (success == false)
                    return;
            }
            else
            {
                foreach (var monsterData in monsterDatas)
                {
                    var newMonsterData = new MonsterData()
                    {
                        name = monsterData.name,
                        rewards = new List<RewardData>(),
                        stat = new StatInfo()
                        {
                            Level = monsterData.Level,
                            Attack = monsterData.Attack,
                            TotalExp = monsterData.TotalExp,
                            MaxHp = monsterData.MaxHp,
                            Speed = monsterData.Speed,
                        },
                        id = monsterData.MonsterDataDbid,
                    };
                    foreach (var reward in monsterData.rewards)
                    {
                        newMonsterData.rewards.Add(new RewardData()
                        {
                            count = reward.count,
                            probability = reward.probability,
                            itemId = reward.itemId
                        });
                    }

                    MonsterDict.Add(monsterData.MonsterDataDbid, newMonsterData);
                }
            }
        }

        public static void LoadItemData()
        {
            using AppDbContext db = new AppDbContext();


            List<ItemDataDb> itemDatas =
                db.ItemDatas.Where(x => x.ItemDataDbid != -1).ToList();

            if (itemDatas.Count == 0)
            {
                ItemDict = LoadJson<ItemLoader, int, ItemData>("ItemData").MakeDict();

                foreach (var itemDataKp in ItemDict)
                {
                    var newItemData = new ItemDataDb()
                    {
                        name = itemDataKp.Value.name,
                        ItemDataDbid = itemDataKp.Key,
                    };

                    int type = newItemData.ItemDataDbid / 100000;

                    switch (type)
                    {
                        case 1:
                            newItemData.type = (int)((WeaponData)itemDataKp.Value).weaponType;
                            newItemData.value = ((WeaponData)itemDataKp.Value).damage;
                            break;
                        case 2:
                            newItemData.type = (int)((ArmorData)itemDataKp.Value).armorType;
                            newItemData.value = ((ArmorData)itemDataKp.Value).defence;
                            break;
                        case 3:
                            newItemData.type = (int)((ConsumableData)itemDataKp.Value).consumableType;
                            newItemData.maxCount = ((ConsumableData)itemDataKp.Value).maxCount;
                            newItemData.value = ((ConsumableData)itemDataKp.Value).value;
                            break;
                        default:
                            break;
                    }

                    db.ItemDatas.Add(newItemData);
                }

                bool success = db.SaveChangesEx();

                if (success == false)
                    return;
            }
            else
            {
                foreach (var itemData in itemDatas)
                {
                    ItemType type = (ItemType)(itemData.id / 100000);
                    switch (type)
                    {
                        case ItemType.None:
                            break;
                        case ItemType.Weapon:
                            ItemDict.Add(itemData.id, new WeaponData()
                            {
                                name = itemData.name,
                                id = itemData.id,
                                damage = itemData.value,
                                weaponType = (WeaponType)itemData.type,
                                itemType = type
                            });
                            break;
                        case ItemType.Armor:
                            ItemDict.Add(itemData.id, new ArmorData()
                            {
                                name = itemData.name,
                                id = itemData.id,
                                defence = itemData.value,
                                armorType = (ArmorType)itemData.type,
                                itemType = type
                            });
                            break;
                        case ItemType.Consumable:
                            ItemDict.Add(itemData.id, new ConsumableData()
                            {
                                name = itemData.name,
                                id = itemData.id,
                                maxCount = itemData.maxCount,
                                value = itemData.value,
                                consumableType = (ConsumableType)itemData.type,
                                itemType = type
                            });
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {
            string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);
        }
    }
}