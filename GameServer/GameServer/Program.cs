using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using GameServer.Data;
using GameServer.DB;
using GameServer.Game;
using ServerCore;

namespace GameServer
{
    class Program
    {
        //연결 처리를 위한 리스너
        static Listener _listener = new Listener();

        //게임 로직 처리를 위한 Task
        static void GameLogicTask()
        {
            while (true)
            {
                GameLogic.Instance.Update();
                Thread.Sleep(0);
            }
        }

        //DB처리를 위한 Task
        static void DbTask()
        {
            while (true)
            {
                DbTransaction.Instance.Flush();
                Thread.Sleep(0);
            }
        }

        //네트워크 처리를 위한 Task
        static void NetworkTask()
        {
            while (true)
            {
                List<ClientSession> sessions = SessionManager.Instance.GetSessions();
                foreach (ClientSession session in sessions)
                {
                    session.FlushSend();
                }

                Thread.Sleep(0);
            }
        }

        //엔드포인트 포트
        public static int Port { get; set; } = 7777;

        //ip주소
        public static string IpAddress { get; set; }

        static void Main(string[] args)
        {
            // DbTask
            {
                Thread t = new Thread(DbTask);
                t.Name = "DB";
                t.Start();
            }

            // NetworkTask
            {
                Thread t = new Thread(NetworkTask);
                t.Name = "Network Send";
                t.Start();
            }

            //설정 파일 로드
            ConfigManager.LoadConfig();
            //데이터 로드
            DataManager.LoadData();


            //클라이언트 접속을 위한 소켓 초기화
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = (args.Length > 1) ? IPAddress.Parse(args[0]) : ipHost.AddressList[1];


            Port = (args.Length > 1) ? int.Parse(args[1]) : Port;

            IPEndPoint endPoint = new IPEndPoint(ipAddr, Port);

            IpAddress = ipAddr.ToString();

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening..." + Port + "/" + ipAddr);


            //방 생성
            GameLogic.Instance.Push(() => { GameLogic.Instance.Add(1); });

            // GameLogic
            Thread.CurrentThread.Name = "GameLogic";
            GameLogicTask();
        }
    }
}