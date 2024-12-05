using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using GameServer.Data;
using GameServer.Game;

namespace GameServer.DB
{
    //파티셜 클래스
    public partial class DbTransaction : JobSerializer
    {
        public static DbTransaction Instance { get; } = new DbTransaction();

        // Me (GameRoom) -> You (Db) -> Me (GameRoom)
        public static void SavePlayerStatus(Player player, GameRoom room)
        {
            //예외처리
            if (player == null || room == null)
                return;

            //플레이어 정보
            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Level = player.Level;
            playerDb.Hp = player.Hp;
            playerDb.TotalExp = player.Exp;
            playerDb.CurMap = player.Room.Map.MapId;
            playerDb.PosX = player.Pos.X;
            playerDb.PosY = player.Pos.Y;
            playerDb.PosZ = 0;

            //플레이어 정보를 db에 저장
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Entry(playerDb).State = EntityState.Unchanged;
                    db.Entry(playerDb).Property(nameof(PlayerDb.Hp)).IsModified = true;
                    db.Entry(playerDb).Property(nameof(PlayerDb.CurMap)).IsModified = true;
                    db.Entry(playerDb).Property(nameof(PlayerDb.Level)).IsModified = true;
                    db.Entry(playerDb).Property(nameof(PlayerDb.TotalExp)).IsModified = true;
                    db.Entry(playerDb).Property(nameof(PlayerDb.PosX)).IsModified = true;
                    db.Entry(playerDb).Property(nameof(PlayerDb.PosY)).IsModified = true;
                    db.Entry(playerDb).Property(nameof(PlayerDb.PosZ)).IsModified = true;
                    bool success = db.SaveChangesEx();
                    if (success)
                    {
                        //room.Push(()=> System.Console.WriteLine($"Hp Saved ({playerDb.Hp})"));
                    }
                }
            });
        }

        public static void RewardPlayer(Player player, RewardData rewardData, GameRoom room)
        {
            if (player == null || rewardData == null || room == null)
                return;


            var test = player.Inven.Find(x => x.TemplateId == rewardData.itemId && x.Stackable);

            var rewardSlot = (test == null) ? player.Inven.GetEmptySlot() : player.Inven.GetAvailableSlot(test.TemplateId);
            var rewardCount = (test == null) ? rewardData.count : test.Count + rewardData.count;


            if (rewardSlot == null)
                return;

            var rewardItem = new ItemDb()
            {
                TemplateId = rewardData.itemId,
                Count = rewardCount,
                Slot = (int)rewardSlot,
                OwnerDbId = player.PlayerDbId
            };

            if (test != null)
            {
                rewardItem.ItemDbId = test.ItemDbId;
            }

            // You
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    if (rewardItem.ItemDbId != 0)
                    {
                        db.Entry(rewardItem).State = EntityState.Unchanged;
                        db.Entry(rewardItem).Property(nameof(ItemDb.Count)).IsModified = true;
                    }
                    else
                    {
                        db.Items.Add(rewardItem);
                    }

                    bool success = db.SaveChangesEx();
                    if (success)
                    {
                        // Me
                        room.Push(() =>
                        {
                            var newItem = Item.MakeItem(rewardItem);
                            player.Inven.Add(newItem);

                            // Client Noti
                            {
                                S_AddItem itemPacket = new S_AddItem();
                                ItemInfo itemInfo = new ItemInfo();
                                itemInfo.MergeFrom(newItem.Info);
                                itemPacket.Items.Add(itemInfo);

                                player.Session.Send(itemPacket);
                            }
                        });
                    }
                }
            });
        }
    }
}