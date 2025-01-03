﻿using System.Collections.Generic;
using System.IO;
using GameServer.DB;
using GameServer.Game;
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
        public static Dictionary<int, MonsterData> MonsterDict { get; private set; } = new Dictionary<int, MonsterData>();
        public static Dictionary<int, ShopData> ShopDict { get; private set; } = new Dictionary<int, ShopData>();

        #region 데이터 로드

        public static void LoadData()
        {
            StatDict = LoadJson<StatDataLoader, int, StatInfo>("StatData").MakeDict();
            SkillDict = LoadJson<SkillDataLoader, int, Skill>("SkillData").MakeDict();
            LoadItemData();
            LoadMonsterData();
            LoadShopData();
        }

        public static void LoadShopData()
        {
            using AppDbContext db = new AppDbContext();

            List<ShopDb> shopDatas = db.ShopDatas.Where(x => x.ShopDbId != -1).Include(shopDb => shopDb.ShopProducts).ToList();

            if (shopDatas.Count == 0)
            {
                ShopDict = LoadJson<ShopLoader, int, ShopData>("ShopData").MakeDict();

                foreach (var shopDataKp in ShopDict)
                {
                    var newShopData = new ShopDb()
                    {
                        ShopDbId = shopDataKp.Key,
                        Name = shopDataKp.Value.name,
                        ShopProducts = new List<ShopProductDb>()
                    };

                    foreach (var productInfo in shopDataKp.Value.productList)
                    {
                        newShopData.ShopProducts.Add(new ShopProductDb()
                        {
                            ShopProductDbId = productInfo.id,
                            PId = productInfo.pId,
                            CType = productInfo.cType,
                            CAmount = productInfo.cAmount,
                            Quantity = productInfo.quantity
                        });
                    }


                    db.ShopDatas.Add(newShopData);
                }

                bool success = db.SaveChangesEx();

                if (success == false)
                    return;
            }
            else
            {
                foreach (var shopData in shopDatas)
                {
                    var newShopData = new ShopData()
                    {
                        id = shopData.ShopDbId,
                        name = shopData.Name,
                        productList = new List<ProductData>()
                    };
                    foreach (var product in shopData.ShopProducts)
                    {
                        newShopData.productList.Add(new ProductData()
                        {
                            id = product.ShopProductDbId,
                            cAmount = product.CAmount,
                            cType = product.CType,
                            pId = product.PId,
                            quantity = product.Quantity
                        });
                    }

                    ShopDict.Add(shopData.ShopDbId, newShopData);
                }
            }
        }

        public static void LoadMonsterData()
        {
            using AppDbContext db = new AppDbContext();

            List<MonsterDb> monsterDatas =
                db.MonsterDatas.Include(x => x.rewards).Where(x => x.MonsterDbId != -1).ToList();

            if (monsterDatas.Count == 0)
            {
                MonsterDict = LoadJson<MonsterLoader, int, MonsterData>("MonsterData").MakeDict();

                foreach (var monsterDataKp in MonsterDict)
                {
                    var newMonsterData = new MonsterDb()
                    {
                        MonsterDbId = monsterDataKp.Key,
                        name = monsterDataKp.Value.name,
                        Level = monsterDataKp.Value.stat.Level,
                        MaxHp = monsterDataKp.Value.stat.MaxHp,
                        Attack = monsterDataKp.Value.stat.Attack,
                        Speed = monsterDataKp.Value.stat.Speed,
                        TotalExp = monsterDataKp.Value.stat.TotalExp,
                        rewards = new List<DB.MonsterRewardDb>()
                    };

                    foreach (var rewardData in monsterDataKp.Value.rewards)
                    {
                        var newRewardData = new MonsterRewardDb()
                        {
                            count = rewardData.count,
                            probability = rewardData.probability,
                            itemId = rewardData.itemId,
                            MonsterDbId = monsterDataKp.Key
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
                        id = monsterData.MonsterDbId,
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

                    MonsterDict.Add(monsterData.MonsterDbId, newMonsterData);
                }
            }
        }

        public static void LoadItemData()
        {
            using AppDbContext db = new AppDbContext();


            List<ItemDataDb> itemDatas =
                db.ItemDatas.Where(x => x.ItemTemplateId != -1).ToList();

            if (itemDatas.Count == 0)
            {
                ItemDict = LoadJson<ItemLoader, int, ItemData>("ItemData").MakeDict();

                foreach (var itemDataKp in ItemDict)
                {
                    var newItemData = new ItemDataDb()
                    {
                        name = itemDataKp.Value.name,
                        ItemTemplateId = itemDataKp.Key,
                    };

                    int type = newItemData.ItemTemplateId / 100000;

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
                        case 4:
                            newItemData.maxCount = ((CurrencyData)itemDataKp.Value).maxCount;
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
                    ItemType type = (ItemType)(itemData.ItemTemplateId / 100000);
                    switch (type)
                    {
                        case ItemType.None:
                            break;
                        case ItemType.Weapon:
                            ItemDict.Add(itemData.ItemTemplateId, new WeaponData()
                            {
                                name = itemData.name,
                                id = itemData.ItemTemplateId,
                                damage = itemData.value,
                                weaponType = (WeaponType)itemData.type,
                                itemType = type
                            });
                            break;
                        case ItemType.Armor:
                            ItemDict.Add(itemData.ItemTemplateId, new ArmorData()
                            {
                                name = itemData.name,
                                id = itemData.ItemTemplateId,
                                defence = itemData.value,
                                armorType = (ArmorType)itemData.type,
                                itemType = type
                            });
                            break;
                        case ItemType.Consumable:
                            ItemDict.Add(itemData.ItemTemplateId, new ConsumableData()
                            {
                                name = itemData.name,
                                id = itemData.ItemTemplateId,
                                maxCount = itemData.maxCount,
                                value = itemData.value,
                                consumableType = (ConsumableType)itemData.type,
                                itemType = type
                            });
                            break;
                        case ItemType.Currency:
                            ItemDict.Add(itemData.ItemTemplateId, new CurrencyData()
                            {
                                name = itemData.name,
                                id = itemData.ItemTemplateId,
                                maxCount = itemData.maxCount,
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

        #endregion


        public static int GetLevelForExp(int currentExp)
        {
            int left = 1;
            int right = StatDict.Count;

            while (left <= right)
            {
                int mid = (left + right) / 2;

                if (StatDict[mid].TotalExp <= currentExp && (mid == StatDict.Count || StatDict[mid + 1].TotalExp > currentExp))
                    return mid;
                else if (StatDict[mid].TotalExp > currentExp)
                    right = mid - 1;
                else
                    left = mid + 1;
            }

            return 1;
        }
    }
}