using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GameServer.Data;

namespace GameServer.DB
{
    public class AppDbContext : DbContext
    {
        //DB
        public DbSet<AccountGameDb> Accounts { get; set; }

        public DbSet<ServerDb> Servers { get; set; }
        public DbSet<PlayerDb> Players { get; set; }
        public DbSet<ItemDb> Items { get; set; }
        public DbSet<MapDb> MapDatas { get; set; }
        public DbSet<MonsterDb> MonsterDatas { get; set; }
        public DbSet<MonsterRewardDb> RewardDatas { get; set; }
        public DbSet<ItemDataDb> ItemDatas { get; set; }
        public DbSet<ShopDb> ShopDatas { get; set; }
        public DbSet<ShopProductDb> ShopProductDatas { get; set; }

        //로그
        static readonly ILoggerFactory _logger = LoggerFactory.Create(builder => { builder.AddConsole(); });

        string _connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GameDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options
                //.UseLoggerFactory(_logger)
                .UseSqlServer(ConfigManager.Config == null ? _connectionString : ConfigManager.Config.connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AccountGameDb>()
                .HasIndex(a => a.AccountName)
                .IsUnique();

            builder.Entity<PlayerDb>()
                .HasIndex(p => p.PlayerName)
                .IsUnique();

            builder.Entity<ServerDb>()
                .HasIndex(s => s.Name)
                .IsUnique();

            builder.Entity<MapDb>()
                .HasIndex(s => s.MapDbId)
                .IsUnique();

            builder.Entity<MonsterDb>()
                .HasIndex(s => s.MonsterDbId)
                .IsUnique();

            builder.Entity<MonsterRewardDb>()
                .HasIndex(s => s.MonsterRewardDbId)
                .IsUnique();

            builder.Entity<ItemDataDb>()
                .HasIndex(s => s.ItemDataDbId)
                .IsUnique();

            builder.Entity<ItemDb>()
                .HasIndex(s => new { s.ItemDbId, s.OwnerDbId })
                .IsUnique();


            builder.Entity<ShopDb>()
                .HasIndex(s => s.ShopDbId)
                .IsUnique();

            builder.Entity<ShopProductDb>()
                .HasIndex(s => new { s.ShopProductDbId, s.ShopDbId })
                .IsUnique();
        }
    }
}