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


        public static void BuyItemNoti(Player? player, ItemDb? costItem, List<ItemDb>? rewardItems, GameRoom? room)
        {
            if (player == null || costItem == null || rewardItems == null || room == null)
                return;


            // You
            Instance.Push(() =>
            {
                using (var db = new AppDbContext())
                {
                    db.Entry(costItem).State = EntityState.Unchanged;
                    db.Entry(costItem).Property(nameof(ItemDb.Count)).IsModified = true;


                    foreach (var getItem in rewardItems)
                    {
                        if (getItem.ItemDbId != 0)
                        {
                            db.Entry(getItem).State = EntityState.Unchanged;
                            db.Entry(getItem).Property(nameof(ItemDb.Count)).IsModified = true;
                        }
                        else
                        {
                            db.Items.Add(getItem);
                        }
                    }


                    bool success = db.SaveChangesEx();
                    if (success)
                    {
                        // Me
                        room.Push(() =>
                        {
                            S_BuyItem buyPacket = new S_BuyItem()
                            {
                                Result = true
                            };

                            {
                                var newItem = Item.MakeItem(costItem);
                                player.Inven.Add(newItem);

                                var rewardInfo = new ItemInfo() { };
                                rewardInfo.MergeFrom(newItem.Info);
                                buyPacket.Items.Add(rewardInfo);
                            }


                            foreach (var getItem in rewardItems)
                            {
                                var newItem = Item.MakeItem(getItem);
                                player.Inven.Add(newItem);

                                var rewardInfo = new ItemInfo() { };
                                rewardInfo.MergeFrom(newItem.Info);
                                buyPacket.Items.Add(rewardInfo);
                            }


                            player.Session.Send(buyPacket);
                        });
                    }
                }
            });
        }
    }
}