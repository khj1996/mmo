using Google.Protobuf.Protocol;

namespace GameServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        //장비 장착
        public void HandleEquipItem(Player player, C_EquipItem equipPacket)
        {
            if (player == null)
                return;

            player.HandleEquipItem(equipPacket);
        }
    }
}