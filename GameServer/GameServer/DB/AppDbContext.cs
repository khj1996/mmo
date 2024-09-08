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
        public DbSet<MonsterDataDb> MonsterDatas { get; set; }
        public DbSet<RewardDataDb> RewardDatas { get; set; }
        public DbSet<ItemDataDb> ItemDatas { get; set; }

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

            builder.Entity<MonsterDataDb>()
                .HasIndex(s => s.MonsterDataDbid)
                .IsUnique();

            builder.Entity<RewardDataDb>()
                .HasIndex(s => s.RewardDataDbid)
                .IsUnique();


            builder.Entity<ItemDataDb>()
                .HasIndex(s => s.ItemDataDbid)
                .IsUnique();
        }
    }
}