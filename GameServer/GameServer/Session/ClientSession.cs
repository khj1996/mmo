using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using GameServer.Game;
using GameServer.Data;

namespace GameServer
{
    //유저 세션
    public partial class ClientSession : PacketSession
    {
        //유저 현재 상태 체크용 
        public PlayerServerState ServerState { get; private set; } = PlayerServerState.ServerStateLogin;

        //세션에 해당하는 플레이어가 조작하는 유저
        public Player? MyPlayer { get; set; }
        //할당된 세션 id
        public int SessionId { get; set; }

        //락
        object _lock = new object();
        //세션에 해당하는 유저에게 보낼 패킷
        List<ArraySegment<byte>> _reserveQueue = new List<ArraySegment<byte>>();

        // 패킷 모아 보내기
        int _reservedSendBytes = 0;
        //마지막으로 보낸 틱
        long _lastSendTick = 0;

        //유저 접속 체크용 틱
        //강종등으로 인한 유저 연결 해제용
        long _pingpongTick = 0;

        //유저 접속 체크용
        public void Ping()
        {
            if (_pingpongTick > 0)
            {
                long delta = (System.Environment.TickCount64 - _pingpongTick);
                if (delta > 30 * 1000)
                {
                    Console.WriteLine("Disconnected by PingCheck");
                    Disconnect();
                    return;
                }
            }

            S_Ping pingPacket = new S_Ping();
            Send(pingPacket);

            GameLogic.Instance.PushAfter(5000, Ping);
        }

        //클라이언트에서 핑 패킷을 받으면 틱 갱신
        public void HandlePong()
        {
            _pingpongTick = System.Environment.TickCount64;
        }

        #region Network

        // 세션에 보낼 버퍼 추가용도 
        //FlushSend에서 실질적으로 보냄
        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

            lock (_lock)
            {
                _reserveQueue.Add(sendBuffer);
                _reservedSendBytes += sendBuffer.Length;
            }
        }

        // 실제 Network IO 보내는 부분
        public void FlushSend()
        {
            List<ArraySegment<byte>> sendList = null;

            lock (_lock)
            {
                // 0.1초가 지났거나, 너무 패킷이 많이 모일 때 (1만 바이트)
                long delta = (System.Environment.TickCount64 - _lastSendTick);
                if (delta < 100 && _reservedSendBytes < 10000)
                    return;

                // 패킷 모아 보내기
                _reservedSendBytes = 0;
                _lastSendTick = System.Environment.TickCount64;

                sendList = _reserveQueue;
                _reserveQueue = new List<ArraySegment<byte>>();
            }

            Send(sendList);
        }

        //유저가 연결시 실행
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            {
                S_Connected connectedPacket = new S_Connected();
                Send(connectedPacket);
            }

            //핑체크 시작
            GameLogic.Instance.PushAfter(5000, Ping);
        }

        //패킷 수신
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        //연결 해제
        public override void OnDisconnected(EndPoint endPoint)
        {
            GameLogic.Instance.Push(() =>
            {
                if (MyPlayer == null)
                    return;

                GameRoom room = GameLogic.Instance.Find(1);
                room.Push(room.LeaveGame, MyPlayer.Info.ObjectId);
            });

            SessionManager.Instance.Remove(this);
        }

        //패킷 송신
        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }

        #endregion
    }
}