using Google.Protobuf;
using Google.Protobuf.Protocol;
using LoginServer;
using LoginServer.Game;
using ServerCore;

class PacketHandler
{
    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
    }

    public static void C_SkillHandler(PacketSession session, IMessage packet)
    {
    }

    public static void C_LoginHandler(PacketSession session, IMessage packet)
    {
        C_Login loginPacket = packet as C_Login;
        ClientSession clientSession = session as ClientSession;
        clientSession.HandleLogin(loginPacket);
    }

    public static void C_EnterGameHandler(PacketSession session, IMessage packet)
    {
    }

    public static void C_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
    }

    public static void C_UseItemHandler(PacketSession session, IMessage packet)
    {
    }

    public static void C_PongHandler(PacketSession session, IMessage packet)
    {
        ClientSession clientSession = (ClientSession)session;
    }
    public static void C_BuyItemHandler(PacketSession session, IMessage packet)
    {/*
        produc equipPacket = (C_EquipItem)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleEquipItem, player, equipPacket);*/
    }
}