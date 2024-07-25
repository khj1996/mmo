using Google.Protobuf;
using Google.Protobuf.Protocol;
using GameServer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Game
{
    public partial class GameRoom : JobSerializer
    {
        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;

            // TODO : 검증
            PositionInfo movePosInfo = movePacket.PosInfo;
            ObjectInfo info = player.Info;

            // TODO :다른 좌표로 이동할 경우, 갈 수 있는지 체크
            /*if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
            {
                if (Map.CanGo(new Vector2Float(movePosInfo.PosX, movePosInfo.PosY)) == false)
                    return;
            }*/


            player.State = movePosInfo.State;

            var result = Map.ApplyMove(player, new Vector2Float()
            {
                x = movePacket.PosInfo.PosX,
                y = movePacket.PosInfo.PosY,
            }, false);

            // 다른 플레이어한테도 알려준다
            S_Move resMovePacket = new S_Move();
            resMovePacket.ObjectId = player.Info.ObjectId;
            resMovePacket.PosInfo = movePacket.PosInfo;

            //이동 불가능지역 이동시 이전 위치 패킷 전송
            if (!result)
            {
                player.Session.Send(resMovePacket);
            }

            Broadcast(player.CellPos, resMovePacket);
        }

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
                    arrow.Info.PosInfo.State = CreatureState.Moving;
                    arrow.Info.PosInfo.MoveDir = player.Info.PosInfo.MoveDir;
                    arrow.Info.PosInfo.PosX = player.Info.PosInfo.PosX;
                    arrow.Info.PosInfo.PosY = player.Info.PosInfo.PosY;
                    arrow.Speed = skillData.projectile.speed;
                    Push(EnterGame, arrow);
                }
                    break;
            }
        }
    }
}