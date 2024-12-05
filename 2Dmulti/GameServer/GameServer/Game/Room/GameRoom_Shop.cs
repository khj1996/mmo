using GameServer.Data;
using GameServer.DB;
using Google.Protobuf.Protocol;

namespace GameServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        public void HandleBuyItem(Player? player, C_BuyItem buyItemPacket)
        {
            if (player == null || !DataManager.ShopDict.TryGetValue(buyItemPacket.Shop, out var shopData))
                return;

            var productData = shopData.productList.FirstOrDefault(x => x.id == buyItemPacket.ProductId);
            if (productData == null)
            {
                SendBuyItemResult(player, false);
                return;
            }

            var costItem = player.Inven.Find(x => x.TemplateId == productData.cType && x.Count >= productData.cAmount);;
            if (costItem == null)
            {
                SendBuyItemResult(player, false);
                return;
            }

            if (!DataManager.ItemDict.TryGetValue(productData.pId, out var rewardData))
            {
                SendBuyItemResult(player, false);
                return;
            }

            var rewardDatas = new List<ItemDb>();

            var updatedCostItem = new ItemDb()
            {
                ItemDbId = costItem.ItemDbId,
                TemplateId = costItem.TemplateId,
                Count = costItem.Count - productData.cAmount,
                Slot = costItem.Slot,
                OwnerDbId = player.PlayerDbId
            };

            // Find an available slot for the reward item, handling stackable items
            var existingItem = player.Inven.Find(x => x.TemplateId == rewardData.id && x.Stackable);
            var rewardSlot = existingItem == null ? player.Inven.GetEmptySlot() : player.Inven.GetAvailableSlot(existingItem.TemplateId);
            var rewardCount = existingItem == null ? productData.quantity : existingItem.Count + productData.quantity;

            var rewardItem = new ItemDb()
            {
                TemplateId = rewardData.id,
                Count = rewardCount,
                Slot = (int)rewardSlot,
                OwnerDbId = player.PlayerDbId,
            };
            
            if (existingItem != null)
            {
                rewardItem.ItemDbId = existingItem.ItemDbId;
            }

            rewardDatas.Add(rewardItem);

            DbTransaction.BuyItemNoti(player, updatedCostItem, rewardDatas, player.Room);
        }

        private void SendBuyItemResult(Player player, bool result)
        {
            var packet = new S_BuyItem() { Result = result };
            player.Session.Send(packet);
        }
    }
}