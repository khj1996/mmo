using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;

namespace GameServer.Game.Room
{
    public class VisionCube
    {
        //시야의 소유주
        public Player Owner { get; private set; }

        //시야 에들어와있는 물체
        public HashSet<GameObject> PreviousObjects { get; private set; } = new HashSet<GameObject>();

        //생성자
        public VisionCube(Player owner)
        {
            Owner = owner;
        }

        //시야 내부에 존재하는 오브젝트 획득
        public HashSet<GameObject> GatherObjects()
        {
            if (Owner == null || Owner.Room == null)
                return null;

            HashSet<GameObject> objects = new HashSet<GameObject>();

            //근접한 존 획득
            List<Zone> zones = Owner.Room.GetAdjacentZones(Owner.CellPos);

            //현재 유저의 위치
            Vector2Float cellPos = Owner.CellPos;
            //각 존마다 반복하여 획득 가능한 오브젝트 획득
            foreach (Zone zone in zones)
            {
                foreach (Player player in zone.Players)
                {
                    if (player.PlayerDbId == Owner.PlayerDbId)
                        continue;

                    var dis = (player.CellPos - cellPos).Magnitude;

                    if (dis > GameRoom.VisionDis)
                        continue;

                    objects.Add(player);
                }

                foreach (Monster monster in zone.Monsters)
                {
                    var dis = (monster.CellPos - cellPos).Magnitude;

                    if (dis > GameRoom.VisionDis)
                        continue;

                    objects.Add(monster);
                }

                foreach (Projectile projectile in zone.Projectiles)
                {
                    var dis = (projectile.CellPos - cellPos).Magnitude;

                    if (dis > GameRoom.VisionDis)
                        continue;

                    objects.Add(projectile);
                }
            }

            return objects;
        }

        //업데이트
        public void Update()
        {
            if (Owner == null || Owner.Room == null)
                return;

            HashSet<GameObject> currentObjects = GatherObjects();

            // 기존엔 없었는데 새로 생긴 애들 Spawn 처리
            List<GameObject> added = currentObjects.Except(PreviousObjects).ToList();
            if (added.Count > 0)
            {
                S_Spawn spawnPacket = new S_Spawn();

                foreach (GameObject gameObject in added)
                {
                    ObjectInfo info = new ObjectInfo();
                    info.MergeFrom(gameObject.Info);
                    spawnPacket.Objects.Add(info);
                }

                Owner.Session.Send(spawnPacket);
            }

            // 기존엔 있었는데 사라진 애들 Despawn 처리
            List<GameObject> removed = PreviousObjects.Except(currentObjects).ToList();
            if (removed.Count > 0)
            {
                S_Despawn despawnPacket = new S_Despawn();

                foreach (GameObject gameObject in removed)
                {
                    despawnPacket.ObjectIds.Add(gameObject.Id);
                }

                Owner.Session.Send(despawnPacket);
            }

            // 교체
            PreviousObjects = currentObjects;

            Owner.Room.PushAfter(100, Update);
        }
    }
}