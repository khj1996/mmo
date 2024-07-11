﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using GameServer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;

            ObjectInfo info = player.Info;
            if (info.PosInfo.State != CreatureState.Idle)
                return;

            // TODO : 스킬 사용 가능 여부 체크
            info.PosInfo.State = CreatureState.Skill;
            S_Skill skill = new S_Skill() { Info = new SkillInfo() };
            skill.ObjectId = info.ObjectId;
            skill.Info.SkillId = skillPacket.Info.SkillId;
            Broadcast(player.CellPos, skill);

            Data.Skill skillData = null;
            if (DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out skillData) == false)
                return;

            switch (skillData.skillType)
            {
                case SkillType.SkillAuto:
                {
                    //TODO : 공격 가능하게 
                    /*Vector2Int skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                    GameObject target = Map.Find(skillPos);
                    if (target != null)
                    {
                        Console.WriteLine("Hit GameObject !");
                    }*/
                }
                    break;
                case SkillType.SkillProjectile:
                {
                    Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                    if (arrow == null)
                        return;

                    arrow.Owner = player;
                    arrow.Data = skillData;
                    arrow.PosInfo.State = CreatureState.Moving;
                    arrow.Dir =  skillPacket.MoveDir;
                    arrow.PosInfo.PosX = player.PosInfo.PosX;
                    arrow.PosInfo.PosY = player.PosInfo.PosY;
                    arrow.Speed = skillData.projectile.speed;
                    Push(EnterGame, arrow, false);
                }
                    break;
            }
        }
    }
}