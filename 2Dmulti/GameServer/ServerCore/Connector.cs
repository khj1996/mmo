using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerCore
{
    public class Connector
    {
        //등록된 함수를 invoke하여 세션을 생성해주는 대리자
        Func<Session> _sessionFactory;

        /// <summary>
        /// 연결시 실행하는 기능
        /// </summary>
        /// <param name="endPoint">연결을 위한 엔드포인트</param>
        /// <param name="sessionFactory">_sessionFactory를 사용하여 실행할 메소드</param>
        /// <param name="count">연결하려는 클라이언트 숫자</param>
        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
        {
            //생성을 원하는 만큼 반복
            for (int i = 0; i < count; i++)
            {
                // 매개변수로 받은 엔드포인트를 이용하여 소켓 생성
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                //세션 생성시 실행하려는 기능
                _sessionFactory = sessionFactory;

                //송신과 수신을 담당하는 클래스
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                //작업이 완료되면 실행하는 기능
                args.Completed += OnConnectCompleted;
                //엔드포인트
                args.RemoteEndPoint = endPoint;
                //현재 작업과 연결된 사용자
                args.UserToken = socket;

                //연결
                RegisterConnect(args);

                //여러개 생성시 텀
                Thread.Sleep(10);
            }
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            //현재 사용자(소켓)
            Socket socket = args.UserToken as Socket;

            //소켓 타입이 아닐시 실패
            if (socket == null)
                return;

            try
            {
                //비동기 형식으로 연결 시도
                bool pending = socket.ConnectAsync(args);

                //처리가 끝났을시
                if (pending == false)
                {
                    OnConnectCompleted(null, args);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //연결이 완료됬을시 실행되는 기능
        //TODO : 기능 확인 sender
        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                //생성이 정상적으로 끝났을떄
                if (args.SocketError == SocketError.Success)
                {
                    //세션 생성
                    Session session = _sessionFactory.Invoke();

                    //세션 초기화
                    session.Start(args.ConnectSocket);

                    //연결된 엔드포인트 등록
                    session.OnConnected(args.RemoteEndPoint);
                }
                else
                {
                    Console.WriteLine($"OnConnectCompleted Fail: {args.SocketError}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}