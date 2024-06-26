using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;

public class NetworkManager
{
	public string Token { get; set; }

	ServerSession _session = new ServerSession();

	private bool isConnected = false; 

	public void Send(IMessage packet)
	{
		_session.Send(packet);
	}

	public void Connect(ServerInfo info)
	{
		if (isConnected)
		{
			DisConnect();
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

	public void DisConnect()
	{
		_session.Disconnect();
		isConnected = false;
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
