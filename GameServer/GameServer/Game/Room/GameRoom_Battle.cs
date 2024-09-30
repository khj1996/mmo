using Google.Protobuf;
using Google.Protobuf.Protocol;
using GameServer.Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        public void HandleMove(Player? player, C_Move movePacket)
        {
            if (player == null)
                return;

            // TODO : 검증
            var movePosInfo = movePacket.PosInfo;
            player.State = movePosInfo.State;

            var result = Map.ApplyMove(player, new Vector2Float()
            {
                x = movePacket.PosInfo.PosX,
                y = movePacket.PosInfo.PosY,
            });

            //이동 불가능지역 이동시 이전 위치 패킷 전송
            if (!result)
            {
                return;
            }

            // 다른 플레이어한테도 알려준다
            var resMovePacket = new S_Move
            {
                ObjectId = player.Info.ObjectId,
                PosInfo = movePacket.PosInfo
            };


            Broadcast(player.CellPos, resMovePacket, player.PlayerDbId);
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
                    arrow.Info.PosInfo.MoveDir = player.Info.PosInfo.MoveDir;
                    arrow.Info.PosInfo.PosX = player.Info.PosInfo.PosX;
                    arrow.Info.PosInfo.PosY = player.Info.PosInfo.PosY;
                    Console.WriteLine(arrow.Info.PosInfo.PosX + "," + arrow.Info.PosInfo.PosY);
                    arrow.Info.PosInfo.LookDir = skillPacket.MoveDir;
                    arrow.moveDir = new Vector3(skillPacket.MoveDir.X, skillPacket.MoveDir.Y, skillPacket.MoveDir.Z);
                    arrow.Speed = skillData.projectile.speed;
                    Push(EnterGame, arrow);
                }
                    break;
            }
        }
    }
}