using Google.Protobuf;
using Google.Protobuf.Protocol;
using GameServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        //플레이어 시야
        public const int VisionDis = 10;

        //방 id
        public int RoomId { get; set; }
        public float tickRate = 20f; // 20틱 per second (50ms 간격)
        public int tickInterval => (int)(1000 / tickRate);


        //방 객체 관리를 위한 딕셔너리
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

        public Zone[,] Zones { get; private set; }
        public int ZoneCells { get; private set; }

        public Map Map { get; private set; } = new Map();


        // ㅁㅁㅁ
        // ㅁㅁㅁ
        // ㅁㅁㅁ
        public Zone GetZone(Vector2Float cellPos)
        {
            float x = (cellPos.x - Map.MinX) / ZoneCells;
            float y = (Map.MaxY - cellPos.y) / ZoneCells;
            return GetZone((int)y, (int)x);
        }

        public Zone GetZone(int indexY, int indexX)
        {
            if (indexX < 0 || indexX >= Zones.GetLength(1))
                return null;
            if (indexY < 0 || indexY >= Zones.GetLength(0))
                return null;

            return Zones[indexY, indexX];
        }

        public void Init(int mapId, int zoneCells)
        {
            Map.LoadMap(mapId, ConfigManager.Config.mapDataPath);

            ZoneCells = zoneCells; // 10

            int countY = (Map.SizeY + zoneCells - 1) / zoneCells;
            int countX = (Map.SizeX + zoneCells - 1) / zoneCells;
            Zones = new Zone[countY, countX];
            for (int y = 0; y < countY; y++)
            {
                for (int x = 0; x < countX; x++)
                {
                    Zones[y, x] = new Zone(y, x);
                }
            }

            //몬스터 생성
            /*for (int i = 0; i < 1; i++)
            {
                Monster monster = ObjectManager.Instance.Add<Monster>();
                monster.Init(1);
                SetRandomPos(monster);
                EnterGame(monster);
            }*/
        }

        // 누군가 주기적으로 호출해줘야 한다
        public void Update()
        {
            Flush();
        }

        Random _rand = new Random();

        public void SetRandomPos(GameObject gameObject)
        {
            gameObject.Pos.X = 5;
            gameObject.Pos.Y = 5;
        }

        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            Vector2Float spawnPos;
            spawnPos.x = gameObject.Pos.X;
            spawnPos.y = gameObject.Pos.Y;


            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            if (type == GameObjectType.Player)
            {
                Player player = gameObject as Player;
                _players.Add(gameObject.Id, player);
                player.Room = this;

                player.RefreshAdditionalStat();

                Map.ApplyMove(player, new Vector2Float(player.CellPos.x, player.CellPos.y));
                GetZone(player.CellPos).Players.Add(player);

                // 본인한테 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();

                    enterPacket.Player = player.Info;
                    enterPacket.Player.PosInfo.Move = enterPacket.Player.PosInfo.Pos;
                    player.Session.Send(enterPacket);
                    player.Vision.Update();
                    player.Update();
                }

                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                Broadcast(gameObject.CellPos, spawnPacket, player.PlayerDbId);
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = gameObject as Monster;
                _monsters.Add(gameObject.Id, monster);
                monster.Room = this;

                GetZone(monster.CellPos).Monsters.Add(monster);
                Map.ApplyMove(monster, new Vector2Float(monster.CellPos.x, monster.CellPos.y));

                monster.Update();

                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                Broadcast(gameObject.CellPos, spawnPacket);
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = gameObject as Projectile;
                _projectiles.Add(gameObject.Id, projectile);
                projectile.Room = this;

                GetZone(projectile.CellPos).Projectiles.Add(projectile);
                projectile.Start(100);
            }
        }

        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

            Vector2Float cellPos;

            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (_players.Remove(objectId, out player) == false)
                    return;

                cellPos = player.CellPos;

                player.OnLeaveGame();
                Map.ApplyLeave(player);
                player.Room = null;

                // 본인한테 정보 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = null;
                if (_monsters.Remove(objectId, out monster) == false)
                    return;

                cellPos = monster.CellPos;
                Map.ApplyLeave(monster);
                monster.Room = null;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(objectId, out projectile) == false)
                    return;

                cellPos = projectile.CellPos;
                Map.ApplyLeave(projectile);
                projectile.Room = null;
            }
            else
            {
                return;
            }

            // 타인한테 정보 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.ObjectIds.Add(objectId);
                Broadcast(cellPos, despawnPacket);
            }
        }

        public Player FindPlayer(Func<GameObject, bool> condition)
        {
            foreach (Player player in _players.Values)
            {
                if (condition.Invoke(player))
                    return player;
            }

            return null;
        }

        public List<Player> FindSquarePlayer(Vector2Float point, float radius)
        {
            float maxY = point.y + radius;
            float minY = point.y - radius;
            float maxX = point.x + radius;
            float minX = point.x - radius;

            var targetList = _players.Values.Where(x =>
                x.Pos.Y >= minY &&
                x.Pos.Y <= maxY &&
                x.Pos.X >= minX &&
                x.Pos.X <= maxX).ToList();

            return targetList;
        }

        // 살짝 부담스러운 함수
        public Player FindClosestPlayer(Vector2Float pos, int range)
        {
            List<Player> players = GetAdjacentPlayers(pos, range);

            players.Sort((left, right) =>
            {
                float leftDist = (left.CellPos - pos).CellDistFromZero;
                float rightDist = (right.CellPos - pos).CellDistFromZero;
                return (int)leftDist - (int)rightDist;
            });

            foreach (Player player in players)
            {
                List<Vector2Float> path = Map.FindPath(pos, player.CellPos, checkObjects: true);
                if (path.Count < 2 || path.Count > range)
                    continue;

                return player;
            }

            return null;
        }

        public void Broadcast(Vector2Float pos, IMessage packet, int playerDbId = -1)
        {
            List<Zone> zones = GetAdjacentZones(pos);

            foreach (var p in zones.SelectMany(z => z.Players))
            {
                if (p.PlayerDbId == playerDbId)
                    continue;

                var dis = (p.CellPos - pos).Magnitude;

                if (dis > VisionDis)
                    continue;

                p.Session.Send(packet);
            }
        }

        public List<Player> GetAdjacentPlayers(Vector2Float pos, int range)
        {
            List<Zone> zones = GetAdjacentZones(pos, range);
            return zones.SelectMany(z => z.Players).ToList();
        }

        // ㅁㅁㅁㅁㅁㅁ
        // ㅁㅁㅁㅁㅁㅁ
        // ㅁㅁㅁㅁㅁㅁ
        // ㅁㅁㅁㅁㅁㅁ
        public List<Zone> GetAdjacentZones(Vector2Float cellPos, int range = GameRoom.VisionDis)
        {
            HashSet<Zone> zones = new HashSet<Zone>();

            float maxY = cellPos.y + range;
            float minY = cellPos.y - range;
            float maxX = cellPos.x + range;
            float minX = cellPos.x - range;

            // 좌측 상단
            Vector2Float leftTop = new Vector2Float(minX, maxY);
            float minIndexY = (Map.MaxY - leftTop.y) / ZoneCells;
            float minIndexX = (leftTop.x - Map.MinX) / ZoneCells;

            // 우측 하단
            Vector2Float rightBot = new Vector2Float(maxX, minY);
            float maxIndexY = (Map.MaxY - rightBot.y) / ZoneCells;
            float maxIndexX = (rightBot.x - Map.MinX) / ZoneCells;

            for (int x = (int)minIndexX; x <= maxIndexX; x++)
            {
                for (int y = (int)minIndexY; y <= maxIndexY; y++)
                {
                    Zone zone = GetZone(y, x);
                    if (zone == null)
                        continue;

                    zones.Add(zone);
                }
            }

            return zones.ToList();
        }
    }
}