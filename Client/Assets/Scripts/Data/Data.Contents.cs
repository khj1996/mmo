using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;

namespace Data
{
    #region Stat

    [Serializable]
    public class StatDataLoader : ILoader<int, StatInfo>
    {
        public List<StatInfo> stats = new List<StatInfo>();

        public Dictionary<int, StatInfo> MakeDict()
        {
            Dictionary<int, StatInfo> dict = new Dictionary<int, StatInfo>();
            foreach (StatInfo stat in stats)
            {
                stat.Hp = stat.MaxHp;
                dict.Add(stat.Level, stat);
            }

            return dict;
        }
    }

    #endregion

    #region Shop

    [Serializable]
    public class ShopData
    {
        public int id;
        public string name;
        public List<ProductData> productList;
    }

    [Serializable]
    public class ProductData
    {
        public int id;
        public int pId;
        public int quantity;
        public int cType;
        public int cAmount;
    }


    [Serializable]
    public class ShopLoader : ILoader<int, ShopData>
    {
        public List<ShopData> Shops = new List<ShopData>();

        public Dictionary<int, ShopData> MakeDict()
        {
            Dictionary<int, ShopData> dict = new Dictionary<int, ShopData>();
            foreach (ShopData shop in Shops)
            {
                dict.Add(shop.id, shop);
            }

            return dict;
        }
    }

    #endregion

    #region Skill

    [Serializable]
    public class Skill
    {
        public int id;
        public string name;
        public float cooldown;
        public int damage;
        public SkillType skillType;
        public ProjectileInfo projectile;
    }

    public class ProjectileInfo
    {
        public string name;
        public float speed;
        public int range;
        public string prefab;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, Skill>
    {
        public List<Skill> skills = new List<Skill>();

        public Dictionary<int, Skill> MakeDict()
        {
            Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
            foreach (Skill skill in skills)
                dict.Add(skill.id, skill);
            return dict;
        }
    }

    #endregion

    #region Item

    [Serializable]
    public class ItemData
    {
        public int id;
        public string name;
        public ItemType itemType;
    }

    public class WeaponData : ItemData
    {
        public WeaponType weaponType;
        public int damage;
    }

    public class ArmorData : ItemData
    {
        public ArmorType armorType;
        public int defence;
    }

    public class ConsumableData : ItemData
    {
        public ConsumableType consumableType;
        public int maxCount;
        public int value;
    }

    public class CurrencyData : ItemData
    {
        public int maxCount;
    }

    [Serializable]
    public class ItemLoader : ILoader<int, ItemData>
    {
        public List<WeaponData> weapons = new List<WeaponData>();
        public List<ArmorData> armors = new List<ArmorData>();
        public List<ConsumableData> consumables = new List<ConsumableData>();
        public List<CurrencyData> currency = new List<CurrencyData>();

        public Dictionary<int, ItemData> MakeDict()
        {
            var dict = new Dictionary<int, ItemData>();
            foreach (var item in weapons)
            {
                item.itemType = ItemType.Weapon;
                dict.Add(item.id, item);
            }

            foreach (var item in armors)
            {
                item.itemType = ItemType.Armor;
                dict.Add(item.id, item);
            }

            foreach (var item in consumables)
            {
                item.itemType = ItemType.Consumable;
                dict.Add(item.id, item);
            }

            foreach (var item in currency)
            {
                item.itemType = ItemType.Currency;
                dict.Add(item.id, item);
            }

            return dict;
        }
    }

    #endregion

    #region Monster

    [Serializable]
    public class RewardData
    {
        public int probability; // 100분율
        public int itemId;
        public int count;
    }

    [Serializable]
    public class MonsterData
    {
        public int id;
        public string name;
        public StatInfo stat;

        public List<RewardData> rewards;
    }

    [Serializable]
    public class MonsterLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
            {
                dict.Add(monster.id, monster);
            }

            return dict;
        }
    }

    #endregion
}