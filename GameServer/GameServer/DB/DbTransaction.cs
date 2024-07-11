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
			playerDb.Hp = player.Info.StatInfo.Hp;
			playerDb.CurMap = player.Room.Map.MapId;
			playerDb.PosX = player.Info.PosInfo.PosX;
			playerDb.PosY = player.Info.PosInfo.PosY;
			playerDb.PosZ = player.Info.PosInfo.PosZ;
			
			//플레이어 정보를 db에 저장
			Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{
					db.Entry(playerDb).State = EntityState.Unchanged;
					db.Entry(playerDb).Property(nameof(PlayerDb.Hp)).IsModified = true;
					db.Entry(playerDb).Property(nameof(PlayerDb.CurMap)).IsModified = true;
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

			// TODO : 살짝 문제가 있긴 하다...
			// 1) DB에다가 저장 요청
			// 2) DB 저장 OK
			// 3) 메모리에 적용
			int? slot = player.Inven.GetEmptySlot();
			if (slot == null)
				return;

			ItemDb itemDb = new ItemDb()
			{
				TemplateId = rewardData.itemId,
				Count = rewardData.count,
				Slot = slot.Value,
				OwnerDbId = player.PlayerDbId
			};

			// You
			Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{
					db.Items.Add(itemDb);
					bool success = db.SaveChangesEx();
					if (success)
					{
						// Me
						room.Push(() =>
						{
							Item newItem = Item.MakeItem(itemDb);
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
