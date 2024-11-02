using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameServer.DB;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Game
{
    public struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int Y;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
                return 0;
            return F < other.F ? 1 : -1;
        }
    }

    public struct Vector2Float
    {
        public float x;
        public float y;

        private const float Epsilon = 1e-5f;

        public Vector2Float(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2Float Up => new(0, 1);
        public static Vector2Float Down => new(0, -1);
        public static Vector2Float Left => new(-1, 0);
        public static Vector2Float Right => new(1, 0);

        public static Vector2Float operator +(Vector2Float a, Vector2Float b) => new(a.x + b.x, a.y + b.y);
        public static Vector2Float operator -(Vector2Float a, Vector2Float b) => new(a.x - b.x, a.y - b.y);

        public static bool operator ==(Vector2Float a, Vector2Float b) =>
            MathF.Abs(a.x - b.x) < Epsilon && MathF.Abs(a.y - b.y) < Epsilon;

        public static bool operator !=(Vector2Float a, Vector2Float b) => !(a == b);

        public override bool Equals(object obj) => obj is Vector2Float other && this == other;

        public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();

        public Vector2Float Normalized => new(x / Magnitude, y / Magnitude);

        public float Magnitude => MathF.Sqrt(SqrMagnitude);

        public float SqrMagnitude => x * x + y * y;
    }

    public class Map
    {
        public int MapId { get; set; }

        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }

        public int SizeX
        {
            get { return MaxX - MinX + 1; }
        }

        public int SizeY
        {
            get { return MaxY - MinY + 1; }
        }

        //벽등 충돌체
        bool[,] _collision;


        //객체 제거시
        public bool ApplyLeave(GameObject gameObject)
        {
            if (gameObject.Room == null)
                return false;
            if (gameObject.Room.Map != this)
                return false;

            PositionInfo posInfo = gameObject.Info.PosInfo;
            if (posInfo.Pos.X < MinX || posInfo.Pos.X > MaxX)
                return false;
            if (posInfo.Pos.Y < MinY || posInfo.Pos.Y > MaxY)
                return false;

            // Zone
            Zone zone = gameObject.Room.GetZone(gameObject._Pos);
            zone.Remove(gameObject);

            return true;
        }

        private const float Tolerance = 0.001f; // 작은 오차 값

        // 맵 경계 확인 함수
        private bool IsWithinBounds(Vector2Float pos)
        {
            return pos.x >= MinX && pos.x <= MaxX && pos.y >= MinY && pos.y <= MaxY;
        }

        private bool IsWithinTileBounds(int tileX, int tileY)
        {
            return tileX >= 0 && tileX < SizeX - 1 && tileY >= 0 && tileY < SizeY - 1;
        }


        // 충돌 검사 함수
        public bool CanGo(Vector2Float curPos, Vector2Float destPos)
        {
            // 범위 확인
            if (!IsWithinBounds(destPos))
                return false;

            // 좌표 변환
            var (curX, curY) = GetTilePos(curPos.x, curPos.y);
            var (destX, destY) = GetTilePos(destPos.x, destPos.y);

            // Bresenham 알고리즘을 사용해 경로 상의 타일을 검사
            int dx = Math.Abs(destX - curX);
            int dy = Math.Abs(destY - curY);
            int sx = curX < destX ? 1 : -1;
            int sy = curY < destY ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                // 현재 타일이 충돌 타일인지 검사
                if (_collision[curY, curX])
                    return false;

                // 도착지에 도달했을 경우 종료
                if (curX == destX && curY == destY) break;

                // Bresenham의 알고리즘에 따라 이동
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    curX += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    curY += sy;
                }
            }

            return true; // 충돌 없이 도착
        }


        // 존 이동 처리 함수
        private void HandleZoneTransition(GameObject gameObject, Vector2Float currentPos, Vector2Float dest)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            var nowZone = gameObject.Room.GetZone(currentPos);
            var afterZone = gameObject.Room.GetZone(dest);

            if (nowZone != afterZone)
            {
                switch (type)
                {
                    case GameObjectType.Player:
                        nowZone.Players.Remove((Player)gameObject);
                        afterZone.Players.Add((Player)gameObject);
                        break;
                    case GameObjectType.Monster:
                        nowZone.Monsters.Remove((Monster)gameObject);
                        afterZone.Monsters.Add((Monster)gameObject);
                        break;
                    case GameObjectType.Projectile:
                        nowZone.Projectiles.Remove((Projectile)gameObject);
                        afterZone.Projectiles.Add((Projectile)gameObject);
                        break;
                }
            }
        }

        private (int tileX, int tileY) GetTilePos(float x, float y)
        {
            int tileX = (int)MathF.Floor(x - MinX);
            int tileY = (int)MathF.Floor(MaxY - y);
            return (tileX, tileY);
        }

        private (int x, int y) GetLocalPos(int tileX, int tileY)
        {
            int x = tileX + MinX;
            int y = MaxY - tileY;
            return (x, y);
        }

        private void AdjustDestinationForCollision(ref Vector2Float dest, Vec2 moveDirection, float diffX, float diffY)
        {
            float epsilon = 0.2f;

            if (diffX > 0)
            {
                if (moveDirection.X > 0)
                {
                    dest.x = dest.x - (1 - diffX) - epsilon;
                }
                else if (moveDirection.X < 0)
                {
                    dest.x = dest.x + (1 - diffX) + epsilon;
                }
            }

            if (diffY > 0)
            {
                if (moveDirection.Y > 0)
                {
                    dest.y = dest.y - (1 - diffY) - epsilon;
                }
                else if (moveDirection.Y < 0)
                {
                    dest.y = dest.y + (1 - diffY) + epsilon;
                }
            }
        }

        private bool CheckCollisionInTiles(ref Vector2Float pos, Vec2 move, ObjectSize size)
        {
            float left = pos.x - size.Left;
            float bottom = pos.y - size.Bottom;
            float right = pos.x + size.Right;
            float top = pos.y + size.Top;

            var (tileLeft, tileBottom) = GetTilePos(left, bottom);
            var (tileRight, tileTop) = GetTilePos(right, top);
            var (currentTileX, currentTileY) = GetTilePos(pos.x, pos.y);

            float diffX = 0f;
            float diffY = 0f;

            for (int x = tileLeft; x <= tileRight; x++)
            {
                if (_collision[currentTileY, x])
                {
                    float tileMinX = x + MinX;
                    float tileMaxX = x + MinX + 1;

                    if (right > tileMinX && left < tileMaxX)
                    {
                        Console.WriteLine(right + "   " + tileMaxX);
                        diffX = (right > tileMaxX) ? right - tileMaxX : tileMinX - left;
                    }

                    AdjustDestinationForCollision(ref pos, move, diffX, 0);
                    break;
                }
            }

            for (int y = tileTop; y <= tileBottom; y++)
            {
                if (_collision[y, currentTileX])
                {
                    float tileMinY = MaxY - (y + 1);
                    float tileMaxY = MaxY - y;

                    if (top > tileMinY && bottom < tileMaxY)
                    {
                        diffY = (top > tileMaxY) ? top - tileMaxY : tileMinY - bottom;
                    }

                    AdjustDestinationForCollision(ref pos, move, 0, diffY);
                    return false;
                }
            }

            return true;
        }


        // 이동 및 충돌 처리 함수
        public bool ApplyMove(GameObject gameObject, Vector2Float dest, Vec2? moveDirection = null)
        {
            if (gameObject.Room == null || gameObject.Room.Map != this)
                return false;

            Vector2Float currentPos = gameObject._Pos;
            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            switch (type)
            {
                case GameObjectType.Player or GameObjectType.Monster when moveDirection != null:
                {
                    CheckCollisionInTiles(ref dest, gameObject.Move, gameObject.size);

                    break;
                }
                case GameObjectType.Projectile:
                {
                    var result = CanGo(currentPos, dest);

                    if (!result)
                    {
                        return result;
                    }

                    break;
                }
            }

            // 존 이동 처리
            HandleZoneTransition(gameObject, currentPos, dest);

            // 실제 좌표 이동
            gameObject.Pos.X = dest.x;
            gameObject.Pos.Y = dest.y;
            return true; // 성공적으로 이동
        }


        //맵 데이터 획득
        public void LoadMap(int mapId, string pathPrefix)
        {
            using AppDbContext db = new AppDbContext();


            MapDb findMapData = db.MapDatas.Where(a => a.MapDbId == mapId).FirstOrDefault();

            MapId = mapId;

            if (findMapData == null)
            {
                StringBuilder tileStr = new StringBuilder();

                MapId = mapId;

                string mapName = "Map_" + mapId.ToString("000");

                // Collision 관련 파일
                string text = File.ReadAllText($"{pathPrefix}/{mapName}.txt");
                StringReader reader = new StringReader(text);

                MapDb newMapData = new MapDb()
                {
                    MapDbId = mapId,
                    MinX = int.Parse(reader.ReadLine()),
                    MaxX = int.Parse(reader.ReadLine()),
                    MinY = int.Parse(reader.ReadLine()),
                    MaxY = int.Parse(reader.ReadLine()),
                };


                for (int y = 0; y < newMapData.MaxY - newMapData.MinY; y++)
                {
                    tileStr.Append((newMapData.MaxY - newMapData.MinY - 1 != y)
                        ? reader.ReadLine() + ","
                        : reader.ReadLine());
                }

                newMapData.TileInfo = tileStr.ToString();

                db.MapDatas.Add(newMapData);
                bool success = db.SaveChangesEx();

                if (success == false)
                    return;

                findMapData = newMapData;
            }


            MinX = findMapData.MinX;
            MaxX = findMapData.MaxX;
            MinY = findMapData.MinY;
            MaxY = findMapData.MaxY;


            int xCount = MaxX - MinX;
            int yCount = MaxY - MinY;
            _collision = new bool[yCount, xCount];

            var tileData = findMapData.TileInfo.Split(",");

            for (int y = 0; y < yCount; y++)
            {
                string line = tileData[y];
                for (int x = 0; x < xCount; x++)
                {
                    _collision[y, x] = (line[x] == '1');
                }
            }
        }

        #region A* PathFinding

        int[] _deltaY = new int[] { 1, -1, 0, 0 };
        int[] _deltaX = new int[] { 0, 0, -1, 1 };
        int[] _cost = new int[] { 10, 10, 10, 10 };

        public List<(int tileX, int tileY)> FindPath(Vector2Float start, Vector2Float dest, int maxDist = 10)
        {
            HashSet<(int tileX, int tileY)> closedSet = new HashSet<(int tileX, int tileY)>();
            Dictionary<(int tileX, int tileY), float> openSet = new Dictionary<(int tileX, int tileY), float>();
            Dictionary<(int tileX, int tileY), (int tileX, int tileY)> parentMap = new Dictionary<(int tileX, int tileY), (int tileX, int tileY)>();
            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            var startTilePos = GetTilePos(start.x, start.y);
            var destTilePos = GetTilePos(dest.x, dest.y);
            int initialH = 10 * (Math.Abs(destTilePos.tileY - startTilePos.tileY) + Math.Abs(destTilePos.tileX - startTilePos.tileX));
            pq.Push(new PQNode { F = initialH, G = 0, X = startTilePos.tileX, Y = startTilePos.tileY });
            openSet[startTilePos] = initialH;
            parentMap[startTilePos] = startTilePos;

            while (pq.Count > 0)
            {
                var currentNode = pq.Pop();
                (int tileX, int tileY) currentPos = (currentNode.X, currentNode.Y);

                if (currentPos == destTilePos)
                    break;

                closedSet.Add(currentPos);

                for (int i = 0; i < _deltaY.Length; i++)
                {
                    (int tileX, int tileY) nextPos = (currentPos.tileX + _deltaX[i], currentPos.tileY + _deltaY[i]);


                    if (closedSet.Contains(nextPos) ||
                        Math.Abs(startTilePos.tileY - nextPos.tileY) + Math.Abs(startTilePos.tileX - nextPos.tileX) > maxDist ||
                        !IsWithinTileBounds(nextPos.tileX, nextPos.tileY) ||
                        !CanPass(nextPos))
                        continue;

                    int g = currentNode.G + _cost[i];
                    int h = 10 * (Math.Abs(destTilePos.tileY - nextPos.tileY) + Math.Abs(destTilePos.tileX - nextPos.tileX));
                    int f = g + h;

                    if (!openSet.TryGetValue(nextPos, out float existingF) || existingF > f)
                    {
                        openSet[nextPos] = f;
                        pq.Push(new PQNode { F = f, G = g, X = nextPos.tileX, Y = nextPos.tileY });
                        parentMap[nextPos] = currentPos;
                    }
                }
            }

            return ExtractPath(parentMap, destTilePos);
        }

        private bool CanPass((int tileX, int tileY) pos)
        {
            return !_collision[pos.tileY, pos.tileX];
        }

        private List<(int tileX, int tileY)> ExtractPath(Dictionary<(int tileX, int tileY), (int tileX, int tileY)> parentMap, (int tileX, int tileY) destination)
        {
            List<(int tileX, int tileY)> path = new List<(int tileX, int tileY)>();
            (int tileX, int tileY) step = destination;
            while (parentMap.TryGetValue(step, out (int tileX, int tileY) parent) && parent != step)
            {
                path.Add(GetLocalPos(step.tileX, step.tileY));
                step = parent;
            }

            path.Reverse();
            return path;
        }

        #endregion
    }
}