using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;

public class NetworkManager
{
    private static NetworkManager _instance = null;

    public static NetworkManager Instance
    {
        get { return _instance; }
    }

    public NetworkManager()
    {
        _instance = this;
    }

    public string Token { get; set; }

    ServerSession _session = new ServerSession();

    public bool isConnected = false;

    public void Send(IMessage packet)
    {
        if (!isConnected)
        {
            _session.Disconnect();
            Token = null;
            Managers.Scene.LoadScene("Login");
            return;
        }

        _session.Send(packet);
    }

    public void Connect(ServerInfo info)
    {
        if (Token == null)
        {
            Debug.LogWarning("연결 실패: 토큰이 없습니다.");
            return;
        }

        if (isConnected)
        {
            _session.Disconnect();
            isConnected = false;
            _session = new ServerSession();
        }

        Debug.Log($"{info.IpAddress} : {info.Port}");

        IPAddress ipAddr = IPAddress.Parse(info.IpAddress);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, info.Port);

        Connector connector = new Connector();

        connector.Connect(endPoint,
            () => { return _session; },
            1);
        isConnected = true;
    }

    public void Update()
    {
        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach (PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            if (handler != null)
                handler.Invoke(_session, packet.Message);
        }
    }
}