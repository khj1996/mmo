using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("Account")] public int AccountDbId { get; set; }
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
    public class MonsterDataDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MonsterDataDbid { get; set; }

        public string name { get; set; }

        public int Level { get; set; }
        public int MaxHp { get; set; }
        public int Attack { get; set; }
        public float Speed { get; set; }
        public int TotalExp { get; set; }

        public ICollection<RewardDataDb> rewards { get; set; }
    }

    [Table("RewardData")]
    public class RewardDataDb
    {
        public int RewardDataDbid { get; set; }
        public int probability { get; set; }
        public int itemId { get; set; }
        public int count { get; set; }


        [ForeignKey("Owner")] public int? OwnerDbId { get; set; }
        public MonsterDataDb Owner { get; set; }
    }

    [Table("ItemData")]
    public class ItemDataDb
    {
        public int ItemDataDbid { get; set; }
        public int id { get; set; }
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
    [Table("ShopProduct")]
    public class ShopProductDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ShopProductId { get; set; }

        public int PId { get; set; }
        public int CType { get; set; }
        public int CAmount { get; set; }
    }
    
    [Table("Shop")]
    public class ShopDb
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ShopId { get; set; }

        public int Name { get; set; }
        public ICollection<ShopProductDb> ProductList { get; set; }
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