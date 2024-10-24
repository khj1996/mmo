﻿using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameServer.DB;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Game
{
    //위치
    public struct Pos
    {
        public Pos(int y, int x)
        {
            Y = y;
            X = x;
        }

        public float Y;
        public float X;

        public static bool operator ==(Pos lhs, Pos rhs)
        {
            return Math.Abs(lhs.Y - rhs.Y) < 0.01 && Math.Abs(lhs.X - rhs.X) < 0.01;
        }

        public static bool operator !=(Pos lhs, Pos rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return (Pos)obj == this;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    //A*알고리즘
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

        public Vector2Float(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2Float Up => new(0, 1);
        public static Vector2Float Down => new(0, -1);
        public static Vector2Float Left => new(-1, 0);
        public static Vector2Float Right => new(1, 0);

        public static Vector2Float operator +(Vector2Float a, Vector2Float b)
        {
            return new Vector2Float(a.x + b.x, a.y + b.y);
        }

        public static Vector2Float operator -(Vector2Float a, Vector2Float b)
        {
            return new Vector2Float(a.x - b.x, a.y - b.y);
        }

        public float Magnitude => MathF.Sqrt(SqrMagnitude);

        public float SqrMagnitude => (x * x + y * y);

        public float CellDistFromZero => Math.Abs(x) + Math.Abs(y);
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
            Zone zone = gameObject.Room.GetZone(gameObject.CellPos);
            zone.Remove(gameObject);

            return true;
        }

        private const float Tolerance = 0.001f; // 작은 오차 값

        // 맵 경계 확인 함수
        private bool IsWithinBounds(Vector2Float pos)
        {
            return pos.x >= MinX && pos.x <= MaxX && pos.y >= MinY && pos.y <= MaxY;
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

        private void AdjustDestinationForCollision(ref Vector2Float dest, Vec2 moveDirection, float diffX, float diffY)
        {
            float epsilon = 0.2f;

            // X 방향 이동 처리 (오른쪽 이동 중)
            if (moveDirection.X > 0 && diffX > 0)
            {
                dest.x = dest.x - diffX - epsilon;
            }
            else if (moveDirection.X < 0 && diffX > 0)
            {
                dest.x = dest.x + diffX + epsilon;
            }

            if (moveDirection.Y > 0 && diffY > 0)
            {
                dest.y = dest.y - (1 - diffY) - epsilon;
            }
            else if (moveDirection.Y < 0 && diffY > 0)
            {
                dest.y = dest.y + (1 - diffY) + epsilon;
            }
        }

        private bool CheckCollisionInTiles(ref Vector2Float pos, Vec2 move, ObjectSize size)
        {
            float leftBottomX = pos.x - size.Left;
            float leftBottomY = pos.y - size.Bottom;
            float rightTopX = pos.x + size.Right;
            float rightTopY = pos.y + size.Top;

            var (tileLeftBottomX, tileLeftBottomY) = GetTilePos(leftBottomX, leftBottomY);
            var (tileRightTopX, tileRightTopY) = GetTilePos(rightTopX, rightTopY);

            for (int x = tileLeftBottomX; x <= tileRightTopX; x++)
            {
                for (int y = tileRightTopY; y <= tileLeftBottomY; y++)
                {
                    if (_collision[y, x])
                    {
                        float tileMinX = x + MinX;
                        float tileMaxX = x + MinX + 1;
                        float tileMinY = MaxY - (y + 1);
                        float tileMaxY = MaxY - y;

                        float diffX = 0f;
                        float diffY = 0f;

                        if (rightTopX > tileMinX)
                        {
                            if (leftBottomX < tileMaxX)
                            {
                                if (rightTopX > tileMaxX)
                                {
                                    diffX = rightTopX - tileMaxX;
                                }
                                else
                                {
                                    diffX = tileMinX - leftBottomX;
                                }
                            }
                        }

                        if (rightTopY > tileMinY)
                        {
                            if (leftBottomY < tileMaxY)
                            {
                                if (rightTopY > tileMaxY)
                                {
                                    diffY = rightTopY - tileMaxY;
                                }
                                else
                                {
                                    diffY = tileMinY - leftBottomY;
                                }
                            }
                        }

                        AdjustDestinationForCollision(ref pos, move, diffX, diffY);
                        return false;
                    }
                }
            }

            return true;
        }

        // 이동 및 충돌 처리 함수
        public bool ApplyMove(GameObject gameObject, Vector2Float dest, Vec2? moveDirection = null)
        {
            if (gameObject.Room == null || gameObject.Room.Map != this)
                return false;

            Vector2Float currentPos = gameObject.CellPos;
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

        // U D L R  //  UL UR DL DR
        int[] _deltaY = new int[] { 1, -1, 0, 0, 1, 1, -1, -1 };
        int[] _deltaX = new int[] { 0, 0, -1, 1, -1, -1, 1, 1 };
        int[] _cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 };

        public List<Vector2Float> FindPath(Vector2Float startCellPos, Vector2Float destCellPos,
            bool checkObjects = true, int maxDist = 10)
        {
            List<Pos> path = new List<Pos>();

            // 점수 매기기
            // F = G + H
            // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
            // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
            // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

            // (y, x) 이미 방문했는지 여부 (방문 = closed 상태)
            HashSet<Pos> closeList = new HashSet<Pos>(); // CloseList

            // (y, x) 가는 길을 한 번이라도 발견했는지
            // 발견X => MaxValue
            // 발견O => F = G + H
            Dictionary<Pos, int> openList = new Dictionary<Pos, int>(); // OpenList
            Dictionary<Pos, Pos> parent = new Dictionary<Pos, Pos>();

            // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            // CellPos -> ArrayPos
            Pos pos = Cell2Pos(startCellPos);
            Pos dest = Cell2Pos(destCellPos);

            // 시작점 발견 (예약 진행)
            openList.Add(pos, 10 * (int)(Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)));

            pq.Push(new PQNode()
            {
                F = 10 * (int)(Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = (int)pos.Y,
                X = (int)pos.X
            });
            parent.Add(pos, pos);

            while (pq.Count > 0)
            {
                // 제일 좋은 후보를 찾는다
                PQNode pqNode = pq.Pop();
                Pos node = new Pos(pqNode.Y, pqNode.X);
                // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
                if (closeList.Contains(node))
                    continue;

                // 방문한다
                closeList.Add(node);

                // 목적지 도착했으면 바로 종료
                if (node.Y == dest.Y && node.X == dest.X)
                    break;

                // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
                for (int i = 0; i < _deltaY.Length; i++)
                {
                    Pos next = new Pos((int)node.Y + _deltaY[i], (int)node.X + _deltaX[i]);

                    // 너무 멀면 스킵
                    if (Math.Abs(pos.Y - next.Y) + Math.Abs(pos.X - next.X) > maxDist)
                        continue;

                    // 유효 범위를 벗어났으면 스킵
                    // 벽으로 막혀서 갈 수 없으면 스킵
                    if (next.Y != dest.Y || next.X != dest.X)
                    {
                        /*if (CanGo(Pos2Cell(next)) == false) // CellPos
                            continue;*/
                    }

                    // 이미 방문한 곳이면 스킵
                    if (closeList.Contains(next))
                        continue;

                    // 비용 계산
                    int g = 0; // node.G + _cost[i];
                    int h = 10 * (int)((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                    // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵

                    int value = 0;
                    if (openList.TryGetValue(next, out value) == false)
                        value = Int32.MaxValue;

                    if (value < g + h)
                        continue;

                    // 예약 진행
                    if (openList.TryAdd(next, g + h) == false)
                        openList[next] = g + h;

                    pq.Push(new PQNode() { F = g + h, G = g, Y = (int)next.Y, X = (int)next.X });

                    if (parent.TryAdd(next, node) == false)
                        parent[next] = node;
                }
            }

            return CalcCellPathFromParent(parent, dest);
        }

        List<Vector2Float> CalcCellPathFromParent(Dictionary<Pos, Pos> parent, Pos dest)
        {
            List<Vector2Float> cells = new List<Vector2Float>();

            if (parent.ContainsKey(dest) == false)
            {
                Pos best = new Pos();
                int bestDist = Int32.MaxValue;

                foreach (Pos pos in parent.Keys)
                {
                    float dist = Math.Abs(dest.X - pos.X) + Math.Abs(dest.Y - pos.Y);
                    // 제일 우수한 후보를 뽑는다
                    if (dist < bestDist)
                    {
                        best = pos;
                        bestDist = (int)dist;
                    }
                }

                dest = best;
            }

            {
                Pos pos = dest;
                while (parent[pos] != pos)
                {
                    cells.Add(Pos2Cell(pos));
                    pos = parent[pos];
                }

                cells.Add(Pos2Cell(pos));
                cells.Reverse();
            }

            return cells;
        }

        Pos Cell2Pos(Vector2Float cell)
        {
            // CellPos -> ArrayPos
            return new Pos(MaxY - (int)cell.y, (int)cell.x - MinX);
        }

        Vector2Float Pos2Cell(Pos pos)
        {
            // ArrayPos -> CellPos
            return new Vector2Float(pos.X + MinX, MaxY - pos.Y);
        }

        #endregion
    }
}