using GameServer.Data;
using GameServer.DB;
using Google.Protobuf.Protocol;

namespace GameServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        //장비 장착
        public void HandleBuyItem(Player? player, C_BuyItem buyItemPacket)
        {
            if (player == null)
                return;

            if (DataManager.ShopDict.TryGetValue(buyItemPacket.Shop, out var shopData))
            {
                //상품 존재여부 확인
                ProductData productData = shopData.productList.FirstOrDefault(x => x.id == buyItemPacket.ProductId);
                if (productData == null)
                {
                    S_BuyItem packet = new S_BuyItem()
                    {
                        Result = false
                    };

                    player.Session.Send(packet);
                    return;
                }

                //재화 확인
                var checkCost = player.Inven.Find(x => x.TemplateId == productData.cType && x.Count >= productData.cAmount);
                if (checkCost == null)
                {
                    S_BuyItem packet = new S_BuyItem()
                    {
                        Result = false
                    };

                    player.Session.Send(packet);
                    return;
                }

                //지급아이템 확인
                if (!DataManager.ItemDict.TryGetValue(productData.pId, out var rewardData))
                {
                    S_BuyItem packet = new S_BuyItem()
                    {
                        Result = false
                    };

                    player.Session.Send(packet);
                    return;
                }

                var rewardDatas = new List<ItemDb>();

                var costItem = new ItemDb()
                {
                    ItemDbId = checkCost.ItemDbId,
                    TemplateId = checkCost.TemplateId,
                    Count = checkCost.Count - productData.cAmount,
                    Slot = checkCost.Slot,
                    OwnerDbId = player.PlayerDbId
                };

                var test = player.Inven.Find(x => x.TemplateId == rewardData.id && x.Stackable);

                //TODO : 보유 최대치 적용 필요
                var rewardSlot = (test == null) ? player.Inven.GetEmptySlot() : player.Inven.GetAvailableSlot(test.TemplateId);
                var rewardCount = (test == null) ? 1 : test.Count + 1;

                var rewardItem = new ItemDb()
                {
                    TemplateId = rewardData.id,
                    Count = rewardCount,
                    Slot = (int)rewardSlot,
                    OwnerDbId = player.PlayerDbId
                };

                if (test != null)
                {
                    rewardItem.ItemDbId = test.ItemDbId;
                }

                //지급 아이템
                rewardDatas.Add(rewardItem);


                DbTransaction.BuyItemNoti(player, costItem, rewardDatas, player.Room);
            }
        }
    }
}