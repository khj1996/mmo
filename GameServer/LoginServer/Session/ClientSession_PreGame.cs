using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using LoginServer.Data;
using LoginServer.DB;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoginServer
{
    //계정 로그인 
    public partial class ClientSession : PacketSession
    {
        //계정 id
        public int AccountDbId { get; private set; }

        //플레이어 정보
        //다캐릭 대응
        public List<LobbyPlayerInfo> LobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();

        //로그인(계정 로그인)
        public void HandleLogin(C_Login loginPacket)
        {
            if (ServerState != PlayerServerState.ServerStateLogin)
                return;

            var accountDbId = int.Parse(JwtUtils.DecipherJwtAccessToken(loginPacket.JwtToken).Subject);


            var checkUser = SessionManager.Instance.FindByAccountDbId(SessionId, accountDbId);


            //중복 유저 로그인시 연결 해제
            if (checkUser != null)
            {
                checkUser.Disconnect();
            }

            //redis를 이용하여 추가
            SessionManager.Instance._redisDb.StringSet($"user:{accountDbId}:isLoggedIn", true, TimeSpan.FromHours(1));
            

            LobbyPlayers.Clear();

            using (AppDbContext db = new AppDbContext())
            {
                //계정 정보 획득
                var findAccountGame = db.Accounts
                    .Include(a => a.Players).FirstOrDefault(a => a.AccountGameDbId == accountDbId);


                //서버 정보 획득
                var serverDbs = db.Servers
                    .Where(x => true)
                    .ToList();

                //기존 계정
                if (findAccountGame != null)
                {
                    findAccountGame.JwtToken = loginPacket.JwtToken;

                    db.Entry(findAccountGame).Property(nameof(AccountGameDb.JwtToken)).IsModified = true;

                    bool success = db.SaveChangesEx();

                    if (success == false)
                        return;

                    AccountDbId = findAccountGame.AccountGameDbId;
                }
                else
                {
                    //신규 계정
                    AccountGameDb newAccountGame = new AccountGameDb()
                    {
                        AccountGameDbId = accountDbId,
                        AccountName = $"Player_{accountDbId}",
                        JwtToken = loginPacket.JwtToken
                    };
                    db.Accounts.Add(newAccountGame);
                    bool success = db.SaveChangesEx();
                    if (success == false)
                        return;

                    // AccountDbId 메모리에 기억
                    AccountDbId = newAccountGame.AccountGameDbId;
                }


                S_Login loginOk = new S_Login() { LoginOk = 1 };


                foreach (var serverDb in serverDbs)
                {
                    ServerInfo serverInfo = new ServerInfo()
                    {
                        Name = serverDb.Name,
                        IpAddress = serverDb.IpAddress,
                        Port = serverDb.Port,
                        BusyScore = serverDb.BusyScore,
                    };
                    loginOk.ServerInfos.Add(serverInfo);
                }


                Send(loginOk);
                // 로비로 이동
                ServerState = PlayerServerState.ServerStateLobby;
            }
        }
    }
}