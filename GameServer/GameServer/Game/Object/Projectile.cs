using System.Numerics;
using Google.Protobuf.Protocol;

namespace GameServer.Game
{
    public class Projectile : GameObject
    {
        public Data.Skill Data { get; set; }

        public Vector2Float moveDir { get; set; }

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