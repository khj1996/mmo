﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using UnityEngine;

class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        var enterGamePacket = packet as S_EnterGame;

        Managers.Map.LoadMap(enterGamePacket.MapId);


        Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGameHandler = packet as S_LeaveGame;
        Managers.Object.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
        foreach (ObjectInfo obj in spawnPacket.Objects)
        {
            Managers.Object.Add(obj, myPlayer: false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        foreach (int id in despawnPacket.ObjectIds)
        {
            Managers.Object.Remove(id);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;

        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
            return;

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return;

        bc.UpdatePosition(movePacket);
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;

        GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.UseSkill(skillPacket);
        }
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp changePacket = packet as S_ChangeHp;

        GameObject go = Managers.Object.FindById(changePacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.Hp = changePacket.Hp;
        }
    }

    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = packet as S_Die;

        GameObject go = Managers.Object.FindById(diePacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.Hp = 0;
            cc.OnDead();
        }
    }

    public static void S_ConnectedHandler(PacketSession session, IMessage packet)
    {
        Debug.Log("S_ConnectedHandler");
        C_Login loginPacket = new C_Login();

        loginPacket.JwtToken = Managers.Network.Token;
        Managers.Network.Send(loginPacket);
    }

    public static void S_LoginHandler(PacketSession session, IMessage packet)
    {
        S_Login loginPacket = (S_Login)packet;
        Debug.Log($"LoginOk({loginPacket.LoginOk})");


        Managers.UI.ShowPopupUI<UI_SelectServerPopup>().SetServers(loginPacket.ServerInfos);
    }

    public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        S_CreatePlayer createOkPacket = (S_CreatePlayer)packet;

        if (createOkPacket.Player == null)
        {
            Managers.UI.ShowPopupUI<UI_SimpleTextPopup>().SetText("생성실패", "닉네임 중복");
        }
        else
        {
            Managers.UI.ShowPopupUI<UI_SimpleTextPopup>().SetText("생성성공", "캐릭터 추가");
            GameObject.Find(nameof(UI_SelectCharacterPopup)).GetComponent<UI_SelectCharacterPopup>()
                .AddCharacter(createOkPacket.Player);
        }
    }

    public static void S_ItemListHandler(PacketSession session, IMessage packet)
    {
        S_ItemList itemList = (S_ItemList)packet;

        Managers.Inven.Clear();

        foreach (ItemInfo itemInfo in itemList.Items)
        {
            Item item = Item.MakeItem(itemInfo);
            Managers.Inven.Add(item);
        }

        if (Managers.Object.MyPlayer != null)
            Managers.Object.MyPlayer.RefreshAdditionalStat();
    }

    public static void S_AddItemHandler(PacketSession session, IMessage packet)
    {
        S_AddItem itemList = (S_AddItem)packet;

        Managers.Inven.Add(itemList.Items.ToList());

        if (Managers.Object.MyPlayer != null)
        {
            Managers.Object.MyPlayer.RefreshAdditionalStat();
            Managers.Object.MyPlayer.Exp += 3;
        }
    }

    public static void S_UseItemHandler(PacketSession session, IMessage packet)
    {
        S_UseItem equipItemOk = (S_UseItem)packet;

        Item item = Managers.Inven.Get(equipItemOk.ItemDbId);
        if (item == null)
            return;

        if (item.ItemType != ItemType.Consumable)
        {
            item.Equipped = equipItemOk.Equipped;
            Debug.Log("아이템 착용 변경!");


            if (Managers.Object.MyPlayer != null)
            {
                Managers.Object.MyPlayer.RefreshAdditionalStat();
            }
        }
        else if (item.ItemType == ItemType.Consumable)
        {
            item.Count -= 1;
        }

        UI_GameScene gameSceneUI = Managers.UI.CurrentSceneUI as UI_GameScene;
        gameSceneUI.InvenUI.RefreshUI(Define.InvenRefreshType.All);
        gameSceneUI.StatUI.RefreshUI();
    }

    public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
    {
        S_ChangeStat itemList = (S_ChangeStat)packet;

        // TODO
    }

    public static void S_EnterServerHandler(PacketSession session, IMessage packet)
    {
        S_EnterServer enterPacket = (S_EnterServer)packet;

        Managers.UI.ShowPopupUI<UI_SelectCharacterPopup>().SetCharacter(enterPacket.Players);
    }

    public static void S_BuyItemHandler(PacketSession session, IMessage packet)
    {
        S_BuyItem buyPacket = (S_BuyItem)packet;
        Debug.Log(buyPacket.Result);

        if (!buyPacket.Result)
            return;

        foreach (ItemInfo itemInfo in buyPacket.Items)
        {
            Item item = Item.MakeItem(itemInfo);
            Debug.Log(item.Count);
            Managers.Inven.Add(item);
        }

        Debug.Log("아이템을 획득했습니다!");

        var gameSceneUI = Managers.UI.CurrentSceneUI as UI_GameScene;

        if (gameSceneUI != null)
        {
            gameSceneUI.InvenUI.RefreshUI(Define.InvenRefreshType.All);

            var remainingItems = buyPacket.Items.Skip(1).ToList();
            Managers.UI.ShowPopupUI<UI_GetItemPopUp>().OpenPopUpMultiItem(remainingItems);
        }
    }

    public static void S_UpdateLevelHandler(PacketSession session, IMessage packet)
    {
        S_UpdateLevel updateLevelPacket = (S_UpdateLevel)packet;


        if (Managers.Object.MyPlayer != null)
        {
            Managers.UI.ShowPopupUI<UI_LevelUp>().LevelUp(Managers.Object.MyPlayer.Stat, updateLevelPacket.StatInfo);
            Managers.Object.MyPlayer.Stat = updateLevelPacket.StatInfo;
            Managers.Object.MyPlayer.isLevelUp = true;
        }
    }

    public static void S_PingHandler(PacketSession session, IMessage packet)
    {
        C_Pong pongPacket = new C_Pong();
        //Debug.Log("[Server] PingCheck");
        Managers.Network.Send(pongPacket);
    }
}