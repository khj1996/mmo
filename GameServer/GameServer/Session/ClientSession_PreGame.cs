using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using GameServer.Data;
using GameServer.DB;
using GameServer.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{
    //계정 로그인 
    public partial class ClientSession : PacketSession
    {
        //계정 id
        public int AccountDbId { get; private set; }

        //플레이어 정보
        //다캐릭 대응
        public List<LobbyPlayerInfo> LobbyPlayers { get; set; } = new();

        //로그인(계정 로그인)
        public void HandleLogin(C_Login loginPacket)
        {
            if (ServerState != PlayerServerState.ServerStateLobby)
                return;

            var accountDbId = int.Parse(JwtUtils.DecipherJwtAccessToken(loginPacket.JwtToken).Subject);


            var checkUser = SessionManager.Instance.FindByAccountDbId(SessionId, accountDbId);

            //중복 유저 로그인시 연결 해제
            if (checkUser != null)
            {
                checkUser.Disconnect();
            }

            LobbyPlayers.Clear();


            using (AppDbContext db = new AppDbContext())
            {
                //계정 정보 획득
                var findAccountGame = db.Accounts
                    .Include(a => a.Players).FirstOrDefault(a => a.AccountGameDbId == accountDbId);

                // AccountDbId 메모리에 기억
                AccountDbId = findAccountGame.AccountGameDbId;

                var loginOk = new S_EnterServer();
                foreach (var playerDb in findAccountGame.Players)
                {
                    var lobbyPlayer = new LobbyPlayerInfo()
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
                        },
                        PosInfo = new PositionInfo()
                        {
                            Pos = new Vec2()
                            {
                                X = playerDb.PosX,
                                Y = playerDb.PosY,
                            }
                        }
                    };

                    // 메모리에도 들고 있다
                    LobbyPlayers.Add(lobbyPlayer);

                    // 패킷에 넣어준다
                    loginOk.Players.Add(lobbyPlayer);
                }


                Send(loginOk);
                // 로비로 이동
                ServerState = PlayerServerState.ServerStateLobby;
            }
        }

        //인게임 입장
        public void HandleEnterGame(C_EnterGame enterGamePacket)
        {
            if (ServerState != PlayerServerState.ServerStateLobby)
                return;

            //선택 캐릭터 데이터 획득
            var playerInfo = LobbyPlayers.Find(p => p.Name == enterGamePacket.Name);

            if (playerInfo == null)
                return;

            //캐릭터 데이터 설정
            MyPlayer = ObjectManager.Instance.Add<Player>();
            {
                MyPlayer.PlayerDbId = playerInfo.PlayerDbId;
                MyPlayer.Info.Name = playerInfo.Name;
                MyPlayer.Info.PosInfo.State = CreatureState.Idle;
                MyPlayer.Move = playerInfo.PosInfo.Move;
                MyPlayer.Pos = playerInfo.PosInfo.Pos;
                MyPlayer.Info.StatInfo.MergeFrom(playerInfo.StatInfo);
                MyPlayer.Session = this;

                var itemListPacket = new S_ItemList();

                // 아이템 목록을 갖고 온다
                using (AppDbContext db = new AppDbContext())
                {
                    var items = db.Items
                        .Where(i => i.OwnerDbId == playerInfo.PlayerDbId)
                        .ToList();

                    foreach (var itemDb in items)
                    {
                        var item = Item.MakeItem(itemDb);

                        if (item != null)
                        {
                            MyPlayer.Inven.Add(item);

                            var info = new ItemInfo();
                            info.MergeFrom(item.Info);
                            itemListPacket.Items.Add(info);
                        }
                    }
                }

                Send(itemListPacket);
            }

            //유저 상태 변경
            ServerState = PlayerServerState.ServerStateGame;

            //룸에 유저 추가
            GameLogic.Instance.Push(() =>
            {
                GameRoom room = GameLogic.Instance.Find(1);
                room.Push(room.EnterGame, MyPlayer);
            });
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
                        AccountGameDbId = AccountDbId,
                        CurMap = 0,
                        PosX = 0,
                        PosY = 0,
                        PosZ = 0
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
                        },
                        PosInfo = new PositionInfo()
                        {
                            Pos = new Vec2()
                            {
                                X = 0,
                                Y = 0,
                            }
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