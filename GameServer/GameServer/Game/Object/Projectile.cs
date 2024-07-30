using System.Numerics;
using Google.Protobuf.Protocol;

namespace GameServer.Game
{
    public class Projectile : GameObject
    {
        public Data.Skill Data { get; set; }

        public Vector3 moveDir { get; set; }

        public Projectile()
        {
            ObjectType = GameObjectType.Projectile;
        }

        public override void Update()
        {

        }
    }
}