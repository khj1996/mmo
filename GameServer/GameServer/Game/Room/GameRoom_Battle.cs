using Google.Protobuf;
using Google.Protobuf.Protocol;
using GameServer.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace GameServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        /// <summary>
        /// 위치 정보만 갱신해주고 실질적인 오브젝트의 이동은 으브젝트 함수 내에서 실행
        /// </summary>
        /// <param name="player"></param>
        /// <param name="movePacket"></param>
        public void HandleMove(Player? player, C_Move movePacket)
        {
            if (player == null)
                return;

            player.Pos = movePacket.PosInfo.Pos;
            player.Move = movePacket.PosInfo.Move;
            player.LookDir = movePacket.PosInfo.LookDir;
            player.State = movePacket.PosInfo.State;


            Broadcast(player.CellPos, movePacket, player.PlayerDbId);
        }

        public void HandleSkill(Player? player, C_Skill skillPacket)
        {
            if (player == null)
                return;

            var info = player.Info;

            if (info.PosInfo.State != CreatureState.Idle)
                return;

            info.PosInfo.State = CreatureState.Skill;
            var skill = new S_Skill
            {
                Info = new SkillInfo()
                {
                    SkillId = skillPacket.Info.SkillId
                },
                ObjectId = info.ObjectId,
                MoveDir = skillPacket.MoveDir
            };
            Broadcast(player.CellPos, skill);

            if (!DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out var skillData))
                return;

            switch (skillData.skillType)
            {
                case SkillType.SkillAuto:
                {
                    var skillPos = player.GetFrontPos();
                    var targets = player.Room?.FindSquarePlayer(skillPos, 0.5f);


                    if (targets != null)
                    {
                        Console.WriteLine(targets.Count);
                        foreach (var target in targets)
                        {
                            target.OnDamaged(player, skillData.damage + player.TotalAttack);
                            Console.WriteLine("Hit GameObject !");
                        }
                    }
                }
                    break;
                case SkillType.SkillProjectile:
                {
                    var arrow = ObjectManager.Instance.Add<Arrow>();
                    if (arrow == null)
                        return;

                    arrow.Owner = player;
                    arrow.Data = skillData;
                    arrow.Info.PosInfo.State = CreatureState.Moving;
                    arrow.Pos = player.Pos;
                    arrow.Move = skillPacket.MoveDir;
                    arrow.Info.PosInfo.LookDir = skillPacket.MoveDir;
                    arrow.Move = skillPacket.MoveDir;
                    arrow.Speed = skillData.projectile.speed;
                    Push(EnterGame, arrow);
                }
                    break;
            }
        }
    }
}