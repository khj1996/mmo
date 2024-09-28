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

                var changeData = new List<ItemDb>();

                //소모값
                changeData.Add(new ItemDb()
                {
                    ItemDbId = checkCost.ItemDbId,
                    TemplateId = checkCost.TemplateId,
                    Count = checkCost.Count - productData.cAmount,
                    Slot = checkCost.Slot,
                    OwnerDbId = player.PlayerDbId
                });

                //지급 아이템
                changeData.Add(new ItemDb()
                {
                    TemplateId = rewardData.id,
                    Count = 1,
                    Slot = (int)player.Inven.GetAvailableSlot(rewardData.id),
                    OwnerDbId = player.PlayerDbId
                });


                DbTransaction.BuyItemNoti(player, changeData, player.Room);
            }
        }
    }
}