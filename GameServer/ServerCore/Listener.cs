using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    //
    public class Listener
    {
        //연결 수신을 위한 소켓
        Socket _listenSocket;
        //등록된 함수를 invoke하여 세션을 생성해주는 대리자
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            //매개변수로 받은 엔드포인트를 이용하여 소켓 생성
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //세션 생성시 실행하려는 기능
            _sessionFactory += sessionFactory;

            //클라이언트에서 연결 시도를 위한 엔드포인트 등록
            _listenSocket.Bind(endPoint);

            
            //대기가 가능한 수
            _listenSocket.Listen(backlog);

            //수신 작업 등록
            for (int i = 0; i < register; i++)
            {
                //송신과 수신을 담당하는 클래스
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                //작업이 완료되면 실행하는 기능
                args.Completed += OnAcceptCompleted;
                //연결 수신부
                RegisterAccept(args);
            }
            Console.WriteLine("Init Complete");
        }
        
        //수신 작업 등록
        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            try
            {
                //비동기 수신
                bool pending = _listenSocket.AcceptAsync(args);
                if (pending == false)
                    OnAcceptCompleted(null, args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //수신 작업 완료시 실행하는 콜백함수
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                //성공적으로 완료시
                if (args.SocketError == SocketError.Success)
                {
                    //세션 생성
                    Session session = _sessionFactory.Invoke();
                    
                    //세션 초기화
                    session.Start(args.ConnectSocket);
                    
                    //수신 엔드포인트 등록
                    session.OnConnected(args.AcceptSocket.RemoteEndPoint);
                }
                else
                    Console.WriteLine(args.SocketError.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            RegisterAccept(args);
        }
    }
}