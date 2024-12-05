using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GameServer.DB
{
    [Table("AccountGame")]
    public class AccountGameDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountGameDbId { get; set; }

        public string AccountName { get; set; }
        public string JwtToken { get; set; }
        public ICollection<PlayerDb> Players { get; set; }
    }

    [Table("Player")]
    public class PlayerDb
    {
        public int PlayerDbId { get; set; }
        public string PlayerName { get; set; }

        [ForeignKey("AccountGame")] public int AccountGameDbId { get; set; }
        public AccountGameDb AccountGame { get; set; }

        public ICollection<ItemDb> Items { get; set; }

        public int Level { get; set; }
        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public int Attack { get; set; }
        public float Speed { get; set; }
        public int TotalExp { get; set; }
        public int CurMap { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
    }

    [Table("Item")]
    public class ItemDb
    {
        public int ItemDbId { get; set; }
        public int TemplateId { get; set; }
        public int Count { get; set; }
        public int Slot { get; set; }
        public bool Equipped { get; set; } = false;

        [ForeignKey("Owner")] public int? OwnerDbId { get; set; }
        public PlayerDb Owner { get; set; }
    }

    [Table("Monster")]
    public class MonsterDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MonsterDbId { get; set; }

        public string name { get; set; }

        public int Level { get; set; }
        public int MaxHp { get; set; }
        public int Attack { get; set; }
        public float Speed { get; set; }
        public int TotalExp { get; set; }

        public ICollection<MonsterRewardDb> rewards { get; set; }
    }

    [Table("MonsterReward")]
    public class MonsterRewardDb
    {
        public int MonsterRewardDbId { get; set; }
        public int probability { get; set; }
        public int itemId { get; set; }
        public int count { get; set; }


        [ForeignKey("Monster")] public int? MonsterDbId { get; set; }
        public MonsterDb Monster { get; set; }
    }

    [Table("ItemData")]
    public class ItemDataDb
    {
        public int ItemDataDbId { get; set; }
        public int ItemTemplateId { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public int maxCount { get; set; }
        public int value { get; set; }
    }

    [Table("ServerInfo")]
    public class ServerDb
    {
        public int ServerDbId { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int BusyScore { get; set; }
    }

    [Table("MapInfo")]
    public class MapDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MapDbId { get; set; }

        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public int MinX { get; set; }
        public int MinY { get; set; }
        public string TileInfo { get; set; }
    }

    [Table("Shop")]
    public class ShopDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ShopDbId { get; set; }

        public string Name { get; set; }

        public ICollection<ShopProductDb> ShopProducts { get; set; }
    }

    [Table("ShopProduct")]
    [PrimaryKey(nameof(ShopProductDbId), nameof(ShopDbId))]
    public class ShopProductDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ShopProductDbId { get; set; }

        public int Quantity { get; set; }
        public int PId { get; set; }
        public int CType { get; set; }
        public int CAmount { get; set; }


        [ForeignKey("Shop")] public int? ShopDbId { get; set; }
        public ShopDb Shop { get; set; }
    }


    /*[Table("MapObject")]
    public class MapOnjectDb
    {
        public int ItemDbId { get; set; }
        public int TemplateId { get; set; }
        public int Count { get; set; }
        public int Slot { get; set; }
        public bool Equipped { get; set; } = false;

        [ForeignKey("Owner")] public int? OwnerDbId { get; set; }
        public PlayerDb Owner { get; set; }
    }*/
}