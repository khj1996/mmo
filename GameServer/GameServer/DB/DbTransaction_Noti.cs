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


        public static void BuyItemNoti(Player player, List<ItemDb> itemData, GameRoom room)
        {
            if (player == null || itemData == null || room == null)
                return;


            // You
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    foreach (var costItem in itemData.Where(x => x.Count < 0))
                    {
                        db.Entry(costItem).State = EntityState.Unchanged;
                        db.Entry(costItem).Property(nameof(ItemDb.Count)).IsModified = true;
                    }

                    foreach (var getItem in itemData.Where(x => x.Count > 0))
                    {
                        db.Items.Add(getItem);
                    }


                    bool success = db.SaveChangesEx();
                    if (success)
                    {
                        // Me
                        room.Push(() =>
                        {
                            foreach (var costItem in itemData.Where(x => x.Count < 0))
                            {
                                player.Inven.SetItemAmount(costItem.TemplateId, costItem.Count);
                            }

                            foreach (var getItem in itemData.Where(x => x.Count > 0))
                            {
                                Item newItem = Item.MakeItem(getItem);
                                player.Inven.Add(newItem);
                            }


                            // Client Noti
                            {
                                S_BuyItem buyPacket = new S_BuyItem()
                                {
                                    Result = true
                                };

                                player.Session.Send(buyPacket);
                            }
                        });
                    }
                }
            });
        }
    }
}