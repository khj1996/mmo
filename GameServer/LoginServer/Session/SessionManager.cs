using System;
using System.Collections.Generic;
using System.Linq;
using LoginServer.Game;

namespace LoginServer
{
    class SessionManager
    {
        #region 싱글톤

        static SessionManager _session = new SessionManager();

        public static SessionManager Instance
        {
            get { return _session; }
        }

        #endregion

        //세션생성시 id 설정을 위한 값
        int _sessionId = 0;

        //클라이언트 세션 딕셔너리
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();

        //락
        object _lock = new object();

        //현재 접속 유저 리스트화 반환
        public List<ClientSession> GetSessions()
        {
            List<ClientSession> sessions = new List<ClientSession>();

            lock (_lock)
            {
                sessions = _sessions.Values.ToList();
            }
            
            return sessions;
        }

        //세션 생성
        //유저가 접속시 리스너를 통하여 생성
        public ClientSession Generate()
        {
            lock (_lock)
            {
                int sessionId = ++_sessionId;

                ClientSession session = new ClientSession();
                session.SessionId = sessionId;
                _sessions.Add(sessionId, session);

                Console.WriteLine($"Connected ({_sessions.Count}) Players");

                return session;
            }
        }

        public ClientSession FindBySessionId(int id)
        {
            lock (_lock)
            {
                _sessions.TryGetValue(id, out var session);
                return session;
            }
        }

        public ClientSession? FindByAccountDbId(int sessionId, int accountDbId)
        {
            lock (_lock)
            {
                foreach (var session in _sessions.Values)
                {
                    if (session.AccountDbId == accountDbId && session.SessionId != sessionId)
                    {
                        return session;
                    }
                }
            }

            return null;
        }

        //세션 제거
        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.SessionId);
                Console.WriteLine($"Connected ({_sessions.Count}) Players");
            }
        }
    }
}