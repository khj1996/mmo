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
                AccountDb findAccount = db.Accounts
                    .Include(a => a.Players)
                    .Where(a => a.AccountDbId == accountDbId).FirstOrDefault();


                //서버 정보 획득
                List<ServerDb> serverDbs = db.Servers
                    .Where(i => i.ServerDbId != null)
                    .ToList();

                //기존 계정
                if (findAccount != null)
                {
                    // AccountDbId 메모리에 기억
                    AccountDbId = findAccount.AccountDbId;

                    S_Login loginOk = new S_Login() { LoginOk = 1 };
                    foreach (PlayerDb playerDb in findAccount.Players)
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
                        loginOk.Players.Add(lobbyPlayer);
                    }

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
                else
                {
                    //신규 계정
                    AccountDb newAccount = new AccountDb() { AccountName = $"Player_{accountDbId}" };
                    db.Accounts.Add(newAccount);
                    bool success = db.SaveChangesEx();
                    if (success == false)
                        return;

                    // AccountDbId 메모리에 기억
                    AccountDbId = newAccount.AccountDbId;

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

        //인게임 입장
        public void HandleEnterGame(C_EnterGame enterGamePacket)
        {
            if (ServerState != PlayerServerState.ServerStateLobby)
                return;

            //선택 캐릭터 데이터 획득
            LobbyPlayerInfo playerInfo = LobbyPlayers.Find(p => p.Name == enterGamePacket.Name);
            if (playerInfo == null)
                return;


            //유저 상태 변경
            ServerState = PlayerServerState.ServerStateGame;

        }

        //유저 생성
        public void HandleCreatePlayer(C_CreatePlayer createPacket)
        {
            // TODO : 이런 저런 보안 체크
            if (ServerState != PlayerServerState.ServerStateLobby)
                return;

            using (AppDbContext db = new AppDbContext())
            {
                PlayerDb findPlayer = db.Players
                    .Where(p => p.PlayerName == createPacket.Name).FirstOrDefault();

                if (findPlayer != null)
                {
                    // 이름이 겹친다
                    Send(new S_CreatePlayer());
                }
                else
                {
                    // 1레벨 스탯 정보 추출
                    StatInfo stat = null;
                    DataManager.StatDict.TryGetValue(1, out stat);

                    // DB에 플레이어 만들어줘야 함
                    PlayerDb newPlayerDb = new PlayerDb()
                    {
                        PlayerName = createPacket.Name,
                        Level = stat.Level,
                        Hp = stat.Hp,
                        MaxHp = stat.MaxHp,
                        Attack = stat.Attack,
                        Speed = stat.Speed,
                        TotalExp = 0,
                        AccountDbId = AccountDbId
                    };

                    db.Players.Add(newPlayerDb);
                    bool success = db.SaveChangesEx();
                    if (success == false)
                        return;

                    // 메모리에 추가
                    LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
                    {
                        PlayerDbId = newPlayerDb.PlayerDbId,
                        Name = createPacket.Name,
                        StatInfo = new StatInfo()
                        {
                            Level = stat.Level,
                            Hp = stat.Hp,
                            MaxHp = stat.MaxHp,
                            Attack = stat.Attack,
                            Speed = stat.Speed,
                            TotalExp = 0
                        }
                    };

                    // 메모리에도 들고 있다
                    LobbyPlayers.Add(lobbyPlayer);

                    // 클라에 전송
                    S_CreatePlayer newPlayer = new S_CreatePlayer() { Player = new LobbyPlayerInfo() };
                    newPlayer.Player.MergeFrom(lobbyPlayer);

                    Send(newPlayer);
                }
            }
        }
    }
}