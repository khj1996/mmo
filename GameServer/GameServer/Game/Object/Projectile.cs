using System.Numerics;
using Google.Protobuf.Protocol;

namespace GameServer.Game
{
    public class Projectile : GameObject
    {
        public Data.Skill Data { get; set; }
        public bool isDestory = false;

        public Projectile()
        {
            ObjectType = GameObjectType.Projectile;
        }

        public void Start(int delay)
        {
            Room.PushAfter(delay, Update);
        }

        public override void Update()
        {
        }
    }
}