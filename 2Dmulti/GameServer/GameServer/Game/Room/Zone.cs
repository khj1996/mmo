using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Game
{
    public class Zone
    {
        //격자 형태로 나눈 존의 위치
        public int IndexY { get; private set; }
        public int IndexX { get; private set; }

        //현재 존에 들어가있는 객체 관리
        public HashSet<Player> Players { get; set; } = new();
        public HashSet<Monster> Monsters { get; set; } = new();
        public HashSet<Projectile> Projectiles { get; set; } = new();

        //존 생성자
        public Zone(int y, int x)
        {
            IndexY = y;
            IndexX = x;
        }


        //존에 있는 유닛 제거
        public void Remove(GameObject gameObject)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            switch (type)
            {
                case GameObjectType.Player:
                    Players.Remove((Player)gameObject);
                    break;
                case GameObjectType.Monster:
                    Monsters.Remove((Monster)gameObject);
                    break;
                case GameObjectType.Projectile:
                    Projectiles.Remove((Projectile)gameObject);
                    break;
            }
        }

        //유저 탐색
        public Player FindOnePlayer(Func<Player, bool> condition)
        {
            foreach (Player player in Players)
            {
                if (condition.Invoke(player))
                    return player;
            }

            return null;
        }

        //유저 전부 탐색
        public List<Player> FindAllPlayers(Func<Player, bool> condition)
        {
            List<Player> findList = new List<Player>();

            foreach (Player player in Players)
            {
                if (condition.Invoke(player))
                    findList.Add(player);
            }

            return findList;
        }
    }
}