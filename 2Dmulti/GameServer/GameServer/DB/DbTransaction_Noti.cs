using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using GameServer.Data;
using GameServer.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.DB
{
    public partial class DbTransaction : JobSerializer
    {
        public static void EquipItemNoti(Player player, Item item)
        {
            if (player == null || item == null)
                return;

            ItemDb itemDb = new ItemDb()
            {
                ItemDbId = item.ItemDbId,
                Equipped = item.Equipped
            };

            // You
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Entry(itemDb).State = EntityState.Unchanged;
                    db.Entry(itemDb).Property(nameof(ItemDb.Equipped)).IsModified = true;

                    bool success = db.SaveChangesEx();
                    if (!success)
                    {
                        // 실패했으면 Kick
                    }
                }
            });
        }

        public static void UseItemNoti(Player? player, ItemDb? useItem)
        {
            if (player == null || useItem == null)
                return;

            // You
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Entry(useItem).State = EntityState.Unchanged;
                    db.Entry(useItem).Property(nameof(ItemDb.Count)).IsModified = true;
                    
                    

                    bool success = db.SaveChangesEx();
                    if (!success)
                    {
                        // 실패했으면 Kick
                    }
                }
            });
        }


        public static void BuyItemNoti(Player? player, ItemDb? costItem, List<ItemDb>? rewardItems, GameRoom? room)
        {
            if (player == null || costItem == null || rewardItems == null || room == null)
                return;

            Instance.Push(() =>
            {
                using (var db = new AppDbContext())
                {
                    db.Entry(costItem).State = EntityState.Unchanged;
                    db.Entry(costItem).Property(nameof(ItemDb.Count)).IsModified = true;

                    foreach (var item in rewardItems)
                    {
                        if (item.ItemDbId != 0)
                        {
                            db.Entry(item).State = EntityState.Unchanged;
                            db.Entry(item).Property(nameof(ItemDb.Count)).IsModified = true;
                        }
                        else
                        {
                            db.Items.Add(item);
                        }
                    }

                    bool success = db.SaveChangesEx();
                    if (success)
                    {
                        NotifyPlayerOfPurchase(player, costItem, rewardItems, room);
                    }
                }
            });
        }

        private static void NotifyPlayerOfPurchase(Player player, ItemDb costItem, List<ItemDb> rewardItems, GameRoom room)
        {
            room.Push(() =>
            {
                var buyPacket = new S_BuyItem { Result = true };

                AddItemToPacket(player, costItem, buyPacket);

                foreach (var item in rewardItems)
                {
                    AddItemToPacket(player, item, buyPacket);
                }

                player.Session.Send(buyPacket);
            });
        }

        private static void AddItemToPacket(Player player, ItemDb itemDb, S_BuyItem packet)
        {
            var newItem = Item.MakeItem(itemDb);
            player.Inven.Add(newItem);

            var rewardInfo = new ItemInfo();
            rewardInfo.MergeFrom(newItem.Info);
            packet.Items.Add(rewardInfo);
        }
    }
}