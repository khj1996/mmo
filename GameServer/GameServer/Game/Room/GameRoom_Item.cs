using Google.Protobuf.Protocol;

namespace GameServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        //장비 장착
        public void HandleUseItem(Player player, C_UseItem equipPacket)
        {
            if (player == null)
                return;

            player.HandleUseItem(equipPacket);
        }
    }
}