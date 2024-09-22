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
            // TODO : 이런 저런 보안 체크
            if (ServerState != PlayerServerState.ServerStateLogin)
                return;

            var accountDbId = int.Parse(JwtUtils.DecipherJwtAccessToken(loginPacket.JwtToken).Subject);

            // TODO : 문제가 있긴 있다
            // - 동시에 다른 사람이 같은 UniqueId을 보낸다면?
            // - 악의적으로 여러번 보낸다면
            // - 쌩뚱맞은 타이밍에 그냥 이 패킷을 보낸다면?

            LobbyPlayers.Clear();


            using (AppDbContext db = new AppDbContext())
            {
                //계정 정보 획득
                AccountGameDb findAccountGame = db.Accounts
                    .Include(a => a.Players)
                    .Where(a => a.AccountGameDbId == accountDbId).FirstOrDefault();


                //서버 정보 획득
                List<ServerDb> serverDbs = db.Servers
                    .Where(i => i.ServerDbId != null)
                    .ToList();

                //기존 계정
                if (findAccountGame != null)
                {
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


            /*foreach (PlayerDb playerDb in findAccount.Players)
            {
                LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
                {
                    PlayerDbId = playerDb.PlayerDbId,
                    Name = playerDb.PlayerName,
                    StatInfo = new StatInfo()
                    {
                        Level = playerDb.Level,
                        Hp = playerDb.Hp,
                        MaxHp = playerDb.MaxHp,
                        Attack = playerDb.Attack,
                        Speed = playerDb.Speed,
                        TotalExp = playerDb.TotalExp
                    }
                };

                // 메모리에도 들고 있다
                LobbyPlayers.Add(lobbyPlayer);

                // 패킷에 넣어준다
                loginOk.Players.Add(lobbyPlayer);*/
        }


        //인게임 입장
        public void HandleEnterGame(C_EnterGame enterGamePacket)
        {
        }

        //유저 생성
        public void HandleCreatePlayer(C_CreatePlayer createPacket)
        {
        }
    }
}