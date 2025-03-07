﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using GameServer;
using GameServer.Game;
using ServerCore;

class PacketHandler
{
    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;
        
		room.Push(room.HandleMove, player, movePacket);
    }

    public static void C_SkillHandler(PacketSession session, IMessage packet)
    {
        C_Skill skillPacket = packet as C_Skill;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleSkill, player, skillPacket);
    }

    public static void C_LoginHandler(PacketSession session, IMessage packet)
    {
        C_Login loginPacket = packet as C_Login;
        ClientSession clientSession = session as ClientSession;
        clientSession.HandleLogin(loginPacket);
    }

    public static void C_EnterGameHandler(PacketSession session, IMessage packet)
    {
        C_EnterGame enterGamePacket = (C_EnterGame)packet;
        ClientSession clientSession = (ClientSession)session;
        clientSession.HandleEnterGame(enterGamePacket);
    }

    public static void C_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        C_CreatePlayer createPlayerPacket = (C_CreatePlayer)packet;
        ClientSession clientSession = (ClientSession)session;
        clientSession.HandleCreatePlayer(createPlayerPacket);
    }

    public static void C_UseItemHandler(PacketSession session, IMessage packet)
    {
        var equipPacket = (C_UseItem)packet;
        var clientSession = (ClientSession)session;

        var player = clientSession.MyPlayer;
        if (player == null)
            return;

        var room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleUseItem, player, equipPacket);
    }

    public static void C_BuyItemHandler(PacketSession session, IMessage packet)
    {
        var buyItemPacket = (C_BuyItem)packet;
        var clientSession = (ClientSession)session;

        var player = clientSession.MyPlayer;
        if (player == null)
            return;

        var room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleBuyItem, player, buyItemPacket);
    }


    public static void C_PongHandler(PacketSession session, IMessage packet)
    {
        ClientSession clientSession = (ClientSession)session;
        clientSession.HandlePong();
    }
}